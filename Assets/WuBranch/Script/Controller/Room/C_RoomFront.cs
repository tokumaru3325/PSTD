using UnityEngine;
using Steamworks;
using System;
using Unity.Netcode;
using Netcode.Transports;
using System.Text;
using UnityEngine.SceneManagement;

public class C_RoomFront : MonoBehaviour
{
    /// <summary>
    /// 初期化完了
    /// </summary>
    public Action OnInitedData;

    /// <summary>
    /// 入室結果受信
    /// </summary>
    public Action<bool, string> OnJoinResultRecieved;

    /// <summary>
    /// 部屋の情報
    /// </summary>
    private M_RoomFrontData _data;

    //ロビー入室コールバック
    private Callback<LobbyEnter_t> _lobbyEnter;

    /// <summary>
    /// 共通変数
    /// </summary>
    private C_GlobalVariable _globalVariable;

    void Start()
    {
        if (SteamManager.Initialized)
        {
            _lobbyEnter = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
        }
        _globalVariable = FindFirstObjectByType<C_GlobalVariable>();
    }

    /// <summary>
    /// 初期化
    /// </summary>
    /// <param name="roomID">部屋ID</param>
    /// <param name="roomName">部屋名</param>
    /// <param name="password">パスワード</param>
    /// <param name="leaderName">部屋主</param>
    /// <param name="maxMembers">人数上限</param>
    /// <param name="memberNums">人数</param>
    public void Init(CSteamID roomID, string roomName, string password, string leaderName, int maxMembers, int memberNums)
    {
        _data = new M_RoomFrontData(roomID, roomName, password, leaderName, maxMembers, memberNums);
        OnInitedData?.Invoke();
    }

    public void Init(M_RoomFrontData data)
    {
        _data = data;
        OnInitedData?.Invoke();
    }

    /// <summary>
    /// 持っているデータを取得
    /// </summary>
    /// <returns>データ</returns>
    public M_RoomFrontData GetData()
    {
        return _data;
    }

    /// <summary>
    /// ロビーに参加
    /// </summary>
    /// <param name="password">パスワード</param>
    public void JoinLobby(string password = "")
    {
        if (password.Length > 0)
        {
            // パスワードの準備
            byte[] passwordData = Encoding.UTF8.GetBytes(password);
            NetworkManager.Singleton.NetworkConfig.ConnectionData = passwordData;
        }
        // ロビーに参加
        SteamMatchmaking.JoinLobby(_data.LobbyID);
    }

    /// <summary>
    /// ロビー入室コールバック用関数
    /// </summary>
    /// <param name="callback"></param>
    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        //Debug.LogError($"On Lobby Entered");
        //入室失敗時
        EChatRoomEnterResponse response = (EChatRoomEnterResponse)callback.m_EChatRoomEnterResponse;
        if (response != EChatRoomEnterResponse.k_EChatRoomEnterResponseSuccess)
        {
            OnJoinResultRecieved?.Invoke(false, ConvertErrResultToString(response));
            return;
        }

        //ホストのSteamIDを取得
        CSteamID steamID = new CSteamID(callback.m_ulSteamIDLobby);
        string hostAddress = SteamMatchmaking.GetLobbyData(
            steamID,
            RoomParams.HOST_ADDRESS_KEY);

        // ホストもここを通るのでクライアント接続しないように
        // 同じPCでテストすると、ここをコメントアウトする
        if (hostAddress == SteamUser.GetSteamID().ToString())
            return;

        _globalVariable.SetRoomID(steamID);
        _globalVariable.SetRoomRole(MultiRoleType.Client);
        InitMyInfo(steamID);

        //Netcodeでクライアント接続
        var stp = (SteamNetworkingSocketsTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport;
        stp.ConnectToSteamID = ulong.Parse(hostAddress);

        //ホストに接続
        bool result = NetworkManager.Singleton.StartClient();
        OnJoinResultRecieved?.Invoke(true, "");

        //シーンを切り替え
        SceneManager.LoadScene("Room", LoadSceneMode.Single);
        //NetworkManager.Singleton.SceneManager.LoadScene("Room", LoadSceneMode.Single);

        //切断時
        //NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
    }

    /// <summary>
    /// 参加結果を文字に変換
    /// </summary>
    /// <param name="result">参加結果</param>
    /// <returns>参加結果文字列</returns>
    private string ConvertErrResultToString(EChatRoomEnterResponse result)
    {
        switch (result)
        {
            case EChatRoomEnterResponse.k_EChatRoomEnterResponseDoesntExist:
                return "ロビーが存在しません。";
            case EChatRoomEnterResponse.k_EChatRoomEnterResponseFull:
                return "ロビーが満員です。";
            case EChatRoomEnterResponse.k_EChatRoomEnterResponseNotAllowed:
                return "パスワードが違います。";
            default:
                return "予期しないエラーが発生しました。";
        }
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
}
