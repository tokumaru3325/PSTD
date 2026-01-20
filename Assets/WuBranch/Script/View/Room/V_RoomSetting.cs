using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class V_RoomSetting : MonoBehaviour
{
    /// <summary>
    /// 作成ボタン
    /// </summary>
    [SerializeField]
    private Button _createBtn;

    /// <summary>
    /// キャンセルボタン
    /// </summary>
    [SerializeField]
    private Button _cancelBtn;

    /// <summary>
    /// 部屋管理者
    /// </summary>
    [SerializeField]
    private C_RoomManager _roomManager;

    /// <summary>
    /// 部屋名の入力
    /// </summary>
    [SerializeField]
    private TMP_InputField _roomNameInput;

    /// <summary>
    /// 部屋名のエラーメッセージ
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI _roomNameErrTxt;

    /// <summary>
    /// パスワードの入力
    /// </summary>
    [SerializeField]
    private TMP_InputField _passwordInput;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (_createBtn)
            _createBtn.onClick.AddListener(CreateRoom);
        if (_cancelBtn)
            _cancelBtn.onClick.AddListener(CancelCreatingRoom);
        if (!_roomManager)
            _roomManager = FindFirstObjectByType<C_RoomManager>();
        SetNameErrTxt("");
    }

    /// <summary>
    /// 部屋を作る
    /// </summary>
    public void CreateRoom()
    {
        if (_roomNameInput.text.Equals(""))
        {
            SetNameErrTxt("部屋名を入力してください");
            return;
        }
        if (_roomManager)
            _roomManager.CreateLobby(_roomNameInput.text, _passwordInput.text);
        ClosePanel();
    }

    /// <summary>
    /// 部屋を作るのをやめる
    /// </summary>
    public void CancelCreatingRoom()
    {
        ClosePanel();
        // シーンを変更
        //SceneManager.LoadScene("ModeSelect", LoadSceneMode.Single);
    }

    /// <summary>
    /// パネルを閉じる
    /// </summary>
    private void ClosePanel()
    {
        _roomNameInput.text = "";
        _passwordInput.text = "";
        gameObject.SetActive(false);
    }

    private void SetNameErrTxt(string msg)
    {
        if (_roomNameErrTxt)
            _roomNameErrTxt.text = msg;
    }
}
