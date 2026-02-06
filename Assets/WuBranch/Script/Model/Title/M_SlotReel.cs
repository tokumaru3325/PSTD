using UnityEngine;

public class M_SlotReel
{
    /// <summary>
    /// スクロールのID
    /// </summary>
    public int ReelID { get; set; }

    /// <summary>
    /// 今の回転速度
    /// </summary>
    public float CurrentSpinSpeed { get; set; }

    /// <summary>
    /// 回転開始フラグ
    /// </summary>
    public bool IsSpinning { get; set; }

    /// <summary>
    /// 止まるフラグ
    /// </summary>
    public bool IsStopping { get; set; }

    /// <summary>
    /// 目標を探すフラグ
    /// </summary>
    public bool IsSeeking { get; set; }

    /// <summary>
    /// 止まる時の画像のインデックス
    /// </summary>
    public int TargetIndex { get; set; } = -1;

    /// <summary>
    /// スクロールの最大回転速度
    /// </summary>
    public float MaxSpinSpeed { get; set; } = 1000f;

    /// <summary>
    /// スクロールの最小回転速度
    /// </summary>
    public float MinSpinSpeed { get; set; } = 300f;

    /// <summary>
    /// スクロールの回転加速度
    /// </summary>
    public float StartAcceleration { get; set; } = 50f;

    /// <summary>
    /// 停止したい時の加速度
    /// </summary>
    public float Deceleration { get; set; } = 50f;

    /// <summary>
    /// 完全停止まるまでの距離
    /// </summary>
    public float DistToStop { get; set; } = 20f;

    /// <summary>
    /// 完全停止まるまでの時間(秒)
    /// </summary>
    public float TimeToStop { get; set; } = 1.0f;

    /// <summary>
    /// 完全停止まるまでの最小スピード
    /// </summary>
    public float MinSpeedToStop { get; set; } = 300f;

    public M_SlotReel(int id, M_SlotReelData data)
    {
        ReelID = id;
        MaxSpinSpeed = data.MaxSpinSpeed;
        MinSpinSpeed = data.MinSpinSpeed;
        StartAcceleration = data.StartSpinAcceleration;
        Deceleration = data.Deceleration;
        DistToStop = data.DistToStop;
        TimeToStop = data.TimeToStop;
        MinSpeedToStop = data.MinSpeedToStop;
    }


    /// <summary>
    /// 回転速度の更新
    /// </summary>
    public void UpdateSpeed()
    {
        // どんどん速くなるが、最大には越えない
        if (IsSeeking)
            CurrentSpinSpeed = Mathf.Clamp(CurrentSpinSpeed - Deceleration, MinSpinSpeed, MaxSpinSpeed);
        else
            CurrentSpinSpeed = Mathf.Clamp(CurrentSpinSpeed + StartAcceleration, 0, MaxSpinSpeed);
    }
}
