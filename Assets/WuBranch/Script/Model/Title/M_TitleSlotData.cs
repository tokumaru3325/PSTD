using UnityEngine;

[CreateAssetMenu(fileName = "M_TitleSlotData", menuName = "Scriptable Objects/Title Slot Data")]
public class M_TitleSlotData : ScriptableObject
{
    [Tooltip("ロールデータ")]
    public ReelInfo[] Reels;

    [Tooltip("各ロールを止まる時の時間差")]
    public float StopDelay = 1.0f;

    [Tooltip("止まる位置")]
    public float StopHeightOffset = 0f;

    /// <summary>
    /// PSTDを組んだらゲーム開始、ｘｘｘｘを組んだらゲーム終了
    /// </summary>
    [Tooltip("ゲーム開始の組み合わせ")]
    public WantResult[] StartResults;

    [Tooltip("ゲーム終了の組み合わせ")]
    public WantResult[] CloseResults;

    [Tooltip("指定された画像を結果にする(それぞれロールの画像のインデックス,0 ~ 画像の数-1)")]
    public int[] StopIndices;

    [Tooltip("何回遊んだら強制的に当たる")]
    public int MaxPlay = 3;

    [Tooltip("プレイヤーがコントロールするか")]
    public bool CustomControll;
}
