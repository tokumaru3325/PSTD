using System;
using UnityEngine;

public class UnitModel
{
    public int Health { get; private set; }
    public int AttackPower { get; private set; }
    public float AttackSpeed { get; private set; }
    public float MoveSpeed { get; private set; }

    public Vector3 MoveDirection { get; set; }

    /// <summary>
    /// プレイヤー1のユニットだったら1、プレイヤー2のユニットだったら2
    /// </summary>
    public int PlayerSide { get; private set; }

    public event Action OnHealthChanged;

    public void TakeDamage(int amount)
    {
        Health -= amount;
        OnHealthChanged?.Invoke();
    }

}
