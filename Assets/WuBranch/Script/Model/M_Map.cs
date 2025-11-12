using System;
using System.Collections.Generic;
using UnityEngine;

public class M_Map
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private List<List<int>> _map;

    /// <summary>
    /// ルート
    /// </summary>
    private List<List<int>> _path;

    /// <summary>
    /// ルートが更新されたの通知
    /// </summary>
    public event Action<List<List<int>>> OnUpdatePath;

    /// <summary>
    /// ルートをゲット
    /// </summary>
    /// <returns>ルート</returns>
    public List<List<int>> GetPath()
    {
        return _path;
    }

    /// <summary>
    /// ルートを設定
    /// </summary>
    /// <param name="path">ルート</param>
    public void SetPath(List<List<int>> path)
    {
        _path = path;
        OnUpdatePath.Invoke(path);
    }
}
