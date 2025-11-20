using UnityEngine;

public class UnitView : MonoBehaviour
{
    private UnitPresenter presenter;
    public Animator Animator;

    public void PlayAttack() => Animator.SetTrigger("Attack");
    public void PlayHeal() => Animator.SetTrigger("Heal");
    public void PlayMove() => Animator.SetBool("isWalking", true);
    public void PlayDeath() => Animator.SetTrigger("Die");

    public void UpdateHealth(float hp)
    {
        // update sprite, bar, etc.
    }

    public void PlayMoveAnimation()
    {
        Animator.SetBool("isWalking", true);
    }

    public void StopMoveAnimation()
    {
        Animator.SetBool("isWalking", false);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Application.targetFrameRate = 60;
     //   presenter.SetView(this);

    }

    private void Awake()
    {
        presenter = GetComponent<UnitPresenter>();
        presenter.SetView(this);
        Animator = GetComponent<Animator>();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
