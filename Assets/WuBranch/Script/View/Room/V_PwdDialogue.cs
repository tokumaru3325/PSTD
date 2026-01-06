using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Steamworks;

public class V_PwdDialogue : MonoBehaviour
{
    /// <summary>
    /// パスワード入力欄
    /// </summary>
    [SerializeField]
    private TMP_InputField _pwdInput;

    /// <summary>
    /// エラーメッセージ表示欄
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI _errMsgTxt;

    /// <summary>
    /// 呼び出し元の部屋ID
    /// </summary>
    private CSteamID _roomID;

    /// <summary>
    /// 部屋管理者
    /// </summary>
    private C_RoomManager _roomManager;

    void Awake()
    {
        _roomManager = FindFirstObjectByType<C_RoomManager>();
        if (_roomManager)
            _roomManager.OnJoinResultRecieved += RecieveJoinResult;
    }

    void Start()
    {
        Close();
    }

    /// <summary>
    /// 開く
    /// </summary>
    /// <param name="lobbyID">クリックしたロビーのID</param>
    public void Open(CSteamID roomID)
    {
        _roomID = roomID;
        gameObject.SetActive(true);
    }

    /// <summary>
    /// 閉じる
    /// </summary>
    private void Close()
    {
        _pwdInput.text = "";
        _errMsgTxt.text = "";
        gameObject.SetActive(false);
    }

    public void Confirm()
    {
        if (_roomManager)
            _roomManager.JoinLobby(_roomID, _pwdInput.text);
    }

    public void Cancel()
    {
        Close();
    }

    private void RecieveJoinResult(bool isSuccess, string errMsg)
    {
        if (isSuccess)
        {
            Close();
        }
        else
        {
            //エラーメッセージ表示
            _errMsgTxt.text = errMsg;
        }
    }
}
