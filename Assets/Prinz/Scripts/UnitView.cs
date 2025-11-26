using UnityEngine;

public class UnitView : MonoBehaviour
{
    private UnitPresenter presenter;
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Application.targetFrameRate = 60;
    }

    private void Awake()
    {
        presenter = GetComponent<UnitPresenter>();
    //    presenter.SetView(this);
        Animator = GetComponent<Animator>();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
