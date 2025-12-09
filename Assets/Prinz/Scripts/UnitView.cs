using UnityEngine;
using UnityEngine.InputSystem;

public class UnitView : MonoBehaviour
{
    private UnitPresenter presenter;
    public Transform AttackRangeTransform { get; private set; }
    public BoxCollider2D AttackRangeCollider { get; private set; }
    public SpriteRenderer AttackRangeSprite { get; private set; }
    public KnightAttackRange AttackRange { get; private set; }

    public Animator Animator;

    public void PlayAttack() => Animator.SetTrigger("Attack");
    public void StopAttack() => Animator.SetTrigger("StopAttack");
    public void PlayHeal() => Animator.SetTrigger("Heal");
    public void PlayMove() => Animator.SetBool("Move", true);
    public void StopMove() => Animator.SetBool("Move", false);
    public void PlayDeath() => Animator.SetTrigger("Die");

    public void UpdateHealth(float hp)
    {
        // update sprite, bar, etc.
    }
    public void ShowAttackRange(bool show)
    {
        if (AttackRangeSprite != null)
            AttackRangeSprite.enabled = show;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    private void Awake()
    {
        InitializeView();
    }

    public void InitializeView()
    {
        presenter = GetComponent<UnitPresenter>();
        //   AttackRange = GetComponentInChildren<KnightAttackRange>();
        //  AttackRange.SetView(this);
        AttackRangeTransform = transform.Find("KnightAttackRangeClose");
        AttackRangeCollider = AttackRangeTransform.GetComponent<BoxCollider2D>();
        AttackRangeSprite = AttackRangeTransform.GetComponent<SpriteRenderer>();
        AttackRangeSprite.color = Color.lightGreen;
        Animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            //    presenter.SetRangeBuff(2.0f);
            Debug.Log("buff range button pressed");
        }
        if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            //   presenter.SetRangeBuff(-2.0f);
            Debug.Log("debuff range button pressed");
        }
    }

    public void OnEnterRange(Collider2D other)
    {
        if (!presenter.AllowDetection) return;

        Debug.LogWarning($"VIEW : EnterRange trigger with {other.gameObject.name}");
        if (presenter.Model.Targets.Count > 0) AttackRangeSprite.color = Color.softRed;
        presenter.OnEnterRange(other);
    }

    public void OnExitRange(Collider2D other)
    {
        Debug.LogWarning($"VIEW : ExitRange trigger with {other.gameObject.name}");
        if(presenter.Model.Targets.Count == 0) AttackRangeSprite.color = Color.lightGreen;
        presenter.OnExitRange(other);
    }
}
