using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// マップ座標、左上が0,0(Unityの座標だと(-18.5, 8.5))
/// </summary>
public struct M_MapPosition
{
    public int X;
    public int Y;

    public M_MapPosition(int colIndex, int rowNode) : this()
    {
        X = colIndex;
        Y = rowNode;
    }
}
