using UnityEngine;
using UnityEngine.InputSystem;

public class UnitView : MonoBehaviour
{
    private UnitPresenter presenter;
    public Transform AttackRangeTransform { get; private set; }
    public BoxCollider2D AttackRangeCollider { get; private set; }
    public SpriteRenderer AttackRangeSprite { get; private set; }

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
        InitializeView();
    }

    public void UpdateHealth(float hp)
    {
        // update sprite, bar, etc.
    }

    public void EnableAttackRange(bool enable)
    {
        AttackRangeTransform.gameObject.SetActive(enable);
    }

    public void OnDeathAnimationEnd()
    {
    //    Debug.LogWarning("Death animation ended");
    //    PlayDeath(false);
        presenter.Release();
    }

    public void InitializeView()
    {
        presenter = GetComponent<UnitPresenter>();
        Animator = GetComponent<Animator>();

        AttackRangeTransform = transform.Find("KnightAttackRangeClose");
        AttackRangeCollider = AttackRangeTransform.GetComponent<BoxCollider2D>();
        AttackRangeSprite = AttackRangeTransform.GetComponent<SpriteRenderer>();

        AttackRangeSprite.color = Color.lightGreen;
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
