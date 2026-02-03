using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class UnitView : MonoBehaviour
{
    public UnitPresenter presenter;
    [SerializeField] private GameObject _unitSprite;
    private SpriteRenderer _unitSpriteRenderer;
    public Transform AttackRangeTransform { get; private set; }
    public Collider2D AttackRangeCollider { get; private set; }
    public SpriteRenderer AttackRangeSprite { get; private set; }

    [SerializeField]
    private V_HealthGauge _healthGauge;

    public Animator Animator;

    // 2026.01.16 ウー start バフのエフェクト追加
    /// <summary>
    /// 
    /// </summary>
    private const float OUTLINE_WIDTH = 0.005f;

    // 2026.01.16 ウー end バフのエフェクト追加

    public void PlayAttack() => Animator.SetTrigger("Attack");

    /// <summary>
    /// ArcherとMage用。攻撃が実際解放されたら呼ぶ関数。PlayAttack() -> 待つ -> StopAttack() -> ダメージ
    /// </summary>
    public void StopAttack() => Animator.SetTrigger("StopAttack");

    public void TriggerIdle() => Animator.SetTrigger("Idle");

    public void PlayMove(bool move) => Animator.SetBool("Move", move);
    public void FaceUP(bool up) => Animator.SetBool("FacingUP", up);
    public void FaceDOWN(bool down) => Animator.SetBool("FacingDOWN", down);
    public void PlayDeath(bool dead) => Animator.SetBool("Dead", dead);
    public void PlayVictoryDance(bool win) => Animator.SetBool("VictoryDance", win);
    public void PlayDefeatAnimation(bool lose) => Animator.SetBool("DefeatAnim", lose);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log($"{_unitSpriteRenderer.material.name}");
    }

    private void Awake()
    {
        //    InitializeView();
    }

    public void ResetAllAnimations()
    {
        PlayDeath(false);
        PlayMove(false);
        PlayVictoryDance(false);
        PlayDefeatAnimation(false);
    }

    public void UpdateHealth(float hp)
    {
        _healthGauge.SetGauge(hp);
        //    Debug.LogWarning("SetGauge called in View");
    }

    public void EnableAttackRange(bool enable)
    {
        AttackRangeTransform.gameObject.SetActive(enable);
    }

    public void OnDeathAnimationEnd()
    {
        //    Debug.LogWarning("Death animation ended");
        //    PlayDeath(false);
        _healthGauge?.HideGauge();
        presenter?.Release();
    }

    public void OnDefeatAnimationStart()
    {
        _healthGauge?.HideGauge();
    }
    public void OnDefeatAnimationEnd()
    {
        presenter?.FreezeState(true);
    }

    public void FaceRight()
    {
        presenter?.FaceRight();
    }

    public void FaceLeft()
    {
        presenter?.FaceLeft();
    }

    public void InitializeView()
    {
        presenter = GetComponent<UnitPresenter>();
        //    Animator = GetComponent<Animator>();

        RandomizeSpriteOffset();
        _unitSpriteRenderer = _unitSprite.GetComponent<SpriteRenderer>();

        AttackRangeTransform = transform.Find("AttackRange");
        var DataType = presenter.GetDataType();
        // 2026.01.27 ウー start
        //if (DataType is KnightData)
        if (DataType is KnightData || DataType is M_MinerData)
        // 2026.01.27 ウー end
        {
            AttackRangeCollider = AttackRangeTransform.GetComponent<BoxCollider2D>();
        }
        else if (DataType is ArcherData || DataType is MageData)
        {
            AttackRangeCollider = AttackRangeTransform.GetComponent<CircleCollider2D>();
        }
        else { Debug.LogError("Collider reference not found"); }

        AttackRangeSprite = AttackRangeTransform.GetComponent<SpriteRenderer>();

        AttackRangeSprite.color = Color.lightGreen;

        _healthGauge.ShowGauge();

    }

    // Update is called once per frame
    void Update()
    {
        UpdateOrderInLayer();
    }

    public void OnEnterRange(Collider2D other)
    {
        if (!presenter.AllowDetection) return;

        //    Debug.LogWarning($"VIEW : EnterRange trigger with {other.gameObject.name}");
        UpdateAttackRangeSpriteColor();
        presenter.OnEnterRange(other);
    }

    public void OnExitRange(Collider2D other)
    {
        //    Debug.LogWarning($"VIEW : ExitRange trigger with {other.gameObject.name}");
        UpdateAttackRangeSpriteColor();
        presenter.OnExitRange(other);
    }

    private void UpdateOrderInLayer()
    {
        float yPosition = _unitSprite.transform.position.y * 100.0f + transform.position.y;
        _unitSpriteRenderer.sortingOrder = 100 - (int)(yPosition);
    }

    public void RandomizeSpriteOffset()
    {
        float offsetX = Random.Range(-0.2f, 0.2f);
        float offsetY = Random.Range(-0.2f, 0.2f);
        Vector3 spritePos = new Vector3(offsetX, offsetY, 0f);

        _unitSprite.transform.Translate(spritePos);
    }

    public void UpdateAttackRangeSpriteColor()
    {
        Color c;

        if (presenter.IsValidTargetExist()) c = Color.softRed;
        else c = Color.lightGreen;

        //alpha変更
        c.a = 0.3f;
        AttackRangeSprite.color = c;
    }

    public void RenamePrefab(int serialNumber)
    {
        string currentName = gameObject.name;
        string newName = currentName + $"{serialNumber}";
        gameObject.name = newName;
    }

    #region Sound Effects

    public void PlaySwordAttackSE()
    {
        SoundManager.Instance.PlaySE(SoundId.SwordAttack, SEPlayParams.Default);
    }

    public void PlaySwordBlockSE()
    {
        SoundManager.Instance.PlaySE(SoundId.SwordBlock, SEPlayParams.Default);
    }

    public void PlaySwordImpactSE()
    {
        SoundManager.Instance.PlaySE(SoundId.SwordImpact, SEPlayParams.Default);
    }

    public void PlaySwordParrySE()
    {
        SoundManager.Instance.PlaySE(SoundId.SwordParry, SEPlayParams.Default);
    }

    public void PlayBowAttackSE()
    {
        SoundManager.Instance.PlaySE(SoundId.BowAttack, SEPlayParams.Default);
    }

    public void PlayBowBlockSE()
    {
        SoundManager.Instance.PlaySE(SoundId.BowBlock, SEPlayParams.Default);
    }

    public void PlayBowImpactSE()
    {
        SoundManager.Instance.PlaySE(SoundId.BowImpact, SEPlayParams.Default);
    }

    public void PlayBuffSE()
    {
        SoundManager.Instance.PlaySE(SoundId.Buff, SEPlayParams.Default);
    }

    public void PlayBigBuffSE()
    {
        SoundManager.Instance.PlaySE(SoundId.BigBuff, SEPlayParams.Default);
    }

    public void PlayFireBallSE()
    {
        SoundManager.Instance.PlaySE(SoundId.FireBall, SEPlayParams.Default);
    }

    public void PlaySpellImpactSE()
    {
        SoundManager.Instance.PlaySE(SoundId.SpellImpact, SEPlayParams.Default);
    }

    public void PlayMiningSE()
    {
        SoundManager.Instance.PlaySE(SoundId.Mining, SEPlayParams.Default);
    }

    public void PlayRockBreakSE()
    {
        SoundManager.Instance.PlaySE(SoundId.RockBreak, SEPlayParams.Default);
    }

    public void PlayWoodChopSE()
    {
        SoundManager.Instance.PlaySE(SoundId.Chop, SEPlayParams.Default);
    }

    #endregion

    public void CreateBuffEffect(float basesize)
    {
        Material material = _unitSpriteRenderer.material;
        if (!material)
            return;
        for (int index = 1; index <= 4; index++)
        {
            material.SetFloat($"_OutlineWidth{index}", 0.0f);
            material.SetColor($"_OutlineColor{index}", Color.black);
        }
        for (int index = 1; index <= 4; index++)
        {
            material.SetFloat($"_OutlineWidth{index}", OUTLINE_WIDTH / basesize * index);
            material.SetColor($"_OutlineColor{index}", Color.red);
        }

    }

    // 2026.01.16 ウー start バフのエフェクト追加
    /// <summary>
    /// バフのエフェクトを更新
    /// </summary>
    /// <param name="buffs"></param>
    public void UpdateBuffEffect(List<C_Buff> buffs)
    {
        Material material = _unitSpriteRenderer.material;
        if (!material)
            return;
        Debug.Log($"Update buff effect. Clear");
        // クリア
        for (int index = 1; index <= 4; index++)
        {
            material.SetFloat($"_OutlineWidth{index}", 0.0f);
            material.SetColor($"_OutlineColor{index}", Color.black);
        }
        Debug.Log($"Update buff effect. set");
        // 新しいエフェクトの色を設定
        int buffCount = buffs.Count;
        for (int index = 1; index <= buffCount; index++)
        {
            material.SetFloat($"_OutlineWidth{index}", OUTLINE_WIDTH * index);
            material.SetColor($"_OutlineColor{index}", buffs[index - 1].GetEffectColor());
        }
    }
    // 2026.01.16 ウー end バフエフェクト追加


}
