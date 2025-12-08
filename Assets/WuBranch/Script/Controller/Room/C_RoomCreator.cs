using Steamworks;
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
    private const string HOST_ADDRESS_KEY = "HostAddress";

    public ulong LobbyID { get; private set; }

    /// <summary>
    /// 共通変数
    /// </summary>
    private C_GlobalVariable _globalVariable;

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
        _globalVariable = FindFirstObjectByType<C_GlobalVariable>();
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

        CSteamID steamID = new CSteamID(result.m_ulSteamIDLobby);
        //ホストのアドレス（SteamID）を登録
        SteamMatchmaking.SetLobbyData(
            steamID,
            HOST_ADDRESS_KEY,
            SteamUser.GetSteamID().ToString());

        SetRoomInfo(steamID);
        SetMemberInfo(steamID);

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
    private void SetRoomInfo(CSteamID LobbyID)
    {
        SteamMatchmaking.SetLobbyData(LobbyID, RoomParams.GAME_ID_KEY, RoomParams.GAME_ID_VALUE);
        SteamMatchmaking.SetLobbyData(LobbyID, RoomParams.VERSION_KEY, RoomParams.VERSION_VALUE);
        SteamMatchmaking.SetLobbyData(LobbyID, RoomParams.ROOM_NAME_KEY, _roomData.Name);
        SteamMatchmaking.SetLobbyData(LobbyID, RoomParams.ROOM_PASSWORD_KEY, _roomData.Password);
        SteamMatchmaking.SetLobbyData(LobbyID, RoomParams.ROOM_LEADER_KEY, _globalVariable.GetMyName());
    }

    private void SetMemberInfo(CSteamID LobbyID)
    {
        //SteamMatchmaking.SetLobbyMemberData(LobbyI,);
    }
}
