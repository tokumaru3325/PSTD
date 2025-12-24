using UnityEngine;

[CreateAssetMenu(fileName = "BuffDataParent", menuName = "Scriptable Objects/BuffDataParent")]
public abstract class BuffDataParent : ScriptableObject
{
    public float AttackPower = 0;
    public float AttackSpeed = 0;
    public float AttackRange = 0;
    public float MoveSpeed = 0;
}
