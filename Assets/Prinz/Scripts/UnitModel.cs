using UnityEngine;

public class UnitModel
{
    public int Health {  get; private set; }
    public int AttackPower { get; private set; }
    public float AttackSpeed { get; private set; }
    public float MovementSpeed { get; private set; }

    /// <summary>
    /// プレイヤー1のユニットだったら1、プレイヤー2のユニットだったら2
    /// </summary>
    public int PlayerSide { get; private set; }
}
