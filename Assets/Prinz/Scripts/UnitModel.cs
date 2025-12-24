using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum UnitID
{
    Knight,
    Archer,
    Mage
}
public abstract class UnitModel
{
    protected UnitData Data;
    protected BuffDataBase BuffData;
    public void BindBuffData(BuffDataBase data)
    {
        BuffData = data;
    }

    public UnitID UnitID {  get; protected set; }

    public void SetUnitID(UnitID unitID)
    {
        this.UnitID = unitID;
    }

    public UnitModel(UnitData data)
    {
        Data = data;
        MaxHealth = data.MaxHealth;
        Health = data.MaxHealth;
        AttackPower = data.BaseAttackPower;
        AttackSpeed = data.BaseAttackSpeed;
        AttackRange = data.BaseAttackRange;
        MoveSpeed = data.BaseMoveSpeed;
        UnitCost = data.BaseUnitCost;
        UnitCoolDown = data.BaseUnitCoolDown;
        MoveDirection = data.MoveDirection;
    }

    // プレイヤー1のユニットだったら1、プレイヤー2のユニットだったら2
    public string PlayerSide { get; private set; }
    public float MaxHealth { get; private set; }
    public float Health { get; private set; }
    public float AttackPower { get; private set; }
    public float TotalAttackPower => AttackPower + BuffData.AttackPower;
    public float AttackSpeed { get; private set; }
    public float TotalAttackSpeed => AttackSpeed + BuffData.AttackSpeed;
    public float MoveSpeed { get; private set; }
    public float TotalMoveSpeed => MoveSpeed + BuffData.MoveSpeed;
    public int UnitCost { get; private set; }
    public float UnitCoolDown { get; private set; }
    public Vector3 MoveDirection { get; private set; }
    public M_MapPosition EnemyPlayerPos { get; private set; }
    public List<M_MapPosition> Route { get; private set; }
    public int CurrentRouteIndex { get; private set; }
    public bool IsDead => Health <= 0f;
    /// <summary>
    /// HPが50%以下だったらTrue
    /// </summary>
    public bool IsBadlyWounded => Health < MaxHealth / 2.0f;
    /// <summary>
    /// HPが100%以下だったらTrue
    /// </summary>
    public bool IsWounded => Health < MaxHealth;
    public bool IsPlayerInRange { get; private set; }

    public event Action<float, float> OnHealthChanged;

    public event Action<Vector3, Vector3> OnDirectionChanged;

    public event Action<UnitPresenter> OnUnitSpawn;
    public abstract void Tick(UnitPresenter presenter);

    public abstract void BasicAttack(UnitPresenter presenter, float dt);

    public virtual void Heal(UnitPresenter presenter, float dt)
    {
        Owner.Log("This Unit is not supposed to Heal", LogType.Error);
    }
    public abstract void PlayerAttack(float dt);
    public bool CanAttack {  get; private set; }

    public void AllowAttack(bool canattack)
    {
        CanAttack = canattack;
    }

    public UnitData GetDataType()
    {
        return Data;
    }

    //owner
    public UnitPresenter Owner { get; private set; }

    public void BindOwner(UnitPresenter presenter)
    {
        Owner = presenter;
    }

    public C_PlayerTowerController EnemyPlayer { get; private set; }

    public void BindEnemyPlayer(C_PlayerTowerController enemyPlayer)
    {
        EnemyPlayer = enemyPlayer;
    }

    public void NotifySpawn()
    {
        OnUnitSpawn?.Invoke(Owner);
    }

    //=====================================================================================================
    #region Range 
    public float AttackRange { get; private set; }
    public float TotalAttackRange => AttackRange + BuffData.AttackRange;
/*    public float BaseAttackRange { get; private set; }
    public float RangeMultiplier { get; private set; } = 1.0f;
    public float CurrentRange => BaseAttackRange * RangeMultiplier;
*/
/*    public void SetRangeBuff(float factor)
    {
        RangeMultiplier = factor;
    }*/
    #endregion
    //=====================================================================================================
    //=====================================================================================================
    #region Targets
    protected readonly List<UnitPresenter> targets = new();

    public void AddTarget(UnitPresenter t)
    {
        if (!targets.Contains(t))
            targets.Add(t);
        Owner.Log($"Added target : {t}", LogType.Warning);
    }

    public void RemoveTarget(UnitPresenter t)
    {
        targets.Remove(t);
        Owner.Log($"Removed target : {t}", LogType.Warning);
    }

    public void ClearTargets()
    {
        targets.Clear();
    }

    public List<UnitPresenter> Targets => targets;

    public bool HasTargetInRange()
    {
        if (targets.Count == 0) return false;

        return true;
    }

    public void SetPlayerInRange(bool isPlayerInRange)
    {
        IsPlayerInRange = isPlayerInRange;
    }

    public virtual UnitPresenter GetPrimaryTarget()
    {
        if (targets.Count == 0) return null;
        foreach (var t in targets)
        {
            if (t != null)
            {
                if(t.Model?.IsDead == false) return t;
            }
        }
        return null;
    }
    #endregion
    //=====================================================================================================
    //=====================================================================================================
    #region Setter
    public void SetHealth(float amount)
    {
        Health = Mathf.Clamp(amount, 0.0f, MaxHealth);
        OnHealthChanged?.Invoke(Health, MaxHealth);
    }

    public void SetPlayerSide(string playerSide)
    {
        PlayerSide = playerSide;
    }

    public void SetMaxHealth(float amount)
    {
        MaxHealth = Mathf.Max(amount, 1.0f);
    }

    public void SetAttackPower(float amount)
    {
        AttackPower = amount;
    }

    public void SetAttackSpeed(float amount)
    {
        AttackSpeed = Mathf.Max(amount, 0.5f);
    }

    public void SetMoveSpeed(float amount)
    {
        MoveSpeed = Mathf.Max(amount, 0.0f);
    }

    public void SetUnitCost(int amount)
    {
        UnitCost = (int)MathF.Max(amount, 0);
    }

    public void SetUnitCoolDown(float amount)
    {
        UnitCoolDown = MathF.Max(amount, 0);
    }

    public void SetMoveDirection(Vector3 direction)
    {
        if (MoveDirection != direction)
        {
            MoveDirection = direction;
            OnDirectionChanged?.Invoke(direction, MoveDirection);
            Debug.Log($"MoveDirection: {MoveDirection}");
        }
    }

    public void SetCurrentRouteIndex(int ri)
    {
        CurrentRouteIndex = ri;
    }
    public void SetEnemyPlayerPos(M_MapPosition pos)
    {
        EnemyPlayerPos = pos;
    }
    public void SetRoute(List<M_MapPosition> route)
    {
        Route = route;
    }
    #endregion
    //=====================================================================================================
}
