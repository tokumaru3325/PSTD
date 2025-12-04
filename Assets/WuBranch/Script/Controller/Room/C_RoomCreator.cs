using Netcode.Transports;
using Steamworks;
using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class C_RoomCreator : MonoBehaviour
{
    /// <summary>
    /// ロビー作成コールバック
    /// </summary>
    private CallResult<LobbyCreated_t> _onLobbyCreated;

    //ロビーデータ設定用キー
    private const string s_HostAddressKey = "HostAddress";

    /// <summary>
    /// 部屋の名前のキー
    /// </summary>
    private const string ROOM_NAME_KEY = "RoomName";

    /// <summary>
    /// 部屋のパスワードのキー
    /// </summary>
    private const string ROOM_PASSWORD_KEY = "RoomPassword";

    public ulong LobbyID { get; private set; }

    /// <summary>
    /// 部屋の情報
    /// </summary>
    private M_RoomData _roomData;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //SteamManagerの初期化が完了していたら
        if (SteamManager.Initialized)
        {
            _onLobbyCreated = CallResult<LobbyCreated_t>.Create(OnCreateLobby);
        }
    }

    /// <summary>
    /// ロビー作成（ゲームをホスト）
    /// </summary>
    /// <param name="data">部屋データ</param>
    public void CreateLobby(M_RoomData data)
    {
        _roomData = data;
        SteamAPICall_t hCreateLobby = SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, data.MaxMembers);
        _onLobbyCreated.Set(hCreateLobby);
    }

    private void OnCreateLobby(LobbyCreated_t result, bool bIOFailure)
    {
        //ロビー作成成功していなかった場合
        if (result.m_eResult != EResult.k_EResultOK || bIOFailure)
        {
            return;
        }

        //ホストのアドレス（SteamID）を登録
        SteamMatchmaking.SetLobbyData(
            new CSteamID(result.m_ulSteamIDLobby),
            s_HostAddressKey,
            SteamUser.GetSteamID().ToString());

        SetRoomInfo(result.m_ulSteamIDLobby);

        //ロビーID保存
        LobbyID = result.m_ulSteamIDLobby;

        //ホスト開始
        NetworkManager.Singleton.StartHost();
        //シーンを切り替え
        NetworkManager.Singleton.SceneManager.LoadScene("Room", LoadSceneMode.Single);
    }

    /// <summary>
    /// 部屋の情報を設定
    /// </summary>
    private void SetRoomInfo(ulong LobbyID)
    {
        CSteamID steamID = new CSteamID(LobbyID);
        SteamMatchmaking.SetLobbyData(steamID, ROOM_NAME_KEY, _roomData.Name);
        SteamMatchmaking.SetLobbyData(steamID, ROOM_PASSWORD_KEY, _roomData.Password);
    }
}
