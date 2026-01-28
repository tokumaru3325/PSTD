using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class V_DarkMask : MonoBehaviour, IPointerDownHandler
{

    /// <summary>
    /// クリックできるかどうかのフラグ
    /// </summary>
    [SerializeField]
    private bool _backgroundClickSwitch;

    /// <summary>
    /// 障害物のマネージャー
    /// </summary>
    [SerializeField]
    private C_ObstacleManager _ObstacleManager;

    /// <summary>
    /// クリックできるようにする
    /// </summary>
    public void EnableClickEvent()
    {
        _backgroundClickSwitch = true;
    }

    /// <summary>
    /// クリックできないように
    /// </summary>
    public void DisableClickEvent()
    {
        _backgroundClickSwitch = false;
    }

    /// <summary>
    /// マスクを開く
    /// </summary>
    public void OpenMask()
    {
        gameObject.SetActive(true);

        // raycasterのeventMaskの値を変更
        // 障害物だけが反応できるように
        Physics2DRaycaster raycaster = Camera.main.GetComponent<Physics2DRaycaster>();
        //LayerMask.LayerToName(-1);
        raycaster.eventMask = LayerMask.GetMask("Obstacle");
    }

    /// <summary>
    /// マスクを閉じる
    /// </summary>
    public void CloseMask()
    {
        // raycasterのeventMaskの値を変更
        // 障害物だけが反応できるように
        Physics2DRaycaster raycaster = Camera.main.GetComponent<Physics2DRaycaster>();
        raycaster.eventMask = ~0;
        gameObject.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!_backgroundClickSwitch)
            return;

        DisableClickEvent();
        CloseMask();
        _ObstacleManager.DisableObstacleSelection();
    }
}
