using UnityEngine;
using UnityEngine.UIElements;

public class M_PlayerPosInfo
{
    /// <summary>
    /// マップ座標
    /// </summary>
    public M_MapPosition MapPos;

    /// <summary>
    /// 使用中かどうか
    /// </summary>
    public bool IsUsed;

    /// <summary>
    /// 向き
    /// </summary>
    public Quaternion Dir;

    public M_PlayerPosInfo(M_MapPosition mapPos, bool isUsed, bool isLeft)
    {
        MapPos = mapPos;
        IsUsed = isUsed;
        // 元画像は左向きなので、右向きなら-1、左向きなら1
        Dir = new Quaternion(0, isLeft ? 0 : 180, 0, 0);
    }


}
