using System.Collections.Generic;
using UnityEngine;

public class C_ObstacleManager : MonoBehaviour
{
    /// <summary>
    /// マップマネージャー
    /// </summary>
    [SerializeField]
    private C_MapManager _mapManager;

    /// <summary>
    /// 障害物
    /// </summary>
    [SerializeField]
    private GameObject _stonePrefab;

    void Awake()
    {
        if (_mapManager)
        {
            _mapManager.OnMapLoadCompleted += HandleMapCompleted;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            C_ObstacleStone[] stones = GetComponentsInChildren<C_ObstacleStone>();
            if (stones.Length > 0)
            {
                stones[0].DecreaseHP(10);
            }
        }
    }

    /// <summary>
    /// 障害物を生成
    /// </summary>
    private void HandleMapCompleted()
    {
        if (!_mapManager)
            return;

        List<M_MapPosition> obstacles = _mapManager.GetObstacles();
        foreach (M_MapPosition obstacle in obstacles)
        {
            C_ObstacleStone obj = CreateStone(obstacle);
            if (!obj)
            {
                Debug.Log($"Create Stone Failed");
                return;
            }
            // 初期化
            obj.OnDead += HandleDestroyObstacle;
        }
    }

    /// <summary>
    /// 石の障害物を生成
    /// </summary>
    /// <param name="mapPos">マップ位置</param>
    /// <returns>石のオブジェクト</returns>
    private C_ObstacleStone CreateStone(M_MapPosition mapPos)
    {
        if (!_stonePrefab)
            return null;

        Vector3 unityPos = _mapManager.ConvertToUnityPos(mapPos);
        GameObject stone = Instantiate(_stonePrefab, unityPos, Quaternion.identity, this.transform);
        return stone.GetComponent<C_ObstacleStone>();
    }

    /// <summary>
    /// 障害物を削除する
    /// </summary>
    private void HandleDestroyObstacle(Vector3 pos)
    {
        _mapManager.DestroyObstacle(pos);
    }
}
