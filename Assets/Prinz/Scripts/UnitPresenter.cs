using UnityEngine;

public class UnitPresenter: MonoBehaviour
{
    private UnitModel _model;
    private UnitView _view;
    private UnitStateMachine _statemachine;
    UnitData Data;

    public void Initialize(UnitData data)
    {
        _view = GetComponent<UnitView>();
        CreateModelFromData(data);

        //キャラクタの向きを初期化する
        if (data.MoveDirection.x < 0)
            FaceLeft(transform);
        else FaceRight(transform);

        //   model.OnHealthChanged += OnHealthChanged;
    }

    private void InitializeModel()
    {
        _model.SetPlayerSide(Data.PlayerSide);
        _model.SetMaxHealth(Data.MaxHealth);
        _model.SetHealth(Data.MaxHealth);
        _model.SetAttackPower(Data.AttackPower);
        _model.SetAttackSpeed(Data.AttackSpeed);
        _model.SetMoveSpeed(Data.MoveSpeed);
        _model.SetUnitCost(Data.UnitCost);
        _model.SetUnitCoolDown(Data.UnitCoolDown);
        _model.SetMoveDirection(Data.MoveDirection);
    }

    private void OnHealthChanged()
    {
        _view.UpdateHealth(_model.Health);
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
            _model = new KnightModel(kd);
        else if (data is ArcherData ad)
            _model = new ArcherModel(ad);
        else if (data is MageData md)
            _model = new MageModel(md);
        else
        {
            Debug.LogError("Unknown data type: " + data.GetType());
            return;
        }

        //キャラクタにAIを付ける
        CreateStateMachineFromModel(_model);
    }

    protected void CreateStateMachineFromModel(UnitModel model)
    {
        _statemachine = new UnitStateMachine(model);
    }

    private void OnDisable()
    {
      //  model.OnHealthChanged -= OnHealthChanged;
    //    view?.PlayMove();
        _model = null; // clear model to avoid stale state when pooled
    }

    public void SetView(UnitView view)
    {
        this._view = view;
    }

    public void Move(float movespeed, Vector3 direction)
    {
        if (direction.x < 0) // TODO : call it when change direction not everyframe
            FaceLeft(transform);
        else FaceRight(transform);

            transform.Translate(direction * movespeed * Time.deltaTime);
        
        _view?.PlayMove();
    }

    // Update is called once per frame
    void Update()
    {
        _model?.Tick(this);
    }

    public void Takedamage(int dmg)
    {
        _model.SetHealth(_model.Health - dmg);
    }

    public bool IsEnemyInRange(float range) { /* ...*/ return false; }
    public void PerformMeleeAttack(float dmg) { /* ... */ _view?.PlayAttack(); }
    public void PerformMagicAttack(float dmg) { /* ... */ _view?.PlayAttack(); }
    public void ReceiveHeal(float amount) { /* ... */ _view?.PlayHeal(); }

    public void PlayHealVFX() { /* particles */ }

    public bool TryGetLowHpAlly(out UnitPresenter ally)
    {
        ally = null;
        return false; //ally search logic here somewhere maybe
    }

    public void SpawnProjectile(GameObject prefab, float speed, float damage)
    {
        // Instantiate, configure velocity and damage
        _view?.PlayAttack();
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
