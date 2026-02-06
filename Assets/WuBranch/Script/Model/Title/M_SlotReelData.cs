using UnityEngine;

[CreateAssetMenu(fileName = "M_SlotReelData", menuName = "Scriptable Objects/Title Slot Reel Data")]
public class M_SlotReelData : ScriptableObject
{
    [Tooltip("生成する図の大きさ")]
    public Vector2 ImageSize = new Vector2(270, 190);

    [Tooltip("スクロールの最大回転速度")]
    public float MaxSpinSpeed = 150f;

    [Tooltip("スクロールの最小回転速度")]
    public float MinSpinSpeed = 50f;

    [Tooltip("スクロールの回転加速度")]
    public float StartSpinAcceleration = 10f;

    [Tooltip("停止の加速度")]
    public float Deceleration = 30f;

    [Tooltip("完全停止まるまでの距離")]
    public float DistToStop = 5.0f;

    [Tooltip("完全停止まるまでの時間(秒)")]
    public float TimeToStop = 3.0f;

    [Tooltip("完全停止まるまでの最小スピード")]
    public float MinSpeedToStop = 50f;
}
