using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class UnitPresenter: MonoBehaviour
{
    public UnitModel Model { get; private set; }
    public UnitView View { get; private set; }

    private C_MapManager _mapManager;

    public CapsuleCollider2D Collider;

    private UnitStateMachine _stateMachine;

    public ObjectPoolTest Pool { get; private set; }

    UnitData Data;

    //=====================================================================================================
    #region 初期化 - Initialization
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    private void OnEnable()
    {
    }
    public void Initialize(UnitData data, Vector3 currentPos, Vector3 enemyPos, string PlayerTag)
    {
        View = GetComponent<UnitView>();
        Collider = GetComponent<CapsuleCollider2D>();
        CreateModelFromData(data);
        Model.SetPlayerSide(PlayerTag);
        View.InitializeView();
        View.UpdateHealth(Model.Health / Model.MaxHealth);

        Model.OnHealthChanged += OnHealthChanged;
        Model.OnDirectionChanged += OnDirectionChanged;

        //敵のプレヤーを取得して記録する
        BindEnemyPlayer();

        Model.ClearTargets();
        Model.SetPlayerInRange(false);

        M_MapPosition mp = _mapManager.ConvertToMapPos(currentPos);
        Vector3 up = _mapManager.ConvertToUnityPos(mp);
        Debug.Log($"mp: {mp.X}/{mp.Y}, up: {up}");
        gameObject.transform.position = up;

        // 位置をマップ座標に変換
        if (_mapManager)
        {
            M_MapPosition start = _mapManager.ConvertToMapPos(transform.position);
            M_MapPosition end = _mapManager.ConvertToMapPos(enemyPos);
            Model.SetEnemyPlayerPos(end);
            Model.SetRoute(C_PathSearch.GetPath(_mapManager.GetAllRoute(), start, end));
        }

        //キャラクタの向きを初期化する
        UpdateDirection(data.MoveDirection);

        _stateMachine.Initialize(new IdleState(Model, this));

        Collider.enabled = true;
        View.EnableAttackRange(true);

        Debug.LogWarning($"Data type is : {Model.GetDataType()}");

    }

    public C_MapManager MapManager { get { return _mapManager; } }
    private void BindEnemyPlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player1");
    //    if(playerObject == null) { Debug.LogError("No player found when binding"); }
        if (playerObject.CompareTag(Model.PlayerSide))
        {
            //    playerObject = GameObject.FindWithTag("Player2");
            playerObject = GameObject.FindGameObjectWithTag("Player2");
            Model?.BindEnemyPlayer(playerObject.GetComponent<C_PlayerTowerController>());
        }
        else
        {
            Model?.BindEnemyPlayer(playerObject.GetComponent<C_PlayerTowerController>());
        }
    }

    public void SetPool(ObjectPoolTest pool)
    {
        Pool = pool;
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
        //    _stateMachine.Initialize(new MoveState(Model, this)); //change to IdleState(Model, this) if needed
        }
        else if (data is ArcherData ad)
        {
            Model = new ArcherModel(ad);
            _stateMachine = new UnitStateMachine();
        //    _stateMachine.Initialize(new MoveState(Model, this)); //change to IdleState(Model, this) if needed
        }
        else if (data is MageData md)
        {
            Model = new MageModel(md);
            _stateMachine = new UnitStateMachine();
        //    _stateMachine.Initialize(new MoveState(Model, this)); //change to IdleState(Model, this) if needed
        }
        else
        {
            Debug.LogError("Unknown data type: " + data.GetType());
            return;
        }

        Model.BindOwner(this);
    }

    #endregion
    //=====================================================================================================

    //=====================================================================================================
    #region 解除 - Release
    public void Release()
    {
        Debug.Log($"Releasing 1 Unit from {Model.PlayerSide}");
        Pool?.Release(this);
    }


    private void OnDisable()
    {
        //    Model.OnHealthChanged -= OnHealthChanged;
        _stateMachine = null;
        Model = null; // clear model to avoid stale state when pooled
    }
    #endregion
    //=====================================================================================================
    public void OnEnterState()
    {
        View.UpdateAttackRangeSpriteColor();
    }
    //=====================================================================================================
    #region 更新 - Update
    // Update is called once per frame
    void Update()
    {
        Model?.Tick(this);
        _stateMachine?.Tick(Time.deltaTime);
     //   ShowRange(true);
    }

    void FixedUpdate()
    {
        _stateMachine?.FixedTick(Time.fixedDeltaTime);
    }
    private void PrepareDeath()
    {
        Collider.enabled = false;
        View.EnableAttackRange(false);
    }
    private void OnHealthChanged(float health, float maxHealth)
    {
        if (Model.IsDead)
        {
            PrepareDeath();
            _stateMachine.TrySetState(new DeadState(Model, this));
            return;
        }

        View.UpdateHealth(health / maxHealth);
    }

    private void OnDirectionChanged(Vector3 direction, Vector3 moveDirection)
    {
        if (direction == Vector3.up)
        {
            View.FaceUP(true);
            View.FaceDOWN(false);
        }
        else if (direction == Vector3.down)
        {
            View.FaceDOWN(true);
            View.FaceUP(false);
        }
        else if (direction == Vector3.left)
        {
            View.FaceDOWN(false);
            View.FaceUP(false);
            FaceLeft(transform);
        }
        else if (direction == Vector3.right)
        {
            View.FaceDOWN(false);
            View.FaceUP(false);
            FaceRight(transform);
        }
        else { Debug.LogError("Unexpected direction"); }
    }
    public void FaceRight(Transform transform)
    {
        transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
    }

    public void FaceLeft(Transform transform)
    {
        transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
    }

    public void UpdateDirection(Vector3 direction)
    {
        Model.SetMoveDirection(direction);
    }
    #endregion
    //=====================================================================================================
    //=====================================================================================================
    #region 攻撃 - attack
    public void TakeDamage(float dmg)
    {
        if(Model.IsDead) return;
        Model?.SetHealth(Model.Health - dmg);
    //    Debug.LogWarning($"Taking {dmg} damage, {Model?.Health} HP remaining.");
    }
    public void SpawnProjectile(GameObject prefab, float speed, float damage)
    {
        // Instantiate, configure velocity and damage here
        View?.PlayAttack();
    }
    //プレヤーに対する攻撃
    public void PerformPlayerAttack()
    {
        if(Model.EnemyPlayer.IsDead()) return;
        View?.StopAttack();
        float damage = Model.AttackPower;
        Model.EnemyPlayer.DecreaseHP(damage);
        View?.PlayAttack();
    }
    //ユニットに対する攻撃
    public void PerformBasicAttack(UnitPresenter target)
    {
        if (target.Model.IsDead) return;
        View?.StopAttack();
        float damage = Model.AttackPower;
        target.TakeDamage(damage);
        View?.PlayAttack();
    }
    public void PerformMagicAttack(float dmg) { /* ... */ View?.PlayAttack(); }
    public void ReceiveHeal(float amount) { /* ... */ View?.PlayHeal(); }
    public void PerformDeath()
    {
        View?.PlayDeath(true);
    }

    #endregion
    //=====================================================================================================
    public void PlayHealVFX() { /* particles */ }

    public bool TryGetLowHpAlly(out UnitPresenter ally)
    {
        ally = null;
        //ally search logic here somewhere maybe
        return false; 
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
    //=====================================================================================================
    #region 範囲 - range
    public bool AllowDetection =>
        false == _stateMachine.Current is DeadState;
    //   _stateMachine.Current is MoveState ||
     //  _stateMachine.Current is IdleState;

    public void OnEnterRange(Collider2D other)
    {
        if (Model.IsDead) return;
        //敵の城でもUnitでもないモノを無視する
        if (other.gameObject.CompareTag(Model.PlayerSide) && other.gameObject.CompareTag("Unit") == false) return; 
     //   Debug.LogError($"PRESENTER : EnterRange trigger with {other.gameObject.name}");

        if (other.gameObject.CompareTag("Unit"))
        {
            bool flowcontrol = HandleUnitTarget(other);
            if (!flowcontrol)
            {
                return;
            }
        }
        else
        {
            bool flowcontrol = HandlePlayerTarget(other);
            if (!flowcontrol)
            {
                return;
            }
        }


        // Tell the FSM that we may need to switch state
        _stateMachine.TrySetState(new AttackState(Model, this));
    }

    private bool HandlePlayerTarget(Collider2D other)
    {
        if(!other.TryGetComponent<C_PlayerTowerController>(out var player)) { return false; }
        if (player.IsDead()) {  return false; }
        Model.SetPlayerInRange(true);
        return true;
    }

    private bool HandleUnitTarget(Collider2D other)
    {
        // Only accept valid UnitPresenters with opposite team
        if (!other.TryGetComponent<UnitPresenter>(out var target)) { /*Debug.LogError("No target found");*/ return false; }
        if (target.Model.PlayerSide == Model.PlayerSide) { /*Debug.LogError("Target invalid : same team");*/ return false; }
        if(target.Model.IsDead) return false;

        Model.AddTarget(target);
    //    Debug.Log("Target added");
        return true;
    }

    public void OnExitRange(Collider2D other)
    {
        if (Model.IsDead) return;
    //    Debug.LogError($"PRESENTER : ExitRange trigger with {other.gameObject.name}");
        //敵のプレヤーが範囲内から離れたら
        if (other.GetComponent<C_PlayerTowerController>() == Model.EnemyPlayer) { Model.SetPlayerInRange(false); }             

        if (!other.TryGetComponent<UnitPresenter>(out var target)) { /*Debug.LogError("No target found");*/ return; }
        if (target.Model.PlayerSide == Model.PlayerSide) { /*Debug.LogError("Target invalid : same team");*/ return; }
        if (target.Model.IsDead)
        {
            Model.RemoveTarget(target);
            return;
        }

        Model.RemoveTarget(target);
        //    Debug.Log("Target removed");

        if (IsValidTargetExist() == false)
            _stateMachine.TrySetState(new IdleState(Model, this));
    }

    public bool IsValidTargetExist()
    {
        if(Model?.Targets.Count == 0 && Model?.IsPlayerInRange == false) return false;
        return true;
    }
    #endregion
    //=====================================================================================================
    //=====================================================================================================
    #region Capsule Collider
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.collider.TryGetComponent<UnitPresenter>(out var target)) { /*Debug.LogError("No target found");*/ return; }
        _stateMachine.TrySetState(new IdleState(Model, this));
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        
    }
    #endregion
}
