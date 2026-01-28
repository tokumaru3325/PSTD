using UnityEngine;

public class V_MinerSpawnButton : SpawnButton
{
    [Tooltip("画面全体のマスク")]
    [SerializeField]
    private V_DarkMask _mask;

    [SerializeField]
    private C_ObstacleManager _obstacleManager;

    public override void OnButtonDown_Spawn()
    {
        if (_isGameEnding) //[2026/01/13] プリンス 追加
            return;

        _obstacleManager.EnableObstacleSelection();
        _mask.EnableClickEvent();
        _mask.OpenMask();
    }

    /// <summary>
    /// 採掘者を生成開始
    /// </summary>
    /// <param name="target">目的</param>
    public void SpawnMiner(GameObject target)
    {
        SpawnUnit(target);

        _obstacleManager.DisableObstacleSelection();
        CloseMask();
    }

    private void CloseMask()
    {
        _mask.DisableClickEvent();
        _mask.CloseMask();
    }
}
