using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundId
{
    SwordAttack,
    SwordBlock,
    SwordImpact,
    SwordParry,
    BowAttack,
    BowBlock,
    BowImpact,
    FireBall,
    SpellImpact,
    Buff,
    BigBuff
}

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [SerializeField] private List<SoundEffectGroup> soundGroups;
    [SerializeField] private int initialPoolSize = 3;
    public int maxPoolSize { get; private set; }

    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioClip bgmMainClip;
    [SerializeField] private AudioClip bgmTitleClip;


    private Dictionary<SoundId, SoundEffectGroup> soundMap;
    //Prevent same-frame spam
    private Dictionary<SoundId, int> lastPlayedFrame = new();
    //Prevent rapid repetition
    private Dictionary<SoundId, float> lastPlayedTime = new();

    private AudioSourcePool pool;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        maxPoolSize = initialPoolSize;

        pool = new AudioSourcePool(initialPoolSize, transform, maxPoolSize);

        soundMap = new Dictionary<SoundId, SoundEffectGroup>();
        foreach (var group in soundGroups)
            soundMap[group.id] = group;

        PlayMainBGM();
    }

    public void PlayMainBGM()
    {
        if (bgmSource == null || bgmMainClip == null) return;

        bgmSource.clip = bgmMainClip;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void StopCurrentBGM()
    {
        if (bgmSource == null || bgmMainClip == null) return;

        bgmSource.Stop();
        bgmSource.clip = null;
    }

    public void PlayTitleBGM()
    {
        if (bgmSource == null || bgmTitleClip == null) return;

        bgmSource.clip = bgmTitleClip;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void PlaySE(SoundId id)
    {
        if (!soundMap.TryGetValue(id, out var group))
            return;

        if (group.clips == null || group.clips.Count == 0)
            return;
        // ---------- FRAME GUARD ----------
        int currentFrame = Time.frameCount;
        if (lastPlayedFrame.TryGetValue(id, out int lastFrame) &&
            lastFrame == currentFrame)
        {
            return;
        }

        lastPlayedFrame[id] = currentFrame;

        // ---------- COOLDOWN GUARD ----------
        float currentTime = Time.time;
        if (lastPlayedTime.TryGetValue(id, out float lastTime) &&
            currentTime - lastTime < group.cooldown)
        {
            return;
        }

        lastPlayedTime[id] = currentTime;

        // ---------- PLAY ----------
        var src = pool.Get();

        src.volume = group.volume;
        src.pitch = Random.Range(group.pitchMin, group.pitchMax);
        src.clip = group.clips[Random.Range(0, group.clips.Count)];
        src.Play();

        StartCoroutine(ReturnWhenFinished(src));
    }

    private IEnumerator ReturnWhenFinished(AudioSource src)
    {
        yield return new WaitWhile(() => src.isPlaying);
        pool.Release(src);
    }
}
