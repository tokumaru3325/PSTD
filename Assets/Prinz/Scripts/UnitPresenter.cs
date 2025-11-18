using UnityEngine;

public class UnitPresenter: MonoBehaviour
{
    public UnitModel model;

    private UnitView view;


    public void Initialize()
    {
        this.model = new UnitModel();

     //   model.OnHealthChanged += OnHealthChanged;
    }
    private void OnHealthChanged()
    {
        view.UpdateHealth(model.Health);
    }

    private void OnDisable()
    {
      //  model.OnHealthChanged -= OnHealthChanged;
    }

    public void SetView(UnitView view)
    {
        this.view = view;
    }

    private void Move()
    {
        transform.position += model.MoveDirection * model.MoveSpeed * Time.deltaTime;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Application.targetFrameRate = 60;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }


}
