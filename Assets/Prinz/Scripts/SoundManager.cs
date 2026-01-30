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
    BigBuff,
    Mining,
    RockBreak,
    Chop,
    SlotSpin,
    SlotJackpot,
    SlotClick,
    Impact
}

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }
    [SerializeField] private SoundSlotSpinController soundSlotSpinController;

    [SerializeField] private List<SoundEffectGroup> soundGroups;
    [SerializeField] private int initialPoolSize = 3;
    public int maxPoolSize { get; private set; }

    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioClip bgmMainClip;
    [SerializeField] private AudioClip bgmTitleClip;
    [SerializeField][Range(0f, 1f)] private float _titleBgmVolume = 1f;
    [SerializeField][Range(0f, 1f)] private float _mainBgmVolume = 1f;


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

        maxPoolSize = initialPoolSize;

        pool = new AudioSourcePool(initialPoolSize, transform, maxPoolSize);

        soundMap = new Dictionary<SoundId, SoundEffectGroup>();
        foreach (var group in soundGroups)
            soundMap[group.id] = group;
    }

    void Start()
    {

    }

    public SoundEffectGroup GetGroup(SoundId id)
    {
        soundMap.TryGetValue(id, out var group);
        return group;
    }

    public void PlayMainBGM()
    {
        if (bgmSource == null || bgmMainClip == null) return;

        bgmSource.clip = bgmMainClip;
        bgmSource.loop = true;
        bgmSource.volume = _mainBgmVolume;
        bgmSource.Play();
    }

    public void PlayTitleBGM()
    {
        if (bgmSource == null || bgmTitleClip == null) return;

        bgmSource.clip = bgmTitleClip;
        bgmSource.loop = true;
        bgmSource.volume = _titleBgmVolume;
        bgmSource.Play();
    }

    public void StopCurrentBGM()
    {
        if (bgmSource == null || bgmMainClip == null) return;

        bgmSource.Stop();
        bgmSource.clip = null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="p"></param>
    public void PlaySE(SoundId id, SEPlayParams p)
    {
        if (!soundMap.TryGetValue(id, out var group))
            return;

        if (group.clips == null || group.clips.Count == 0)
            return;

        // ---------- FRAME GUARD ----------
        int currentFrame = Time.frameCount;
        if (!p.ignoreFrameGuard && 
            lastPlayedFrame.TryGetValue(id, out int lastFrame) &&
            lastFrame == currentFrame)
            return;

        lastPlayedFrame[id] = currentFrame;

        // ---------- COOLDOWN GUARD ----------
        float currentTime = Time.time;
        if (!p.ignoreCooldown && 
            lastPlayedTime.TryGetValue(id, out float lastTime) &&
            currentTime - lastTime < group.cooldown)
            return;

        lastPlayedTime[id] = currentTime;

        // ---------- PLAY ----------
        var src = pool.Get();

        src.volume = group.volume;
        src.pitch = Random.Range(group.pitchMin, group.pitchMax);
        src.loop = p.loop;

        
        src.clip = p.clipIndex.HasValue
            ? group.clips[p.clipIndex.Value]
            : group.clips[Random.Range(0, group.clips.Count)];

        src.Play();

        StartCoroutine(ReturnWhenFinished(src));
    }

    private IEnumerator ReturnWhenFinished(AudioSource src)
    {
        yield return new WaitWhile(() => src.isPlaying);
        pool.Release(src);
    }

    public void StartSlotSpinSE()
    {
        soundSlotSpinController.StartSpin();
    }

    public void StopSlotSpinSE()
    {
        soundSlotSpinController.StopSpinAndResolve();
    }

    public void StopAllSlotSpinSE()
    {
        soundSlotSpinController.StopAll();
    }
}
