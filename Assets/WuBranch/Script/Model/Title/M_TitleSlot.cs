using System;
using System.Linq;
using UnityEngine;

public class M_TitleSlot
{
    /// <summary>
    /// 回しているですか
    /// </summary>
    public bool IsRunning { get; set; }

    /// <summary>
    /// 今遊んだ回数
    /// </summary>
    public int CurrentPlayCount { get; set; }

    /// <summary>
    /// 何回遊んだら強制的に当たる
    /// </summary>
    public int MaxPlayBeforeForceWin { get; set; } = 3;

    /// <summary>
    /// 強制的に成功させるフラグ
    /// </summary>
    public bool ForceCorrect { get; set; }

    // PSTDを組んだらゲーム開始、ｘｘｘｘを組んだらゲーム終了
    /// <summary>
    /// ゲーム開始の組み合わせ
    /// </summary>
    public WantResult[] StartResults;

    /// <summary>
    /// ゲーム終了の組み合わせ
    /// </summary>
    public WantResult[] CloseResults;

    /// <summary>
    /// 指定された画像を結果にする(それぞれロールの画像のインデックス,0 ~ 画像の数-1)
    /// </summary>
    public int[] StopIndices;

    /// <summary>
    /// プレイヤーがコントロールするか
    /// </summary>
    public bool CustomControll;

    /// <summary>
    /// 各ロールを止まる時の時間差
    /// </summary>
    public float StopDelay;

    /// <summary>
    /// 止まる位置
    /// </summary>
    public float StopHeightOffset;

    public M_TitleSlot(M_TitleSlotData data)
    {
        MaxPlayBeforeForceWin = data.MaxPlay;
        StartResults = data.StartResults;
        CloseResults = data.CloseResults;
        StopIndices = data.StopIndices;
        CustomControll = data.CustomControll;
        StopDelay = data.StopDelay;
        StopHeightOffset = data.StopHeightOffset;
    }

    /// <summary>
    /// 各ロールの結果を確認
    /// </summary>
    /// <param name="currentReelIndices">各ロールの結果</param>
    /// <returns>true: あたり、false: 当たってない</returns>
    public SlotResultType CheckResult(int[] currentReelIndices)
    {
        if (IsMatch(CloseResults, currentReelIndices))
            return SlotResultType.Close;
        if (IsMatch(StartResults, currentReelIndices))
            return SlotResultType.Start;
        return SlotResultType.Miss;
    }

    /// <summary>
    /// スロットの結果が欲しい結果ですか
    /// </summary>
    /// <param name="target">欲しい結果</param>
    /// <param name="reelResult">スロットの結果</param>
    /// <returns>true: はい, false: いいえ</returns>
    private bool IsMatch(WantResult[] target, int[] reelResult)
    {
        if (target == null) return false;
        for (int index = 0; index < reelResult.Length; index++)
        {
            if (!target[index].Indices.Contains(reelResult[index]))
                return false;
        }
        return true;
    }

    /// <summary>
    /// ランダムの結果を生成
    /// </summary>
    /// <param name="reelCount">結果の数</param>
    /// <param name="imageCounts">画像の総数</param>
    /// <returns>全部の結果</returns>
    public int[] GenerateRandomResult(int reelCount, int[] imageCounts)
    {
        int[] result = new int[reelCount];
        for (int i = 0; i < reelCount; i++)
            result[i] = UnityEngine.Random.Range(0, imageCounts[i]);

        Debug.Log("今回の結果: " + string.Join(", ", result));
        return result;
    }
}

[Serializable]
public class WantResultT
{
    [Tooltip("指定された画像を結果にする(それぞれロールの画像のインデックス,0 ~ 画像の数-1)")]
    [SerializeField]
    public int[] Indices;
}
