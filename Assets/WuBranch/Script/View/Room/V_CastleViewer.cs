using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class CastleImgPair
{
    /// <summary>
    /// タイプ
    /// </summary>
    [SerializeField]
    public CastleType type;

    /// <summary>
    /// 画像
    /// </summary>
    [SerializeField]
    public Sprite Image;
}

public class V_CastleViewer : MonoBehaviour
{
    /// <summary>
    /// 城の表示画像
    /// </summary>
    [SerializeField]
    private Image _viewer;

    /// <summary>
    /// 全部の城の画像
    /// </summary>
    [SerializeField]
    private CastleImgPair[] _castleSprites;

    /// <summary>
    /// 現在の城のインデックス
    /// </summary>
    private CastleType _currentIndex;

    /// <summary>
    /// 前の城ボタン
    /// </summary>
    [SerializeField]
    private Button _preBtn;

    /// <summary>
    /// 次の城ボタン
    /// </summary>
    [SerializeField]
    private Button _nextBtn;

    /// <summary>
    /// 部屋コントローラ
    /// </summary>
    [SerializeField]
    private C_Room _roomController;

    void Awake()
    {
        _currentIndex = CastleType.Null;
        _viewer.sprite = null;
    }

    /// <summary>
    /// 前の城
    /// </summary>
    public void PreCastle()
    {
        _currentIndex--;
        if (_currentIndex < CastleType.Castle1)
        {
            _currentIndex = CastleType.Castle3;
        }
        _roomController.ChangeCastle(_currentIndex);
    }

    /// <summary>
    /// 次の城
    /// </summary>
    public void NextCastle()
    {
        _currentIndex++;
        if (_currentIndex > CastleType.Castle3)
        {
            _currentIndex = CastleType.Castle1;
        }
        _roomController.ChangeCastle(_currentIndex);
    }

    /// <summary>
    /// 指定された城のタイプを表示
    /// </summary>
    /// <param name="type"></param>
    public void SetCastle(CastleType type)
    {
        _currentIndex = type;
        UpdateCastle();
    }

    /// <summary>
    /// 城の更新
    /// </summary>
    private void UpdateCastle()
    {
        var target = _castleSprites.Where(_ => _.type == _currentIndex);
        if (target.Count() > 0)
            _viewer.sprite = target.First().Image;
        else
            _viewer.sprite = null;
    }

    /// <summary>
    /// 閲覧モード
    /// </summary>
    public void ViewMode()
    {
        _preBtn.gameObject.SetActive(false);
        _nextBtn.gameObject.SetActive(false);
    }

    /// <summary>
    /// 操作モード
    /// </summary>
    public void ControllMode()
    {
        _preBtn.gameObject.SetActive(true);
        _nextBtn.gameObject.SetActive(true);
    }

    /// <summary>
    /// 準備状態により、ボタンの操作可能かどうかが変わる
    /// </summary>
    /// <param name="state">準備状態</param>
    public void ChangeBtnInteractivity(GameReadyState state)
    {
        if (state == GameReadyState.Ready)
        {
            _preBtn.interactable = false;
            _nextBtn.interactable = false;
        }
        else if (state == GameReadyState.Preparing)
        {
            _preBtn.interactable = true;
            _nextBtn.interactable = true;
        }
    }
}
