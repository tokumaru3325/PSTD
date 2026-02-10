using System;
using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class ReelInfo
{
    [Tooltip("画像")]
    [SerializeField]
    public Sprite[] Sprites;
}

[Serializable]
public class WantResult
{
    [Tooltip("指定された画像を結果にする(それぞれロールの画像のインデックス,0 ~ 画像の数-1)")]
    [SerializeField]
    public int[] Indices;
}

public class V_TitleSlot : MonoBehaviour, ITitleSlotView
{
    /// <summary>
    /// リールデータ
    /// </summary>
    public ReelInfo[] ReelSprites { get; private set; }

    /// <summary>
    /// あたりのエフェクト
    /// </summary>
    private V_BlinkEffect _winEffect;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _winEffect = GetComponentInChildren<V_BlinkEffect>();
    }

    /// <summary>
    /// 各リールの画像を設定
    /// </summary>
    /// <param name="data"></param>
    public void SetReelsSprite(ReelInfo[] data)
    {
        ReelSprites = data;
    }

    /// <summary>
    /// あたりのエフェクトを表示する
    /// </summary>
    public void StartWinEffect()
    {
        _winEffect?.StartBlinking();
    }
}
