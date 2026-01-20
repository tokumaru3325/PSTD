using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuffDataBase", menuName = "Scriptable Objects/BuffDataBase")]
public abstract class BuffDataBase : ScriptableObject
{
    public float AttackPower = 0;
    public float AttackSpeed = 0;
    public float AttackRange = 0;
    public float MoveSpeed = 0;
}
