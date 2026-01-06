using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class V_RoomPlayerState : MonoBehaviour
{
    /// <summary>
    /// 状態表示
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI _stateTxt;

    /// <summary>
    /// 背景
    /// </summary>
    [SerializeField]
    private Image _bg;

    /// <summary>
    /// 前の城ボタン
    /// </summary>
    [SerializeField]
    private Button _notReadyBtn;

    /// <summary>
    /// 次の城ボタン
    /// </summary>
    [SerializeField]
    private Button _readyBtn;

    /// <summary>
    /// 部屋コントローラ
    /// </summary>
    [SerializeField]
    private C_Room _roomController;

    /// <summary>
    /// コントロールモードですか
    /// </summary>
    private bool _isControllMode;

    void Awake()
    {
        _isControllMode = false;
    }

    /// <summary>
    /// 閲覧モード
    /// </summary>
    public void ViewMode()
    {
        _notReadyBtn.gameObject.SetActive(false);
        _readyBtn.gameObject.SetActive(false);
        _stateTxt.gameObject.SetActive(true);
    }

    /// <summary>
    /// 操作モード
    /// </summary>
    public void ControllMode()
    {
        _notReadyBtn.gameObject.SetActive(false);
        _readyBtn.gameObject.SetActive(true);
        _stateTxt.gameObject.SetActive(false);
        _isControllMode = true;
    }

    /// <summary>
    /// 状態を設定
    /// </summary>
    /// <param name="state">新しい状態</param>
    public void SetState(GameReadyState state)
    {
        Color bgColor = _bg.color;
        switch (state)
        {
            case GameReadyState.Preparing:
                _stateTxt.text = "準備中";
                _bg.color = new Color(bgColor.r, bgColor.g, bgColor.b, 0);
                if (_isControllMode)
                {
                    _readyBtn.gameObject.SetActive(true);
                    _notReadyBtn.gameObject.SetActive(false);
                }
                break;
            case GameReadyState.Ready:
                _stateTxt.text = "準備完了";
                _bg.color = new Color(bgColor.r, bgColor.g, bgColor.b, 225);
                if (_isControllMode)
                {
                    _readyBtn.gameObject.SetActive(false);
                    _notReadyBtn.gameObject.SetActive(true);
                }
                break;
            case GameReadyState.Null:
                _stateTxt.text = "";
                _bg.color = new Color(bgColor.r, bgColor.g, bgColor.b, 0);
                if (_isControllMode)
                {
                    _readyBtn.gameObject.SetActive(false);
                    _notReadyBtn.gameObject.SetActive(false);
                }
                break;
        }
    }

    /// <summary>
    /// 準備完了
    /// </summary>
    public void Ready()
    {
        _roomController.ChangeState(GameReadyState.Ready);
    }

    /// <summary>
    /// 準備をキャンセル
    /// </summary>
    public void CancelReady()
    {
        _roomController.ChangeState(GameReadyState.Preparing);
    }
}
