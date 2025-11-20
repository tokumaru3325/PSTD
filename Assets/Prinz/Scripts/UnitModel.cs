using System;
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
        AttackPower = data.AttackPower;
        AttackSpeed = data.AttackSpeed;
        MoveSpeed = data.MoveSpeed;
        UnitCost = data.UnitCost;
        UnitCoolDown = data.UnitCoolDown;
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

    public bool IsDead => Health <= 0f;

    public event Action<float, float> OnHealthChanged;


    public abstract void Tick(UnitPresenter presenter);


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
        MoveDirection = direction;
    }
#endregion
}
