using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    #region Singleton Implementation
    public static SoundManager Instance { get; private set; }
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        //DontDestroyOnLoad(gameObject);
    }
    #endregion

    public AudioSource SEaudioSource;
    public AudioSource BGMaudioSource;

    public AudioClip Bgm;

    private List<AudioClip> swordAttack;
    public AudioClip swordAttack1;
    public AudioClip swordAttack2;
    public AudioClip swordAttack3;

    private List<AudioClip> swordBlock;
    public AudioClip swordBlock1;
    public AudioClip swordBlock2;
    public AudioClip swordBlock3;

    private List<AudioClip> swordImpact;
    public AudioClip swordImpact1;
    public AudioClip swordImpact2;
    public AudioClip swordImpact3;

    private List<AudioClip> swordParry;
    public AudioClip swordParry1;
    public AudioClip swordParry2;
    public AudioClip swordParry3;

    private List<AudioClip> bowAttack;
    public AudioClip bowAttack1;
    public AudioClip bowAttack2;

    private List<AudioClip> bowBlock;
    public AudioClip bowBlock1;
    public AudioClip bowBlock2;
    public AudioClip bowBlock3;

    private List<AudioClip> bowImpact;
    public AudioClip bowImpact1;
    public AudioClip bowImpact2;
    public AudioClip bowImpact3;



    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        SEaudioSource = GetComponent<AudioSource>();
        if (SEaudioSource == null)
            DebugManager.Instance.Log("AudioSource not found", LogType.Error);

        InitSEList();

        PlayBGM();
    }

    private void InitSEList()
    {
        swordAttack = new List<AudioClip>
        {
            swordAttack1,
            swordAttack2,
            swordAttack3
        };

        swordBlock = new List<AudioClip>
        {
            swordBlock1,
            swordBlock2,
            swordBlock3
        };
        
        swordImpact = new List<AudioClip>
        {
            swordImpact1,
            swordImpact2,
            swordImpact3
        };

        swordParry = new List<AudioClip>
        {
            swordParry1,
            swordParry2,
            swordParry3
        };

        bowAttack = new List<AudioClip>
        {
            bowAttack1,
            bowAttack2
        };

        bowBlock = new List<AudioClip>
        {
            bowBlock1,
            bowBlock2,
            bowBlock3
        };

        bowImpact = new List<AudioClip>
        {
            bowImpact1,
            bowImpact2,
            bowImpact3
        };
    }

    public void PlayBGM()
    {
        BGMaudioSource.Play();
    }

    public void PlaySwordAttack()
    {
        int index = Random.Range(0, swordAttack.Count);
        SEaudioSource.PlayOneShot(swordAttack[index]);
    //    DebugManager.Instance.Log($"SE swordAttack {index} played", LogType.Error);
    }

    public void PlaySwordBlock()
    {
        int index = Random.Range(0, swordBlock.Count);
        SEaudioSource.PlayOneShot(swordBlock[index]);
    }

    public void PlaySwordImpact()
    {
        int index = Random.Range(0, swordImpact.Count);
        SEaudioSource.PlayOneShot(swordImpact[index]);
    }

    public void PlaySwordParry()
    {
        int index = Random.Range(0, swordParry.Count);
        SEaudioSource.PlayOneShot(swordParry[index]);
    }

    public void PlayBowAttack()
    {
        int index = Random.Range(0, bowAttack.Count);
        SEaudioSource.PlayOneShot(bowAttack[index]);
    }

    public void PlayBowBlock()
    {
        int index = Random.Range(0, bowBlock.Count);
        SEaudioSource.PlayOneShot(bowBlock[index]);
    }

    public void PlayBowImpact()
    {
        int index = Random.Range(0, bowImpact.Count);
        SEaudioSource.PlayOneShot(bowImpact[index]);
    }
}
