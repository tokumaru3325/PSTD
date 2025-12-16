using System.Text;
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

    /// <summary>
    /// 共通変数
    /// </summary>
    private C_GlobalVariable _globalVariable;

    /// <summary>
    /// 部屋の情報
    /// </summary>
    private M_RoomData _roomData;

    void Awake()
    {
        if (FindObjectsByType<C_RoomCreator>(FindObjectsSortMode.None).Length > 1)
        {
            Destroy(this.gameObject);
            return;
        }

        DontDestroyOnLoad(this.gameObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //SteamManagerの初期化が完了していたら
        if (SteamManager.Initialized)
        {
            _onLobbyCreated = CallResult<LobbyCreated_t>.Create(OnLobbyCreated);
        }
        _globalVariable = FindFirstObjectByType<C_GlobalVariable>();
    }

    /// <summary>
    /// ロビー作成（ゲームをホスト）
    /// </summary>
    /// <param name="data">部屋データ</param>
    public void CreateLobby(string roomName, string pwd)
    {
        _roomData = new M_RoomData();
        _roomData.LobbyID = CSteamID.Nil;
        _roomData.Name = roomName;
        _roomData.Password = pwd;
        _roomData.MaxMembers = 2;
        _roomData.MemberNums = 1;
        _roomData.CastleIndex = 0;
        _roomData.State = GameReadyState.Preparing;
        _globalVariable.SetRoomData(_roomData);
        SteamAPICall_t hCreateLobby = SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, _roomData.MaxMembers);
        _onLobbyCreated.Set(hCreateLobby);
    }

    /// <summary>
    /// steamのロビーが作ったら
    /// </summary>
    /// <param name="result"></param>
    /// <param name="bIOFailure"></param>
    private void OnLobbyCreated(LobbyCreated_t result, bool bIOFailure)
    {
        //ロビー作成成功していなかった場合
        if (result.m_eResult != EResult.k_EResultOK || bIOFailure)
        {
            Debug.LogError($"Create Lobby Failed: {result.m_eResult}");
            return;
        }

        CSteamID steamID = new CSteamID(result.m_ulSteamIDLobby);
        SetRoomInfo(steamID);
        SetMemberInfo(steamID);

        NetworkManager.Singleton.ConnectionApprovalCallback = ApprovalCheck;

        _globalVariable.SetRoomRole(MultiRoleType.Host);
        NetworkManager.Singleton.StartHost();
        NetworkManager.Singleton.SceneManager.LoadScene("Room", LoadSceneMode.Single);
    }

    /// <summary>
    /// 部屋の情報を設定
    /// </summary>
    private void SetRoomInfo(CSteamID LobbyID)
    {
        //ホストのアドレス（SteamID）を登録
        SteamMatchmaking.SetLobbyData(LobbyID, RoomParams.HOST_ADDRESS_KEY, SteamUser.GetSteamID().ToString());
        SteamMatchmaking.SetLobbyData(LobbyID, RoomParams.GAME_ID_KEY, RoomParams.GAME_ID_VALUE);
        SteamMatchmaking.SetLobbyData(LobbyID, RoomParams.VERSION_KEY, RoomParams.VERSION_VALUE);
        SteamMatchmaking.SetLobbyData(LobbyID, RoomParams.ROOM_NAME_KEY, _roomData.Name);
        string hasPwd = _roomData.Password.Length != 0 ? "1" : "0";
        SteamMatchmaking.SetLobbyData(LobbyID, RoomParams.ROOM_PASSWORD_KEY, hasPwd);
        SteamMatchmaking.SetLobbyData(LobbyID, RoomParams.ROOM_LEADER_KEY, _globalVariable.GetMyName());
    }

    /// <summary>
    /// メンバー情報を設定
    /// </summary>
    /// <param name="LobbyID"></param>
    private void SetMemberInfo(CSteamID LobbyID)
    {
        SteamMatchmaking.SetLobbyMemberData(LobbyID, RoomParams.MEMBER_NAME_KEY, _globalVariable.GetMyName());
        SteamMatchmaking.SetLobbyMemberData(LobbyID, RoomParams.MEMBER_CASTLE_KEY, _roomData.CastleIndex.ToString());
        SteamMatchmaking.SetLobbyMemberData(LobbyID, RoomParams.MEMBER_STATE_KEY, ((int)_roomData.State).ToString());
    }

    /// <summary>
    /// 接続承認チェック
    /// </summary>
    /// <param name="request"></param>
    /// <param name="response"></param>
    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        // ホストの場合は自動承認
        if (request.ClientNetworkId == NetworkManager.Singleton.LocalClientId)
        {
            response.Approved = true;
            return;
        }

        // パスワードがない場合は自動承認
        string password = _globalVariable.GetRoomData().Password;
        if (string.IsNullOrEmpty(password))
        {
            response.Approved = true;
            return;
        }

        string payload = Encoding.UTF8.GetString(request.Payload);
        response.Approved = payload == password;
    }
}
