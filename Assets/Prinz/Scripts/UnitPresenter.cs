using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class UnitPresenter: MonoBehaviour
{
    public UnitModel Model { get; private set; }
    public UnitView View { get; private set; }

    private C_MapManager _mapManager;

    private UnitStateMachine _stateMachine;

    UnitData Data;

    public void Initialize(UnitData data, Vector3 currentPos, Vector3 enemyPos)
    {
        View = GetComponent<UnitView>();
        CreateModelFromData(data);

        M_MapPosition mp = _mapManager.ConvertToMapPos(currentPos);
        Vector3 up = _mapManager.ConvertToUnityPos(mp);
        Debug.Log($"mp: {mp.X}/{mp.Y}, up: {up}");
        gameObject.transform.position = up;

        // 位置をマップ座標に変換
        if (_mapManager)
        {
            M_MapPosition start = _mapManager.ConvertToMapPos(transform.position);
            M_MapPosition end = _mapManager.ConvertToMapPos(enemyPos);
            Model.SetEnemyPolayerPos(end);
            Model.SetRoute(C_PathSearch.GetPath(_mapManager.GetAllRoute(), start, end));
        }

        //キャラクタの向きを初期化する
        if (data.MoveDirection.x < 0)
            FaceLeft(transform);
        else FaceRight(transform);

        //   model.OnHealthChanged += OnHealthChanged;
    }

/*    private void InitializeModel()
    {
        Model.SetPlayerSide(Data.PlayerSide);
        Model.SetMaxHealth(Data.MaxHealth);
        Model.SetHealth(Data.MaxHealth);
        Model.SetAttackPower(Data.AttackPower);
        Model.SetAttackSpeed(Data.AttackSpeed);
        Model.SetMoveSpeed(Data.MoveSpeed);
        Model.SetUnitCost(Data.BaseUnitCost);
        Model.SetUnitCoolDown(Data.BaseUnitCoolDown);
        Model.SetMoveDirection(Data.MoveDirection);
    }*/

    private void OnHealthChanged()
    {
        View.UpdateHealth(Model.Health);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    protected void CreateModelFromData(UnitData data)
    {
        if (data == null)
        {
            Debug.LogError("UnitPresenter.Initialize called with null UnitData");
            return;
        }

        // map
        _mapManager = FindFirstObjectByType<C_MapManager>();

        //pick the right model subclass
        if (data is KnightData kd)
        {
            Model = new KnightModel(kd);
            _stateMachine = new UnitStateMachine();
            _stateMachine.Initialize(new MoveState(Model, this)); //change to IdleState(Model, this) if needed
        }
        else if (data is ArcherData ad)
        {
            Model = new ArcherModel(ad);
            _stateMachine = new UnitStateMachine();
            _stateMachine.Initialize(new MoveState(Model, this)); //change to IdleState(Model, this) if needed
        }
        else if (data is MageData md)
        {
            Model = new MageModel(md);
            _stateMachine = new UnitStateMachine();
            _stateMachine.Initialize(new MoveState(Model, this)); //change to IdleState(Model, this) if needed
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
        Model = null; // clear model to avoid stale state when pooled
    }

    // Update is called once per frame
    void Update()
    {
        Model?.Tick(this);
        _stateMachine?.Tick(Time.deltaTime);
     //   ShowRange(true);
    }

    public C_MapManager MapManager { get { return _mapManager; } }

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
        //ally search logic here somewhere maybe
        return false; 
    }

    public void SpawnProjectile(GameObject prefab, float speed, float damage)
    {
        // Instantiate, configure velocity and damage here
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

    private void ApplyAttackRange()
    {
        if (View?.AttackRangeTransform == null)
        {
            Debug.LogError("AttackRangeTransform not found");
            return;
        }
        float newRange = Model.CurrentRange;

        // Adjust collider width AND sprite size by scaling the child object
        Vector3 scale = View.AttackRangeTransform.localScale;
        scale.x = newRange;     // grow horizontally
        scale.y = newRange;     // grow vertically (if needed)
        View.AttackRangeTransform.localScale = scale;
    }

    public void SetRangeBuff(float factor)
    {
        Model?.SetRangeBuff(factor);
        ApplyAttackRange();
    }

    public void ShowRange(bool show)
    {
        View?.ShowAttackRange(show);
    }

    public bool AllowDetection =>
       _stateMachine.Current is MoveState ||
       _stateMachine.Current is IdleState;

    public void OnEnterRange(Collider2D other)
    {
        Debug.LogError($"PRESENTER : EnterRange trigger with {other.gameObject.name}");

        // Only accept valid UnitPresenters with opposite team
        if (!other.TryGetComponent<UnitPresenter>(out var target)) return;
        if (target.Model.PlayerSide == Model.PlayerSide) return;

        Model.AddTarget(target);

        // Tell the FSM that we may need to switch state
    //    _stateMachine.TrySetState(new IdleState());
    }
}
