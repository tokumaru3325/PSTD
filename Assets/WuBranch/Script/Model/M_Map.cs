using System;
using System.Collections.Generic;
using UnityEngine;

public class M_Map
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private List<List<int>> _map;

    /// <summary>
    /// ルートコスト
    /// </summary>
    private List<List<int>> _pathCost;

    /// <summary>
    /// ルートが更新されたの通知
    /// </summary>
    public event Action<List<List<int>>> OnUpdatePath;

    public M_Map()
    {
        _map = new List<List<int>>();
        _pathCost = new List<List<int>>();
    }

    /// <summary>
    /// ルートをゲット
    /// </summary>
    /// <returns>ルート</returns>
    public List<List<int>> GetPath()
    {
        return _pathCost;
    }

    /// <summary>
    /// ルートコストをゲット
    /// </summary>
    /// <param name="x">マップ座標X</param>
    /// <param name="y">マップ座標Y</param>
    /// <returns>コスト</returns>
    public int GetPathCost(int x, int y)
    {
        if (y < 0 || y >= _pathCost.Count || x < 0 || x >= _pathCost[y].Count)
        {
            return (int)PathStructure.Blocked;
        }
        return _pathCost[y][x];
    }

    /// <summary>
    /// ルートを設定
    /// </summary>
    /// <param name="path">ルート</param>
    public void SetPath(List<List<int>> path)
    {
        _pathCost = path;
        OnUpdatePath?.Invoke(path);
    }
}
