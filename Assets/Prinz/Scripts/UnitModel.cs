using System;
using System.Collections.Generic;
using UnityEngine;

//[Serializable]
public abstract class UnitModel
{
    protected UnitData Data;

    public UnitModel(UnitData data)
    {
        Data = data;
        PlayerSide = data.PlayerSide;
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
    public int      PlayerSide { get; private set; }

    public float    MaxHealth { get; private set; }
    public float    Health { get; private set; }
    public float    AttackPower { get; private set; }
    public float    AttackSpeed { get; private set; }

    public float    MoveSpeed { get; private set; }
    public int      UnitCost { get; private set; }
    public float    UnitCoolDown { get; private set; }
    public Vector3  MoveDirection { get; private set; }

    public M_MapPosition EnemyPlayerPos { get; private set; }

    public List<M_MapPosition> Route { get; private set; }

    public bool IsDead => Health <= 0f;

    public event Action<float, float> OnHealthChanged;


    #region Range 
    public float    AttackRange { get; private set; }
    public float    BaseAttackRange { get; private set; }
    public float    RangeMultiplier { get; private set; } = 1.0f;
    public float CurrentRange => BaseAttackRange * RangeMultiplier;

    public void SetRangeBuff(float factor)
    {
        RangeMultiplier = factor;
    }
    #endregion

    //owner
    public UnitPresenter Owner { get; private set; }

    public void Bind(UnitPresenter presenter)
    {
        Owner = presenter;
    }

    //target enemy

/*    public UnitPresenter TargetEnemy { get; private set; }

    public void SetTarget(UnitPresenter enemy)
    {
        TargetEnemy = enemy;
    }*/

    private readonly List<UnitPresenter> targets = new();

    public void AddTarget(UnitPresenter t)
    {
        if (!targets.Contains(t))
            targets.Add(t);
    }

    public List<UnitPresenter> Targets => targets;

    public abstract void Tick(UnitPresenter presenter);

    public bool HasEnemyInRange()
    {
        if(targets.Count == 0) return false;

        return true;
    }

    public UnitPresenter ClosestEnemy()
    {
       // targets.ForEach

        return null;
    }

    public void SetEnemyPolayerPos(M_MapPosition pos)
    {
        EnemyPlayerPos = pos;
    }

    public void SetRoute(List<M_MapPosition> route)
    {
        Route = route;
    }

#region Setter
    public void SetHealth(float amount)
    {
        Health = Mathf.Clamp(amount, 0.0f, MaxHealth);
        OnHealthChanged?.Invoke(Health, MaxHealth);
    }

    public void SetPlayerSide(int playerSide)
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

    public void SetMoveDirection (Vector3 direction)
    {
        if (MoveDirection != direction)
        {
            MoveDirection = direction;
            Debug.Log($"MoveDirection: {MoveDirection}");
        }
    }
#endregion
}
