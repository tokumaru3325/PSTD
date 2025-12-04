using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    /// 部屋を作る人
    /// </summary>
    [SerializeField]
    private C_RoomCreator _creator;

    /// <summary>
    /// 部屋名の入力
    /// </summary>
    [SerializeField]
    private TMP_InputField _roomNameInput;

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
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// 部屋を作る
    /// </summary>
    public void CreateRoom()
    {
        // データを準備
        M_RoomData data = new M_RoomData();
        data.Name = _roomNameInput.text;
        data.Password = _passwordInput.text;
        data.MaxMembers = 2;
        _creator.CreateLobby(data);
    }

    /// <summary>
    /// 部屋を作るのをやめる
    /// </summary>
    public void CancelCreatingRoom()
    {
        // 初期化

        // シーンを変更
        SceneManager.LoadScene("ModeSelect", LoadSceneMode.Single);
    }
}
