using UnityEngine;

[CreateAssetMenu(fileName = "M_Obstacle", menuName = "Scriptable Objects/Obstacle")]
public class M_Obstacle : ScriptableObject
{

    [Tooltip("最大HP")]
    public float MaxHealth;

    [Tooltip("表示する順番")]
    public int ZOrder;
}
