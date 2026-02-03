using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class V_ObstacleMarkManager : MonoBehaviour
{
    [Tooltip("マークのプレハブ")]
    [SerializeField]
    private V_ObstacleMark _markPrefab;

    /// <summary>
    /// マップマネージャー
    /// </summary>
    private C_MapManager _mapManager;

    /// <summary>
    /// マーク
    /// </summary>
    private V_ObstacleMark[] _marks = new V_ObstacleMark[0];

    /// <summary>
    /// 障害物
    /// </summary>
    private List<M_MapPosition> _obstacles;

    void Awake()
    {
        _mapManager = FindFirstObjectByType<C_MapManager>();
        if (!_mapManager)
            return;
        _mapManager.OnMapLoadCompleted += HandleMapLoaded;
    }

    /// <summary>
    /// マップロード完了の処理
    /// </summary>
    private void HandleMapLoaded()
    {
        _obstacles = _mapManager.GetObstacles();
        _marks = new V_ObstacleMark[_obstacles.Count];
        CreateAllDirMark(_obstacles);
    }

    /// <summary>
    /// マークを作る
    /// </summary>
    /// <param name="obstacles"></param>
    private void CreateAllDirMark(List<M_MapPosition> obstacles)
    {
        int nums = obstacles.Count;
        for (int index = 0; index < nums; index++)
        {
            V_ObstacleMark mark = Instantiate(_markPrefab, Vector3.zero, Quaternion.identity, this.transform);
            mark.gameObject.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
            Vector3 pos = _mapManager.ConvertToUnityPos(obstacles[index]);
            mark.SetTarget(pos);
            mark.gameObject.SetActive(false);
            _marks[index] = mark;
        }
    }

    /// <summary>
    /// マークを表示する
    /// </summary>
    public void ShowMarks()
    {
        foreach (M_MapPosition mapPos in _obstacles)
        {
            Vector3 pos = _mapManager.ConvertToUnityPos(mapPos);
            V_ObstacleMark mark = _marks.FirstOrDefault(mark => mark.GetTarget().Equals(pos));
            mark.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// マークを非表示する
    /// </summary>
    public void CloseMarks()
    {
        foreach (var mark in _marks)
        {
            mark.gameObject.SetActive(false);
        }
    }

}
