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
        Application.targetFrameRate = 60;
    }

    private void Awake()
    {
        presenter = GetComponent<UnitPresenter>();
        AttackRangeTransform = transform.Find("KnightAttackRangeClose");
        AttackRangeCollider = AttackRangeTransform.GetComponent<BoxCollider2D>();
        AttackRangeSprite = AttackRangeTransform.GetComponent<SpriteRenderer>();
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

/*    private void OnCollisionEnter2D(Collision2D other)
    {
        Debug.LogError("Colliding called in view");
        presenter.OnCollisionEnter2D(other);
    }*/

    private void OnTriggerEnter2D(Collider2D other)
    {
    }

    private void OnTriggerExit2D(Collider2D other)
    {
     //   Debug.LogError($"VIEW : exit trigger with {other.gameObject.name}");
    }

    public void OnEnterRange(Collider2D other)
    {
        Debug.LogError($"VIEW : EnterRange trigger with {other.gameObject.name}");
        if (!presenter.AllowDetection) return;
        presenter.OnEnterRange(other);
    }

}
