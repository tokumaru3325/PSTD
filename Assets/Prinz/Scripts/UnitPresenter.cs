using UnityEngine;

public class UnitPresenter: MonoBehaviour
{
    public UnitModel model;
    public UnitView view;


    public void Initialize(UnitModel model, UnitView view)
    {
        this.model = model;
        this.view = view;

        model.OnHealthChanged += OnHealthChanged;
    }
    private void OnHealthChanged()
    {
        view.UpdateHealth(model.Health);
    }

    private void OnDisable()
    {
        model.OnHealthChanged -= OnHealthChanged;
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
