using UnityEngine;

[CreateAssetMenu(fileName = "UnitData", menuName = "Scriptable Objects/UnitData")]
public abstract class UnitData : ScriptableObject
{

    /// <summary>
    /// プレイヤー1のユニットだったら1、プレイヤー2のユニットだったら2
    /// </summary>
    //    public int      PlayerSide;

    public float MaxHealth;
    public float BaseAttackPower;
    public float BaseAttackSpeed;
    public float BaseAttackRange;
    public float BaseMoveSpeed;

    public int BaseUnitCost;
    public float BaseUnitCoolDown;
    public Vector3 MoveDirection;

    // 2026.01.18 ウー start
    /// <summary>
    /// 危険度
    /// </summary>
    public int DangerLevel;
    // 2026.01.18 ウー end
}
