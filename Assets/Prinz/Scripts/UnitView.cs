using UnityEngine;
using UnityEngine.InputSystem;

public class UnitView : MonoBehaviour
{
    private UnitPresenter presenter;
    public Transform AttackRangeTransform { get; private set; }
    public Collider2D AttackRangeCollider { get; private set; }
    public SpriteRenderer AttackRangeSprite { get; private set; }

    [SerializeField]
    private V_HealthGauge _healthGauge;

    public Animator Animator;

    public void PlayAttack() => Animator.SetTrigger("Attack");
    public void StopAttack() => Animator.SetTrigger("StopAttack");
    public void PlayHeal() => Animator.SetTrigger("Heal");
    public void PlayMove(bool move) => Animator.SetBool("Move", move);
    public void FaceUP(bool up) => Animator.SetBool("FacingUP", up);
    public void FaceDOWN(bool down) => Animator.SetBool("FacingDOWN", down);
    public void PlayDeath(bool dead) => Animator.SetBool("Dead", dead);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    private void Awake()
    {
    //    InitializeView();
    }

    public void UpdateHealth(float hp)
    {
        _healthGauge.SetGauge(hp);
        Debug.LogWarning("SetGauge called in View");
    }

    public void EnableAttackRange(bool enable)
    {
        AttackRangeTransform.gameObject.SetActive(enable);
    }

    public void OnDeathAnimationEnd()
    {
    //    Debug.LogWarning("Death animation ended");
    //    PlayDeath(false);
        _healthGauge.HideGauge();
        presenter.Release();
    }

    public void InitializeView()
    {
        presenter = GetComponent<UnitPresenter>();
        Animator = GetComponent<Animator>();

        AttackRangeTransform = transform.Find("AttackRange");
        var DataType = presenter.Model?.GetDataType();
        if (DataType is KnightData)
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

    }

    public void OnEnterRange(Collider2D other)
    {
        if (!presenter.AllowDetection) return;

    //    Debug.LogWarning($"VIEW : EnterRange trigger with {other.gameObject.name}");
        UpdateAttackRangeSpriteColor();
    //    if (presenter.Model.Targets.Count > 0) AttackRangeSprite.color = Color.softRed;
        presenter.OnEnterRange(other);
    }

    public void OnExitRange(Collider2D other)
    {
    //    Debug.LogWarning($"VIEW : ExitRange trigger with {other.gameObject.name}");
        UpdateAttackRangeSpriteColor();
    //    if(presenter.Model.Targets.Count == 0) AttackRangeSprite.color = Color.lightGreen;
        presenter.OnExitRange(other);
    }

    public void UpdateAttackRangeSpriteColor()
    {
        if (presenter.IsValidTargetExist()) AttackRangeSprite.color = Color.softRed;
        else AttackRangeSprite.color = Color.lightGreen;
    }
}
