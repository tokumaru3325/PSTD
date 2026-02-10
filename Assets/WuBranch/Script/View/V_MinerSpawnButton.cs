using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class V_MinerSpawnButton : SpawnButton
{
    [Tooltip("画面全体のマスク")]
    [SerializeField]
    private V_DarkMask _mask;

    [Tooltip("キャンパスのグラフィックレイキャスター")]
    [SerializeField]
    private GraphicRaycaster _canvasRaycaster;

    [Tooltip("マーク管理者")]
    [SerializeField]
    private V_ObstacleMarkManager _markManger;

    [SerializeField]
    private C_ObstacleManager _obstacleManager;

    protected override void Start()
    {
        base.Start();
        _mask.OnClosed += HandleMaskClosed;
    }

    public override void OnButtonDown_Spawn()
    {
        if (_isGameEnding) //[2026/01/13] プリンス 追加
            return;

        // raycasterのeventMaskの値を変更
        // 障害物だけが反応できるように
        Physics2DRaycaster raycaster = Camera.main.GetComponent<Physics2DRaycaster>();
        raycaster.eventMask = LayerMask.GetMask("Obstacle");
        _canvasRaycaster.blockingMask = LayerMask.GetMask("Obstacle");
        _obstacleManager.EnableObstacleSelection();
        _mask.EnableClickEvent();
        _mask.OpenMask();
        _markManger.ShowMarks();
    }

    /// <summary>
    /// 採掘者を生成開始
    /// </summary>
    /// <param name="target">目的</param>
    public void SpawnMiner(GameObject target)
    {
        SpawnUnit(target);

        _mask.DisableClickEvent();
        _mask.CloseMask();
    }

    /// <summary>
    /// マスクが閉じた後の処理
    /// </summary>
    private void HandleMaskClosed()
    {
        // raycasterのeventMaskの値を変更
        // 障害物だけが反応できるように
        Physics2DRaycaster raycaster = Camera.main.GetComponent<Physics2DRaycaster>();
        raycaster.eventMask = ~0;
        _canvasRaycaster.blockingMask = LayerMask.GetMask("Nothing");

        _obstacleManager.DisableObstacleSelection();
        _markManger.CloseMarks();
    }

    void OnDestroy()
    {
        _mask.OnClosed -= HandleMaskClosed;
    }
}
