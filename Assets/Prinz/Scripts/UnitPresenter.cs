using UnityEngine;

public class UnitPresenter: MonoBehaviour
{
    private UnitModel model;
    private UnitView view;
    UnitData Data;

    public void Initialize(UnitData data)
    {
        view = GetComponent<UnitView>();
        CreateModelFromData(data);

        //   model.OnHealthChanged += OnHealthChanged;
    }

    private void InitializeModel()
    {
        model.SetPlayerSide(Data.PlayerSide);
        model.SetMaxHealth(Data.MaxHealth);
        model.SetHealth(Data.MaxHealth);
        model.SetAttackPower(Data.AttackPower);
        model.SetAttackSpeed(Data.AttackSpeed);
        model.SetMoveSpeed(Data.MoveSpeed);
        model.SetUnitCost(Data.UnitCost);
        model.SetUnitCoolDown(Data.UnitCoolDown);
        model.SetMoveDirection(Data.MoveDirection);
    }

    private void OnHealthChanged()
    {
        view.UpdateHealth(model.Health);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Application.targetFrameRate = 60;
 
    }

    protected void CreateModelFromData(UnitData data)
    {
        if (data == null)
        {
            Debug.LogError("UnitPresenter.Initialize called with null UnitData");
            return;
        }

        //pick the right model subclass
        if (data is KnightData kd)
            model = new KnightModel(kd);
        else if (data is ArcherData ad)
            model = new ArcherModel(ad);
        else if (data is MageData md)
            model = new MageModel(md);
        else
        {
            Debug.LogError("Unknown data type: " + data.GetType());
        }
    }

    private void OnDisable()
    {
      //  model.OnHealthChanged -= OnHealthChanged;
    //    view?.PlayMove();
        model = null; // clear model to avoid stale state when pooled
    }

    public void SetView(UnitView view)
    {
        this.view = view;
    }

    public void Move(float movespeed, Vector3 direction)
    {
        transform.Translate(direction * movespeed * Time.deltaTime);
        view?.PlayMove();
    }

    // Update is called once per frame
    void Update()
    {
        model?.Tick(this);
    }

    public void Takedamage(int dmg)
    {
        model.SetHealth(model.Health - dmg);
    }

    public bool IsEnemyInRange(float range) { /* ...*/ return false; }
    public void PerformMeleeAttack(float dmg) { /* ... */ view?.PlayAttack(); }
    public void PerformMagicAttack(float dmg) { /* ... */ view?.PlayAttack(); }
    public void ReceiveHeal(float amount) { /* ... */ view?.PlayHeal(); }

    public void PlayHealVFX() { /* particles */ }

    public bool TryGetLowHpAlly(out UnitPresenter ally)
    {
        ally = null;
        return false; //ally search logic here somewhere maybe
    }

    public void SpawnProjectile(GameObject prefab, float speed, float damage)
    {
        // Instantiate, configure velocity and damage
        view?.PlayAttack();
    }
}
