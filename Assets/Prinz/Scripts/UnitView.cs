using UnityEngine;

public class UnitView : MonoBehaviour
{
    private UnitPresenter presenter;
    public Animator animator;

    public void UpdateHealth(float hp)
    {
        // update sprite, bar, etc.
    }

    public void PlayMoveAnimation()
    {
        animator.SetBool("isWalking", true);
    }

    public void StopMoveAnimation()
    {
        animator.SetBool("isWalking", false);
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
        animator = GetComponent<Animator>();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
