using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class V_RoomPlayer : MonoBehaviour
{
    /// <summary>
    /// 
    /// </summary>
    [SerializeField]
    private MultiRoleType _target;

    /// <summary>
    /// プレイヤー名表示欄
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI _playerNameTxt;

    /// <summary>
    /// 城
    /// </summary>
    [SerializeField]
    private V_CastleViewer _castleViewer;

    /// <summary>
    /// プレイヤ状態
    /// </summary>
    [SerializeField]
    private V_RoomPlayerState _playerState;

    /// <summary>
    /// 共通変数
    /// </summary>
    private C_GlobalVariable _globalVariable;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _globalVariable = FindFirstObjectByType<C_GlobalVariable>();
        if (_globalVariable)
        {
            MultiRoleType curRole = _globalVariable.GetRoomRole();
            if (curRole == _target)
            {
                // 操作できる
                _castleViewer.ControllMode();
                _playerState.ControllMode();
                return;
            }
        }

        // それ以外閲覧
        _castleViewer.ViewMode();
        _playerState.ViewMode();
    }

    /// <summary>
    /// プレイヤ名を設定
    /// </summary>
    /// <param name="name">プレイヤ名</param>
    public void SetPlayerName(string name)
    {
        _playerNameTxt.text = name;
    }

    /// <summary>
    /// 城を設定
    /// </summary>
    /// <param name="type">城タイプ</param>
    public void SetCastleImg(CastleType type)
    {
        _castleViewer.SetCastle(type);
    }

    /// <summary>
    /// プレイヤの状態を設定
    /// </summary>
    /// <param name="state">プレイヤの状態</param>
    public void SetState(GameReadyState state)
    {
        _playerState.SetState(state);
        _castleViewer.ChangeBtnInteractivity(state);
    }
}
