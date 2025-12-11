using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    /// 呼び出し元のコントローラー
    /// </summary>
    private C_RoomFront _callerController;

    void Start()
    {
        Close();
    }

    /// <summary>
    /// 開く
    /// </summary>
    /// <param name="lobbyID">クリックしたロビーのID</param>
    public void Open(C_RoomFront caller)
    {
        _callerController = caller;
        _callerController.OnJoinResultRecieved += RecieveJoinResult;
        gameObject.SetActive(true);
    }

    /// <summary>
    /// 閉じる
    /// </summary>
    private void Close()
    {
        if (_callerController)
            _callerController.OnJoinResultRecieved -= RecieveJoinResult;
        _pwdInput.text = "";
        _errMsgTxt.text = "";
        gameObject.SetActive(false);
    }

    public void Confirm()
    {
        if (_callerController)
            _callerController.JoinLobby(_pwdInput.text);
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
