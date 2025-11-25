using UnityEngine;

[CreateAssetMenu(fileName = "UnitData", menuName = "Scriptable Objects/UnitData")]
public abstract class UnitData : ScriptableObject
{
    
    /// <summary>
    /// プレイヤー1のユニットだったら1、プレイヤー2のユニットだったら2
    /// </summary>
    public int      PlayerSide;

    public float    MaxHealth;
    public int      AttackPower;
    public float    AttackSpeed;
    public float    MoveSpeed;

    public int      UnitCost;
    public float    UnitCoolDown;
    public Vector3  MoveDirection;

    //

}
