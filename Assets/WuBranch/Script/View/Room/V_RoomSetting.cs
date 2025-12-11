using System.Collections.Generic;
using Steamworks;
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

    /// <summary>
    /// 部屋を作る
    /// </summary>
    public void CreateRoom()
    {
        _creator.CreateLobby(_roomNameInput.text, _passwordInput.text);
    }

    /// <summary>
    /// 部屋を作るのをやめる
    /// </summary>
    public void CancelCreatingRoom()
    {
        // 初期化
        _roomNameInput.text = "";
        _passwordInput.text = "";
        // シーンを変更
        SceneManager.LoadScene("ModeSelect", LoadSceneMode.Single);
    }
}
