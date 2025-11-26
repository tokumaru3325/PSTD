using UnityEngine;

public class UnitPresenter: MonoBehaviour
{
    public UnitModel Model { get; private set; }
    public UnitView View { get; private set; }

    private UnitStateMachine _stateMachine;
    UnitData Data;

    public void Initialize(UnitData data)
    {
        View = GetComponent<UnitView>();
        CreateModelFromData(data);

        //キャラクタの向きを初期化する
        if (data.MoveDirection.x < 0)
            FaceLeft(transform);
        else FaceRight(transform);

        //   model.OnHealthChanged += OnHealthChanged;
    }

    private void InitializeModel()
    {
        Model.SetPlayerSide(Data.PlayerSide);
        Model.SetMaxHealth(Data.MaxHealth);
        Model.SetHealth(Data.MaxHealth);
        Model.SetAttackPower(Data.AttackPower);
        Model.SetAttackSpeed(Data.AttackSpeed);
        Model.SetMoveSpeed(Data.MoveSpeed);
        Model.SetUnitCost(Data.UnitCost);
        Model.SetUnitCoolDown(Data.UnitCoolDown);
        Model.SetMoveDirection(Data.MoveDirection);
    }

    private void OnHealthChanged()
    {
        View.UpdateHealth(Model.Health);
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
        {
            Model = new KnightModel(kd);
            _stateMachine = new UnitStateMachine();
            _stateMachine.Initialize(new MoveState(Model, this)); //change to IdleState if needed
        }
        else if (data is ArcherData ad)
        {
            Model = new ArcherModel(ad);
            _stateMachine = new UnitStateMachine();
            _stateMachine.Initialize(new MoveState(Model, this)); //change to IdleState if needed
        }
        else if (data is MageData md)
        {
            Model = new MageModel(md);
            _stateMachine = new UnitStateMachine();
            _stateMachine.Initialize(new MoveState(Model, this)); //change to IdleState if needed
        }
        else
        {
            Debug.LogError("Unknown data type: " + data.GetType());
            return;
        }
    }

    private void OnDisable()
    {
      //  model.OnHealthChanged -= OnHealthChanged;
    //    view?.PlayMove();
        Model = null; // clear model to avoid stale state when pooled
    }

/*    public void SetView(UnitView view)
    {
        View = view;
    }*/

/*    public void Move(float movespeed, Vector3 direction)
    {
        if (direction.x < 0) // TODO : call it when change direction not everyframe
            FaceLeft(transform);
        else FaceRight(transform);

            transform.Translate(direction * movespeed * Time.deltaTime);
        
        View?.PlayMove();
    }*/

    // Update is called once per frame
    void Update()
    {
        Model?.Tick(this);
        _stateMachine?.Tick(Time.deltaTime);
    }

    public void TakeDamage(float dmg)
    {
        Model.SetHealth(Model.Health - dmg);
    }

    public bool IsEnemyInRange(float range) { /* ...*/ return false; }
    public void PerformMeleeAttack(float dmg) { /* ... */ View?.PlayAttack(); }
    public void PerformMagicAttack(float dmg) { /* ... */ View?.PlayAttack(); }
    public void ReceiveHeal(float amount) { /* ... */ View?.PlayHeal(); }

    public void PlayHealVFX() { /* particles */ }

    public bool TryGetLowHpAlly(out UnitPresenter ally)
    {
        ally = null;
        return false; //ally search logic here somewhere maybe
    }

    public void SpawnProjectile(GameObject prefab, float speed, float damage)
    {
        // Instantiate, configure velocity and damage
        View?.PlayAttack();
    }

    public void FaceRight(Transform transform)
    {
        transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
    }

    public void FaceLeft(Transform transform)
    {
        transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
    }
}
