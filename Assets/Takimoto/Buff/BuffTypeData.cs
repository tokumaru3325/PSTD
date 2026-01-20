using UnityEngine;

[CreateAssetMenu(fileName = "BuffTypeData", menuName = "Scriptable Objects/BuffTypeData")]
public class BuffTypeData : ScriptableObject
{
    public float BuffValue;
    public int BuffTime;

    // 2026.01.17 ウー start
    [Tooltip("エフェクトの色")]
    [ColorUsage(true, true)]
    public Color EffectColor;

    [Tooltip("アイコン")]
    public Sprite BuffIcon;
    // 2026.01.17 ウー end
}
