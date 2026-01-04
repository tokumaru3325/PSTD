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
    /// <param name="roomName">名前</param>
    /// <param name="pwd">パスワード</param>
    public void CreateLobby(string roomName, string pwd)
    {
        InitRoom(roomName, pwd);
        SteamAPICall_t hCreateLobby = SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, _roomData.MaxMembers);
        _onLobbyCreated.Set(hCreateLobby);
    }

    /// <summary>
    /// 部屋を初期化
    /// </summary>
    /// <param name="roomName">名前</param>
    /// <param name="pwd">パスワード</param>
    private void InitRoom(string roomName, string pwd)
    {
        _roomData = new M_RoomData();
        _roomData.LobbyID = CSteamID.Nil;
        _roomData.Name = roomName;
        _roomData.Password = pwd;
        _roomData.MaxMembers = 2;
        _roomData.MemberNums = 1;
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
        _globalVariable.SetRoomID(steamID);
        SetRoomInfo(steamID);
        InitMyInfo(steamID);

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
    /// 自身の情報を先に設定
    /// </summary>
    /// <param name="LobbyID"></param>
    private void InitMyInfo(CSteamID LobbyID)
    {
        SteamMatchmaking.SetLobbyMemberData(LobbyID, RoomParams.MEMBER_NAME_KEY, _globalVariable.GetMyName());
        SteamMatchmaking.SetLobbyMemberData(LobbyID, RoomParams.MEMBER_CASTLE_KEY, ((int)CastleType.Castle1).ToString());
        SteamMatchmaking.SetLobbyMemberData(LobbyID, RoomParams.MEMBER_STATE_KEY, ((int)GameReadyState.Preparing).ToString());
    }

    /// <summary>
    /// 接続承認チェック
    /// </summary>
    /// <param name="request"></param>
    /// <param name="response"></param>
    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        // true から false に遷移すると、接続承認応答が処理されます。
        response.Pending = true;

        // ホストの場合は自動承認
        if (request.ClientNetworkId == NetworkManager.Singleton.LocalClientId)
        {
            response.Approved = true;
            response.Pending = false;
            return;
        }

        // パスワードの確認
        string password = _roomData.Password;
        if (!string.IsNullOrEmpty(password))
        {
            string payload = Encoding.UTF8.GetString(request.Payload);
            // パスワードが違う
            if (payload != password)
            {
                // 接続を許可しない
                response.Approved = false;
                response.Pending = false;
                return;
            }
        }

        // 最大人数をチェック
        if (NetworkManager.Singleton.ConnectedClients.Count >= _roomData.MaxMembers)
        {
            // 接続を許可しない
            response.Approved = false;
            response.Pending = false;
            return;
        }

        response.CreatePlayerObject = false;

        // すべての承認手順を経て、通りました
        response.Approved = true;
        response.Pending = false;
    }

    /// <summary>
    /// 部屋情報をゲット
    /// </summary>
    /// <returns></returns>
    public M_RoomData GetRoomData()
    {
        return _roomData;
    }
}
