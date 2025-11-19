using UnityEngine;

public class UnitPresenter: MonoBehaviour
{
    public UnitModel model;

    private UnitView view;

    [SerializeField]
    UnitData unitData;

    public void Initialize()
    {
        this.model = new UnitModel();
        InitializeModel();

        //   model.OnHealthChanged += OnHealthChanged;
    }

    private void InitializeModel()
    {
        model.SetPlayerSide(unitData.PlayerSide);
        model.SetMaxHealth(unitData.MaxHealth);
        model.SetHealth(unitData.MaxHealth);
        model.SetAttackPower(unitData.AttackPower);
        model.SetAttackSpeed(unitData.AttackSpeed);
        model.SetMoveSpeed(unitData.MoveSpeed);
        model.SetUnitCost(unitData.UnitCost);
        model.SetUnitCoolDown(unitData.UnitCoolDown);
        model.SetMoveDirection(unitData.MoveDirection);
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

    void Takedamage(int dmg)
    {
        model.SetHealth(model.Health - dmg);
    }
}
