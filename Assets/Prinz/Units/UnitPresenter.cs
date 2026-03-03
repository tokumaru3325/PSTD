using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class UnitPresenter : MonoBehaviour
{

    // 2026.01.28 ウー start
    //private UnitModel Model;
    protected UnitModel Model;
    // 2026.01.28 ウー end
    public UnitView View { get; private set; }

    private C_MapManager _mapManager;
    public C_MapManager MapManager { get { return _mapManager; } }

    // 2026.01.28 ウー start 他の形のコライダーも使える
    //public CapsuleCollider2D Collider;
    public Collider2D Collider;
    // 2026.01.28 ウー end

    private UnitStateMachine _stateMachine;

    public UnitObjectPool Pool { get; private set; } //25.1.7 滝本海大 ObjectPoolTestからUnitObjectPoolに変更

    private bool _IsGameEnding = false;
    private bool _IsStateUpdateStopped = false;

    // 2026.02.10 ウー start
    /// <summary>
    /// キャンセルトークン
    /// </summary>
    private CancellationToken _cancelToken;
    // 2026.02.10 ウー end

    //=====================================================================================================
    #region 初期化 - Initialization
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 2026.02.10 ウー start
        _cancelToken = this.GetCancellationTokenOnDestroy();
        // 2026.02.10 ウー end
    }

    private void OnEnable()
    {

    }

    public void Initialize(UnitData data, BuffDataBase buffdata, Vector3 currentPos, Vector3 enemyPos, string PlayerTag, List<C_Buff> buffs, List<UnitPresenter> enemies, PathStrategy strategy)
    {
        View = GetComponent<UnitView>();
        Collider = GetComponent<Collider2D>();
        CreateModelFromData(data);
        Model.SetPlayerSide(PlayerTag);
        Model.SetIsBoss(false);
        View.InitializeView();

        //敵のプレヤーを取得して記録する
        BindEnemyPlayer();

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
            // 2026.01.23 ウー start
            Dictionary<UnitPresenter, M_MapPosition> enemiesPos = GetEnemiesPos(enemies);
            Model.SetEnemyPlayerPos(end);
            //Model.SetRoute(C_PathSearch.GetPath(_mapManager.GetAllRoute(), start, end, Model.MovementStyle));
            Model.SetRoute(C_PathSearch.GetPath(_mapManager.GetAllRoute(), start, end, Model.MovementStyle, enemiesPos, strategy));
            // 2026.01.23 ウー end
        }

        _stateMachine.Initialize(new IdleState(this), _IsGameEnding);

        Collider.enabled = true;
        View.EnableAttackRange(true);

        //キャラクタの向きを初期化する
        SetMoveDirection(data.MoveDirection);

        //    Log($"Data type is : {Model.GetDataType()}", LogType.Warning);

        SubscribeToEvents();

        Model.BindBuffData(buffdata);
        View.UpdateHealth(Model.Health / Model.MaxHealth);
        Model.NotifySpawn();

        // 2026.01.18 ウー start
        UpdateBuffEffect(buffs);
        // 2026.01.18 ウー end

        //test size
        // MakeItBoss(5);
    }

    // 2026.01.23 ウー start
    /// <summary>
    /// すべての敵の座標をゲット
    /// </summary>
    /// <param name="enemies">敵の座標</param>
    private Dictionary<UnitPresenter, M_MapPosition> GetEnemiesPos(List<UnitPresenter> enemies)
    {
        Dictionary<UnitPresenter, M_MapPosition> enemiesPos = new Dictionary<UnitPresenter, M_MapPosition>();
        foreach (UnitPresenter enemy in enemies)
        {
            enemiesPos.Add(enemy, _mapManager.ConvertToMapPos(enemy.transform.position));
        }
        return enemiesPos;
    }
    // 2026.01.23 ウー end

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
    public void BindPool(UnitObjectPool pool)
    {
        Pool = pool;
    }

    protected void CreateModelFromData(UnitData data)
    {
        if (data == null)
        {
            Log("UnitPresenter.Initialize called with null UnitData", LogType.Error);
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
            //    _stateMachine.Initialize(new MoveState(Model, this)); //change to IdleState(this) if needed
        }
        else if (data is ArcherData ad)
        {
            Model = new ArcherModel(ad);
            Model.SetUnitID(UnitID.Archer);
            _stateMachine = new UnitStateMachine();
            //    _stateMachine.Initialize(new MoveState(Model, this)); //change to IdleState(this) if needed
        }
        else if (data is MageData md)
        {
            Model = new MageModel(md);
            Model.SetUnitID(UnitID.Mage);
            _stateMachine = new UnitStateMachine();
            //    _stateMachine.Initialize(new MoveState(Model, this)); //change to IdleState(this) if needed
        }
        else if (data is M_MinerData minerData)
        {
            Model = new M_MinerModel(minerData);
            Model.SetUnitID(UnitID.Miner);
            _stateMachine = new UnitStateMachine();
        }
        else
        {
            Log("Unknown data type: " + data.GetType(), LogType.Error);
            return;
        }


        Model.BindOwner(this);
    }

    //---------------- Events start ----------------//
    private void SubscribeToEvents()
    {
        Model.OnHealthChanged += OnHealthChanged;
        Model.OnDirectionChanged += OnDirectionChanged;
        Model.OnUnitSpawn += OnUnitSpawn;
        UnitModel.OnUnitDeath += UpdateTargets;
        GameManager.GameEnding += OnGameEndingNotify;
    }

    private void UnsubscribeToEvents()
    {
        Model.OnHealthChanged -= OnHealthChanged;
        Model.OnDirectionChanged -= OnDirectionChanged;
        Model.OnUnitSpawn -= OnUnitSpawn;
        UnitModel.OnUnitDeath -= UpdateTargets;
        GameManager.GameEnding -= OnGameEndingNotify;
    }

    private void OnUnitSpawn(UnitPresenter owner)
    {
        DebugManager.Instance.OnUnitSpawn(owner);
        Model.SetSerialNumber(GameManager.Instance.OnUnitSpawn());
        View.RenamePrefab(Model.serialNumber);

        //    Log($"Name set to {gameObject.name}", LogType.Error);
    }

    public void MakeItBoss(float boost)
    {
        SetUnitSize(boost);
        Model.SetIsBoss(true);
        Model.SetAttackPower(Model.AttackPower + boost);
        Model.SetHealth(Model.Health * boost);
        Model.SetMoveSpeed(Model.MoveSpeed / 2);
        Model.SetAttackRange((Model.AttackRange / boost) + (0.25f * boost));
        ApplyAttackRange();
        View.CreateBuffEffect(boost);
    }

    private void ResetSize()
    {
        SetUnitSize(1);
        /*        Log($"Model.AttackPower : {Model.AttackPower}", LogType.Error);
                Log($"Model.Health : {Model.Health}", LogType.Error);
                Log($"Model.MoveSpeed : {Model.MoveSpeed}", LogType.Error);
                Log($"Model.AttackRange : {Model.AttackRange}", LogType.Error);*/

        //    Model.SetAttackPower(Model.AttackPower + boost);
        //    Model.SetHealth(Model.Health * boost);
        //    Model.SetMoveSpeed(Model.MoveSpeed / 2);
        //    Model.SetAttackRange((Model.AttackRange / boost) + (0.25f * boost));
        ApplyAttackRange();
        //    View.CreateBuffEffect(boost);
    }

    private void OnGameEndingNotify(bool ending, string tag)
    {
        Log($"{tag} died and notified this Unit", LogType.Warning);
        if (Model?.PlayerSide != tag)
        {
            Log($"{Model?.PlayerSide} unit wants to go to VictoryState", LogType.Warning);
            float timer = UnityEngine.Random.Range(0.0f, 1.0f);
            // 2026.02.10 ウー start
            //StartCoroutine(DelayVictoryState(timer));
            DelayVictoryState(timer, _cancelToken).Forget();
            // 2026.02.10 ウー end
            return;
        }
        else
        {
            Log($"{Model?.PlayerSide} unit wants to go to DefeatState", LogType.Warning);
            _stateMachine?.TrySetState(new DefeatState(this));
        }
        _IsGameEnding = true;
    }

    private void OnHealthChanged(float health, float maxHealth)
    {
        View.UpdateHealth(health / maxHealth);
        if (IsDead())
        {
            PrepareDeath();
            _stateMachine.TrySetState(new DeadState(this));
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
    //---------------- Events end ----------------//

    #endregion
    //=====================================================================================================
    //=====================================================================================================
    #region 解除 - Release
    public void Release()
    {
        Log($"Releasing 1 Unit from {Model.PlayerSide}", LogType.Log);

        UnsubscribeToEvents();

        Pool?.Release(this);
    }

    private void OnDisable()
    {
        _stateMachine = null;
        Model = null;
    }
    #endregion
    //=====================================================================================================

    //=====================================================================================================
    #region 更新 - Update


    // Update is called once per frame
    void Update()
    {
        Model?.Tick(this);
        if (_IsStateUpdateStopped)
            return;
        _stateMachine?.Tick(Time.deltaTime);
    }

    void FixedUpdate()
    {
        if (_IsGameEnding) return;
        if (_IsStateUpdateStopped)
            return;
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
        DebugManager.Instance.Log(message, type);
    }

    private void PrepareDeath()
    {
        //   View.UpdateHealth(Model.Health / Model.MaxHealth);
        View.ResetBuffEffect();
        Model.NotifyUnitDeath();
        ResetSize();
        Model.ClearTargets();
        Collider.enabled = false;
        View.EnableAttackRange(false);
    }

    public void FaceRight()
    {
        transform.localScale = new Vector3(-1 * Model.Size, transform.localScale.y, transform.localScale.z);
    }

    public void FaceLeft()
    {
        transform.localScale = new Vector3(1 * Model.Size, transform.localScale.y, transform.localScale.z);
    }
    public void OnEnterState(IUnitState EnteringState)
    {
        if (EnteringState == null) return;

        if (_IsGameEnding)
        {

        }

        if (EnteringState is IdleState)
        {
            //   View.ResetAllAnimations();
        }
        View.ResetAllAnimations();
        SetMoveDirection(Model.MoveDirection);
        View.UpdateAttackRangeSpriteColor();
    }

    public void FreezeState(bool stop)
    {
        _IsStateUpdateStopped = stop;
    }

    private IEnumerator DelayVictoryState(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);

            _stateMachine?.TrySetState(new VictoryState(this));
        }
    }
    // 2026.02.10 ウー start
    /// <summary>
    /// 勝利状態への遅延
    /// </summary>
    /// <param name="delay">遅延時間（秒）</param>
    /// <param name="token">キャンセルトークン</param>
    private async UniTaskVoid DelayVictoryState(float delay, CancellationToken token)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: token);
        _stateMachine?.TrySetState(new VictoryState(this));
    }
    // 2026.02.10 ウー end
    #endregion
    //=====================================================================================================
    //=====================================================================================================
    #region 攻撃 - attack
    public void TakeDamage(float dmg)
    {
        if (_IsGameEnding) return;
        Model?.SetHealth(Model.Health - dmg);
        //    Debug.LogWarning($"Taking {dmg} damage, {Model?.Health} HP remaining.");
    }

    /// <summary>
    /// プレヤーに対する攻撃
    /// </summary>
    /// <param name="dt"></param>
    public void PerformPlayerAttack(float dt)
    {
        if (_IsGameEnding) return;
        if (Model.EnemyPlayer.IsDead()) return;
        Model.PlayerAttack(dt);
    }

    /// <summary>
    /// ユニットに対する攻撃
    /// </summary>
    /// <param name="target"></param>
    /// <param name="dt"></param>
    public void PerformBasicAttack(UnitPresenter target, float dt)
    {
        if (_IsGameEnding) return;
        Model.BasicAttack(target, dt);
    }

    public void ShootArrow(Vector3 targetPos)
    {
        ProjectileObjectPool.Instance.GetObj(ProjectileType.Arrow, transform.position, targetPos);
    }

    public void AttackSpell(Vector3 targetPos)
    {
        ProjectileObjectPool.Instance.GetObj(ProjectileType.AttackSpell, transform.position, targetPos);
    }

    public void HealSpell(Vector3 targetPos)
    {
        ProjectileObjectPool.Instance.GetObj(ProjectileType.HealSpell, transform.position, targetPos);
    }

    public void PerformHealSpell(UnitPresenter target, float dt)
    {
        Model.Heal(target, dt);
    }

    public void ReceiveHeal(float amount)
    {
        Model?.SetHealth(Model.Health + amount);
        if (View == null) return;
        View.ResetAllAnimations();
        View.PlayAttack();
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
    //=====================================================================================================
    #region Access Model

    public void SetUnitSize(float amount)
    {
        Model?.SetSize(amount);
        transform.localScale = new Vector3(amount, amount, amount);
    }

    public void SetMoveDirection(Vector3 direction)
    {
        Model.SetMoveDirection(direction);
    }

    public bool IsDead()
    {
        return Model.IsDead;
    }

    public UnitData GetDataType()
    {
        return Model?.GetDataType();
    }

    public bool IsBadlyWounded()
    {
        return Model.IsBadlyWounded;
    }

    public bool IsWounded()
    {
        return Model.IsWounded;
    }

    public string GetPlayerSide()
    {
        return Model.PlayerSide;
    }

    public float GetTotalMoveSpeed()
    {
        return Model.TotalMoveSpeed;
    }

    public Vector3 GetMoveDirection()
    {
        return Model.MoveDirection;
    }

    //    public void SetMoveDirection

    public int GetCurrentRouteIndex()
    {
        return Model.CurrentRouteIndex;
    }

    public int GetRouteCount()
    {
        return Model.Route.Count;
    }

    public M_MapPosition GetRoutePosition(int pos)
    {
        return Model.Route[pos];
    }

    public void SetCurrentRouteIndex(int index)
    {
        Model?.SetCurrentRouteIndex(index);
    }

    public bool IsPlayerInRange()
    {
        return Model.IsPlayerInRange;
    }

    public bool HasTargetInRange()
    {
        return Model.HasTargetInRange();
    }

    public UnitPresenter GetPrimaryTarget()
    {
        return Model?.GetPrimaryTarget();
    }

    public int GetSerialNumber()
    {
        return Model.serialNumber;
    }

    #endregion
    //=====================================================================================================
    //=====================================================================================================
    #region Access View

    public void PlayAttack()
    {
        View?.PlayAttack();

    }

    public void StopAttack()
    {
        View?.StopAttack();
    }

    public void TriggerIdle()
    {
        View?.TriggerIdle();
    }

    public void PlayMove(bool ismoving)
    {
        View?.PlayMove(ismoving);
    }
    #endregion
    //=====================================================================================================
    public void PlayHealVFX() { /* particles */ }

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
        false == _stateMachine.Current is DeadState || _IsGameEnding;

    //   _stateMachine.Current is MoveState ||
    //  _stateMachine.Current is IdleState;

    public bool IsSameTeamAs(UnitPresenter target)
    {
        return target?.Model?.PlayerSide == Model.PlayerSide;
    }


    public void OnEnterRange(Collider2D other)
    {
        if (_IsGameEnding) return;
        if (IsDead()) return;

        //敵の城でもUnitでもないモノを無視する
        // 2026.01.28 ウー start
        //if (other.gameObject.CompareTag(Model.PlayerSide) && other.gameObject.CompareTag("Unit") == false) return;
        if (IsUntargetable(other.gameObject)) return;
        // 2026.01.28 ウー end
        Log($"PRESENTER : EnterRange trigger with {other.gameObject.name}", LogType.Warning);

        bool flowControl = HandleTarget(other);
        if (!flowControl)
        {
            return;
        }

        if (Model.GetPrimaryTarget() != null)
        {
            _stateMachine.TrySetState(new IdleState(this));
        }
    }

    // 2026.01.28 ウー start
    /// <summary>
    /// 非攻撃対象ですか
    /// </summary>
    /// <param name="obj">対象</param>
    /// <returns>true: 非攻撃対象です, false: 攻撃対象です</returns>
    protected virtual bool IsUntargetable(GameObject obj)
    {
        //敵の城でもUnitでもないモノを無視する
        return obj.CompareTag(Model.PlayerSide) && obj.CompareTag("Unit") == false;
    }

    /// <summary>
    /// 対象を処理
    /// </summary>
    /// <param name="other">対象</param>
    /// <returns>true: 完了, false: 中断した</returns>
    protected virtual bool HandleTarget(Collider2D other)
    {
        // 対象がユニットの場合
        if (other.gameObject.CompareTag("Unit"))
        {
            bool flowcontrol = HandleUnitTarget(other);
            if (!flowcontrol)
            {
                return false;
            }
        }
        // 対象が城の場合
        else
        {
            bool flowcontrol = HandlePlayerTarget(other);
            if (!flowcontrol)
            {
                return false;
            }
        }

        return true;
    }
    // 2026.01.28 ウー end

    private bool HandleUnitTarget(Collider2D other)
    {
        if (!other.TryGetComponent<UnitPresenter>(out var target)) { /*Debug.LogError("No target found");*/ return false; }
        if (Model.UnitID == UnitID.Archer || Model.UnitID == UnitID.Knight)
        {
            if (IsSameTeamAs(target)) { /*Debug.LogError("Target invalid : same team");*/ return false; }
        }
        if (target.IsDead()) return false;

        if (IsSameTeamAs(target) && false == target.IsWounded())
        {
            return false;
        }
        Model.AddTarget(target);
        Log("Target added", LogType.Log);
        return true;
    }
    private bool HandlePlayerTarget(Collider2D other)
    {
        if (!other.TryGetComponent<C_PlayerTowerController>(out var player)) { return false; }
        if (player.IsDead()) { return false; }
        Model.SetPlayerInRange(true);
        return true;
    }

    private void UpdateTargets(UnitPresenter target)
    {
        if (_IsGameEnding) return;
        //    Log($"Unit from {Model?.PlayerSide} tries to update targets", LogType.Warning);
        if (Model?.FindTarget(target) != null)
        {
            Log($"Unit from {Model.PlayerSide} updates targets and removes a Unit from {target.Model.PlayerSide}", LogType.Warning);
            Model.RemoveTarget(target);
        }
    }

    public void OnExitRange(Collider2D other)
    {
        if (_IsGameEnding) return;

        //    Debug.LogError($"PRESENTER : ExitRange trigger with {other.gameObject.name}");
        //敵のプレヤーが範囲内から離れたら
        if (other.GetComponent<C_PlayerTowerController>() == Model.EnemyPlayer)
        {
            Model.SetPlayerInRange(false);
        }

        // 2026.01.29 ウー start
        if (other.GetComponent<C_ObstacleStone>() && other.GetComponent<C_ObstacleStone>() == Model.Obstacle)
        {
            Model.SetPlayerInRange(false);
            Model.BindTargetObstacle(null);
        }
        // 2026.01.29 ウー end

        if (!other.TryGetComponent<UnitPresenter>(out var target)) { /*Debug.LogError("No target found");*/ return; }
        if (Model.UnitID == UnitID.Archer || Model.UnitID == UnitID.Knight)
        {
            if (IsSameTeamAs(target)) { /*Debug.LogError("Target invalid : same team");*/ return; }
        }
        if (target.IsDead())
        {
            Model.RemoveTarget(target);
            return;
        }

        Model.RemoveTarget(target);
        //    Debug.Log("Target removed");

        if (IsValidTargetExist() == false)
            _stateMachine.TrySetState(new IdleState(this));
    }

    public bool IsValidTargetExist()
    {
        if (Model?.Targets.Count == 0 && Model?.IsPlayerInRange == false) return false;
        return true;
    }
    #endregion
    //=====================================================================================================
    //=====================================================================================================
    #region Capsule Collider
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.collider.TryGetComponent<UnitPresenter>(out var target)) { /*Debug.LogError("No target found");*/ return; }
        _stateMachine.TrySetState(new IdleState(this));
    }

    private void OnCollisionExit2D(Collision2D collision)
    {

    }
    #endregion

    // 2026.01.18 ウー start
    #region Buff Effect

    /// <summary>
    /// ユニットと同じタグですか
    /// </summary>
    /// <param name="tag">タグ</param>
    /// <returns>true: はい、false: いいえ</returns>
    public bool CompareWithTag(string tag)
    {
        return Model.PlayerSide == tag;
    }

    /// <summary>
    /// バフのエフェクトを更新
    /// </summary>
    /// <param name="buffs">バフ</param>
    public void UpdateBuffEffect(List<C_Buff> buffs)
    {
        View.UpdateBuffEffect(buffs);
    }

    #endregion
    // 2026.01.18 ウー end

    // 2026.01.23 ウー start
    /// <summary>
    /// ユニットの危険度をゲット
    /// </summary>
    /// <returns>危険度</returns>
    public int GetDangerLevel()
    {
        return Model.DangerLevel;
    }
    // 2026.01.23 ウー end

    // 2026.02.10 ウー start
    /// <summary>
    /// ボスユニットかどうかを返す
    /// </summary>
    /// <returns>true: ボスユニット、false: それ以外</returns>
    public bool IsBoss()
    {
        return Model.IsBoss;
    }
    // 2026.02.10 ウー end
}
