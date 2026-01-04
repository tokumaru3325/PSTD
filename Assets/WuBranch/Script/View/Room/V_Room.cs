using System.Collections.Generic;
using Steamworks;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(C_Room))]
public class V_Room : MonoBehaviour
{
    /// <summary>
    /// バックボタン
    /// </summary>
    [SerializeField]
    private Button _backBtn;

    /// <summary>
    /// 部屋名表示テキスト
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI _roomNameTxt;

    /// <summary>
    /// ホストプレイヤー
    /// </summary>
    [SerializeField]
    private V_RoomPlayer _hostPlayer;

    /// <summary>
    /// クライアントプレイヤー
    /// </summary>
    [SerializeField]
    private V_RoomPlayer _clientPlayer;

    /// <summary>
    /// コントローラ
    /// </summary>
    private C_Room _myController;

    void Awake()
    {
        _myController = GetComponent<C_Room>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (_backBtn)
            _backBtn.onClick.AddListener(BackToSelectMode);

        _myController.OnLobbyUpdated += UpdatedRoom;
        _myController.OnMemberChanged += UpdatePlayerName;
        _myController.OnMemberUpdated += UpdatePlayerInfo;
    }

    /// <summary>
    /// 前の画面に戻る
    /// </summary>
    public void BackToSelectMode()
    {
        NetworkManager.Singleton.SceneManager.LoadScene("RoomList", LoadSceneMode.Single);
    }

    /// <summary>
    /// 部屋名を変更
    /// </summary>
    /// <param name="name"></param>
    public void UpdatedRoom(string name)
    {
        _roomNameTxt.text = name;
    }

    /// <summary>
    /// 対象プレイヤを取得
    /// </summary>
    /// <param name="playerID">プレイヤID</param>
    /// <returns>プレイヤ</returns>
    private V_RoomPlayer GetPlayer(CSteamID playerID)
    {
        if (_myController.RoleMap[playerID] == MultiRoleType.Host)
            return _hostPlayer;
        else if (_myController.RoleMap[playerID] == MultiRoleType.Client)
            return _clientPlayer;
        else
            return null;
    }

    /// <summary>
    /// プレイヤの名前を更新
    /// </summary>
    /// <param name="playerID">プレイヤID</param>
    /// <param name="name">プレイヤ名</param>
    private void UpdatePlayerName(CSteamID playerID, string name)
    {
        V_RoomPlayer player = GetPlayer(playerID);
        if (player)
        {
            Debug.Log($"update Player name : {playerID} , {name}");
            player.SetPlayerName(name);
        }
    }

    /// <summary>
    /// プレイヤの情報を更新
    /// </summary>
    /// <param name="playerID">プレイヤID</param>
    /// <param name="data">プレイヤ情報</param>
    private void UpdatePlayerInfo(CSteamID playerID, M_RoomPlayerData data)
    {
        V_RoomPlayer player = GetPlayer(playerID);
        if (player)
        {
            player.SetCastleImg(data.CastleIndex);
            player.SetState(data.State);
        }
    }
}
