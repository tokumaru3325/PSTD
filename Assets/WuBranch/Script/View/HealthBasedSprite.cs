using System;
using UnityEngine;

[Serializable]
public struct HealthBasedSprite
{
    /// <summary>
    /// スプライト
    /// </summary>
    [Tooltip("画像")]
    [SerializeField]
    public Sprite Sprite;

    /// <summary>
    /// 体力の閾値(パーセント)
    /// </summary>
    [Tooltip("体力の閾値")]
    [SerializeField]
    public float ThresholdPecent;
}
