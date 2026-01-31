using System.Collections;
using UnityEngine;

public class SoundSlotSpinController : MonoBehaviour
{
    [SerializeField] private SoundId slotSpinId;

    private AudioSource src;
    private SoundEffectGroup group;
    private Coroutine sequenceRoutine;

    void Awake()
    {
        src = gameObject.AddComponent<AudioSource>();
        src.playOnAwake = false;
        src.loop = false;
    }

    void Start()
    {
        group = SoundManager.Instance.GetGroup(slotSpinId);
    }

    //長いSE：タイミングを合わすことができないため、利用しない方が良い
    public void StartSpin()
    {
        if (group == null || group.clips.Count == 0)
            return;

        StopAll();

        src.clip = group.clips[0];
        src.volume = group.volume;
        src.pitch = 1f;
        src.loop = true;
        src.Play();
    }

    //Player pulls lever → finish sequence
    public void StopSpinAndResolve()
    {
        if (sequenceRoutine != null)
            return;

        sequenceRoutine = StartCoroutine(PlaySequence());
    }

    private IEnumerator PlaySequence()
    {
        src.loop = false;

        for (int i = 1; i < group.clips.Count; i++)
        {
            src.clip = group.clips[i];
            src.Play();
            yield return new WaitWhile(() => src.isPlaying);
        }
        sequenceRoutine = null;
    }

    public void StopAll()
    {
        if (sequenceRoutine != null)
        {
            StopCoroutine(sequenceRoutine);
            sequenceRoutine = null;
        }

        src.Stop();
    }
}
