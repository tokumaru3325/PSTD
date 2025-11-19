using System;
using UnityEngine;

//[Serializable]
public class UnitModel
{
    /// <summary>
    /// プレイヤー1のユニットだったら1、プレイヤー2のユニットだったら2
    /// </summary>
    public int      PlayerSide { get;  set; }
    public float    MaxHealth { get; set; }
    public float    Health { get; private set; }
    public float    AttackPower { get;  set; }
    public float    AttackSpeed { get;  set; }
    public float    MoveSpeed { get;  set; }
    public int      UnitCost { get;  set; }
    public float    UnitCoolDown { get;  set; }
    public Vector3  MoveDirection { get;  set; } //test


    public event Action<float, float> OnHealthChanged;



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
