using UnityEngine;

public class UnitView : MonoBehaviour
{
    private UnitPresenter presenter;

    public void UpdateHealth(float hp)
    {
        // update sprite, bar, etc.
    }

    public void PlayMoveAnimation()
    { 
        // call animation here
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
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
