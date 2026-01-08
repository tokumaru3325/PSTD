using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


public class UnitPresenter: MonoBehaviour
{

    public UnitModel Model { get; private set; }
    public UnitView View { get; private set; }

//    [SerializeField]
//    public BuffData BuffData;

    private C_MapManager _mapManager;

    public CapsuleCollider2D Collider;

    private UnitStateMachine _stateMachine;

    [SerializeField]
    private DebugManager _debugManager;  //make it to singleton

    public UnitObjectPool Pool { get; private set; } //25.1.7 滝本海大 ObjectPoolTestからUnitObjectPoolに変更


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
    public void Initialize(UnitData data, BuffDataBase buffdata, Vector3 currentPos, Vector3 enemyPos, string PlayerTag)
    {
        View = GetComponent<UnitView>();
        Collider = GetComponent<CapsuleCollider2D>();
        CreateModelFromData(data);
        Model.SetPlayerSide(PlayerTag);
        View.InitializeView();
        View.UpdateHealth(Model.Health / Model.MaxHealth);
        BindDebugManager();

        Model.OnHealthChanged += OnHealthChanged;
        Model.OnDirectionChanged += OnDirectionChanged;
        Model.OnUnitSpawn += OnUnitSpawn;


        //敵のプレヤーを取得して記録する
        BindEnemyPlayer();
        M_Tower.OnPlayerDeath += OnPlayerDeathNotify;

        Model.ClearTargets();
        Model.SetPlayerInRange(false);

        M_MapPosition mp = _mapManager.ConvertToMapPos(currentPos);
        Vector3 up = _mapManager.ConvertToUnityPos(mp);
        Log($"mp: {mp.X}/{mp.Y}, up: {up}", LogType.Log);
    //    Debug.Log($"mp: {mp.X}/{mp.Y}, up: {up}");
        gameObject.transform.position = up;

        // 位置をマップ座標に変換
        if (_mapManager)
        {
            M_MapPosition start = _mapManager.ConvertToMapPos(transform.position);
            M_MapPosition end = _mapManager.ConvertToMapPos(enemyPos);
            Model.SetEnemyPlayerPos(end);
            Model.SetRoute(C_PathSearch.GetPath(_mapManager.GetAllRoute(), start, end));
        }

        _stateMachine.Initialize(new IdleState(Model, this));

        Collider.enabled = true;
        View.EnableAttackRange(true);

        //キャラクタの向きを初期化する
        UpdateDirection(data.MoveDirection);

        Log($"Data type is : {Model.GetDataType()}", LogType.Warning);
        //   Debug.LogWarning($"Data type is : {Model.GetDataType()}");

        Model.BindBuffData(buffdata);

        Model.NotifySpawn();
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

    public void SetPool(UnitObjectPool pool)
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
            Model.SetUnitID(UnitID.Knight);
            _stateMachine = new UnitStateMachine();
        //    _stateMachine.Initialize(new MoveState(Model, this)); //change to IdleState(Model, this) if needed
        }
        else if (data is ArcherData ad)
        {
            Model = new ArcherModel(ad);
            Model.SetUnitID(UnitID.Archer);
            _stateMachine = new UnitStateMachine();
        //    _stateMachine.Initialize(new MoveState(Model, this)); //change to IdleState(Model, this) if needed
        }
        else if (data is MageData md)
        {
            Model = new MageModel(md);
            Model.SetUnitID(UnitID.Mage);
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

    private void BindDebugManager()
    {
        GameObject dm = GameObject.FindGameObjectWithTag("DebugManager");
        if (dm != null)
        {
            _debugManager = dm.GetComponent<DebugManager>();
        }
    }

    private void OnUnitSpawn(UnitPresenter owner)
    {
     //   DebugManager.OnUnitSpawn(owner);
        _debugManager.OnUnitSpawn(owner);
    }

    private void OnPlayerDeathNotify(string tag)
    {
        Log($"{tag} died and notified this Unit", LogType.Warning);
        if (Model?.PlayerSide != tag)
        {
            Log($"{Model?.PlayerSide} unit wants to go to VictoryState", LogType.Warning);
            _stateMachine?.TrySetState(new VictoryState(Model, this));
            return;
        }
        else
        {
            Log($"{Model?.PlayerSide} unit wants to go to DefeatState", LogType.Warning);
            _stateMachine?.TrySetState(new DefeatState(Model, this));
        }
        _IsGameFinished = true;
    }

    #endregion
    //=====================================================================================================

    //=====================================================================================================
    #region 解除 - Release
    public void Release()
    {
        Debug.Log($"Releasing 1 Unit from {Model.PlayerSide}");
        Model.OnHealthChanged -= OnHealthChanged;
        Model.OnDirectionChanged -= OnDirectionChanged;
        //    Model.OnUnitSpawn -= OnUnitSpawn;

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
    public void OnEnterState(IUnitState EnteringState)
    {
        if (EnteringState == null) return;

        if(_IsGameFinished)
        {

        }

        if(EnteringState is DeadState)
        {

        }

        UpdateDirection(Model.MoveDirection);
        View.UpdateAttackRangeSpriteColor();
    }
    //=====================================================================================================
    #region 更新 - Update
    private bool _IsGameFinished = false;

    // Update is called once per frame
    void Update()
    {
    //    if(_IsGameFinished) return;
        Model?.Tick(this);
        _stateMachine?.Tick(Time.deltaTime);
     //   ShowRange(true);
    }

    void FixedUpdate()
    {
        if (_IsGameFinished) return;
        _stateMachine?.FixedTick(Time.fixedDeltaTime);
    }

    /// <summary>
    /// この関数を使って、DebugLogの表示をDebugManagerのInspector上で設定することが出来る
    /// 使い方：Log("「○○メッセージ」", 「LogType.Log、LogType.Warning、LogType.Errorのいずれを選ぶ」);
    /// </summary>
    /// <param name="message"></param>
    /// <param name="type"></param>
    public void Log(string message, LogType type)
    {
        _debugManager.Log(message, type);
    }

    private void PrepareDeath()
    {
     //   View.UpdateHealth(Model.Health / Model.MaxHealth);
        Model.ClearTargets();
        Collider.enabled = false;
        View.EnableAttackRange(false);
    }
    private void OnHealthChanged(float health, float maxHealth)
    {
        View.UpdateHealth(health / maxHealth);
        if (Model.IsDead)
        {
            PrepareDeath();
            _stateMachine.TrySetState(new DeadState(Model, this));
        }
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
            FaceLeft();
        }
        else if (direction == Vector3.right)
        {
            View.FaceDOWN(false);
            View.FaceUP(false);
            FaceRight();
        }
        else { Log("Unexpected direction", LogType.Error); }
    }
    public void FaceRight()
    {
        transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
    }

    public void FaceLeft()
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
    //    if(Model.IsDead) return;
        Model?.SetHealth(Model.Health - dmg);
    //    Debug.LogWarning($"Taking {dmg} damage, {Model?.Health} HP remaining.");
    }
    public void SpawnProjectile(GameObject prefab, float speed, float damage)
    {
        // Instantiate, configure velocity and damage here
        View?.PlayAttack();
    }
    //プレヤーに対する攻撃
    public void PerformPlayerAttack(float dt)
    {
        if(Model.EnemyPlayer.IsDead()) return;
        Model.PlayerAttack(dt);
    }
    //ユニットに対する攻撃
    public void PerformBasicAttack(UnitPresenter target, float dt)
    {
     //   if (target.Model.IsDead) return;
        Model.BasicAttack(target, dt); //moving logic to model

/*        View?.StopAttack();
        float damage = Model.AttackPower;
        target.TakeDamage(damage);
        View?.PlayAttack();*/
    }

    public void PerformHealSpell(UnitPresenter target, float dt)
    {
        Model.Heal(target, dt);
    }
    public void PerformMagicAttack(float dmg) { /* ... */ View?.PlayAttack(); }
    public void ReceiveHeal(float amount)
    {
        Model?.SetHealth(Model.Health + amount);
        if (View == null) return;
        View.ResetAllAnimations();
        View.PlayAttack();
    //    View?.PlayHeal();
    }
    public void PerformDeath()
    {
        if (View == null) return;
        View.ResetAllAnimations();
        View.PlayDeath(true);
    }

    public void PerformVictoryAnimation()
    {
        if (View == null) return;
        View.ResetAllAnimations();
        View.PlayVictoryDance(true);
    }

    public void PerformDefeatAnimation()
    {
        if (View == null) return;
        View.ResetAllAnimations();
        View.PlayDefeatAnimation(true);
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
            Log("AttackRangeTransform not found", LogType.Error);
            return;
        }
        float newRange = Model.TotalAttackRange;

        // Adjust collider width AND sprite size by scaling the child object
        Vector3 scale = View.AttackRangeTransform.localScale;
        scale.x = newRange;     // grow horizontally
        scale.y = newRange;     // grow vertically
        View.AttackRangeTransform.localScale = scale;
    }

/*    public void SetRangeBuff(float factor)
    {
        Model?.SetRangeBuff(factor);
        ApplyAttackRange();
    }*/
    //=====================================================================================================
    #region 範囲 - range
    public bool AllowDetection =>
        false == _stateMachine.Current is DeadState;
    //   _stateMachine.Current is MoveState ||
    //  _stateMachine.Current is IdleState;

    public bool IsSameTeamAs(UnitPresenter target)
    {
        return target?.Model?.PlayerSide == Model.PlayerSide;
    }


    public void OnEnterRange(Collider2D other)
    {
        if (Model.IsDead) return;

        //敵の城でもUnitでもないモノを無視する
        if (other.gameObject.CompareTag(Model.PlayerSide) && other.gameObject.CompareTag("Unit") == false) return;
        Log($"PRESENTER : EnterRange trigger with {other.gameObject.name}", LogType.Warning);

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

        if (Model.GetPrimaryTarget() != null)
        {
            _stateMachine.TrySetState(new IdleState(Model, this));
        }
    }


    private bool HandleUnitTarget(Collider2D other)
    {
        if (!other.TryGetComponent<UnitPresenter>(out var target)) { /*Debug.LogError("No target found");*/ return false; }
        if (Model.UnitID == UnitID.Archer || Model.UnitID == UnitID.Knight)
        {
            if (IsSameTeamAs(target)) { /*Debug.LogError("Target invalid : same team");*/ return false; }
        }
        if (target.Model.IsDead) return false;

        if (IsSameTeamAs(target) && false == target.Model.IsWounded)
        {
            return false;
        }
        Model.AddTarget(target);
        Log("Target added", LogType.Log);
        return true;
    }
    private bool HandlePlayerTarget(Collider2D other)
    {
        if(!other.TryGetComponent<C_PlayerTowerController>(out var player)) { return false; }
        if (player.IsDead()) {  return false; }
        Model.SetPlayerInRange(true);
        return true;
    }

    public void OnExitRange(Collider2D other)
    {
    //    if (Model.IsDead) return;
        //    Debug.LogError($"PRESENTER : ExitRange trigger with {other.gameObject.name}");
        //敵のプレヤーが範囲内から離れたら
        if (other.GetComponent<C_PlayerTowerController>() == Model.EnemyPlayer)
        {
            Model.SetPlayerInRange(false);
        }             

        if (!other.TryGetComponent<UnitPresenter>(out var target)) { /*Debug.LogError("No target found");*/ return; }
        if (Model.UnitID == UnitID.Archer || Model.UnitID == UnitID.Knight)
        {
            if (IsSameTeamAs(target)) { /*Debug.LogError("Target invalid : same team");*/ return; }
        }
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
