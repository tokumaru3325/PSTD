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
    public float BaseSize;

    public int BaseUnitCost;
    public float BaseUnitCoolDown;
    public Vector3 MoveDirection;

    // 2026.01.18 ウー start
    [Tooltip("移動方法")]
    public MoveType MovementStyle;

    [Tooltip("危険度")]
    public int DangerLevel;
    // 2026.01.18 ウー end
}
