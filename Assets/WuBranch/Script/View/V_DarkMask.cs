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
    /// 閉じた後の処理
    /// </summary>
    public Action OnClosed;

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
    }

    /// <summary>
    /// マスクを閉じる
    /// </summary>
    public void CloseMask()
    {
        gameObject.SetActive(false);
        OnClosed?.Invoke();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!_backgroundClickSwitch)
            return;

        DisableClickEvent();
        CloseMask();
    }
}
