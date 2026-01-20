using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Netcode.Transports;
using Steamworks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class C_RoomManager : MonoBehaviour
{
    /// <summary>
    /// ロビー作成コールバック
    /// </summary>
    private CallResult<LobbyCreated_t> _onLobbyCreated;

    /// <summary>
    /// 入室結果受信
    /// </summary>
    public Action<bool, string> OnJoinResultRecieved;

    /// <summary>
    /// ロビー入室コールバック
    /// </summary>
    private Callback<LobbyEnter_t> _lobbyEnter;

    /// <summary>
    /// 共通変数
    /// </summary>
    private C_GlobalVariable _globalVariable;

    /// <summary>
    /// 部屋の情報
    /// </summary>
    private M_RoomData _roomData;

    /// <summary>
    /// ゲームマネージャー
    /// </summary>
    [SerializeField]
    private NetworkObject _gameManagerPrefab;

    void Awake()
    {
        if (FindObjectsByType<C_RoomManager>(FindObjectsSortMode.None).Length > 1)
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
            _lobbyEnter = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
        }

        _globalVariable = FindFirstObjectByType<C_GlobalVariable>();
        if (!_globalVariable)
            Debug.LogError("Didnot find GlobalVariable!");
    }

    /// <summary>
    /// ロビー作成（ホストになる）
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
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += OnEnterSceneCompleted;
        //NetworkManager.Singleton.SceneManager.ActiveSceneSynchronizationEnabled
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

        response.CreatePlayerObject = false;

        // ホストの場合は自動承認
        if (request.ClientNetworkId == NetworkManager.Singleton.LocalClientId)
        {
            response.Approved = true;
            response.Pending = false;
            return;
        }

        // 先に通れない条件を確認、全部確認したら承認する
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

        // すべての承認手順を経て、通りました
        response.Approved = true;
        response.Pending = false;
    }

    /// <summary>
    /// ロビーに参加
    /// </summary>
    /// <param name="password">パスワード</param>
    public void JoinLobby(CSteamID lobbyID, string password = "")
    {
        if (password.Length > 0)
        {
            // パスワードの準備
            byte[] passwordData = Encoding.UTF8.GetBytes(password);
            NetworkManager.Singleton.NetworkConfig.ConnectionData = passwordData;
        }
        // ロビーに参加
        SteamMatchmaking.JoinLobby(lobbyID);
    }

    /// <summary>
    /// ロビー入室コールバック用関数
    /// </summary>
    /// <param name="callback"></param>
    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        Debug.LogError($"On Lobby Entered");
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
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
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
    /// クライアントが切断したとき
    /// </summary>
    private void OnClientDisconnect(ulong clientId)
    {
        //クライアント切断コールバック
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnect;
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= OnEnterSceneCompleted;
        //ネットワークマネージャーを破棄（これで新しくNetworkManagerを作る（使う）ことができる）
        NetworkManager.Singleton.Shutdown();
        //メインシーンに戻る
        SceneManager.LoadScene("RoomList");
    }

    /// <summary>
    /// シーンに入ったときの処理
    /// </summary>
    /// <param name="sceneName">シーン名</param>
    /// <param name="loadSceneMode">ロードモード</param>
    /// <param name="clientsCompleted">完了したクライアント</param>
    /// <param name="clientsTimedOut">タイムアウトしたクライアント</param>
    private async void OnEnterSceneCompleted(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        if (sceneName != "MultiGame")
            return;
        Debug.LogError($"Enter Game Scene completed {clientsCompleted.Count}");
        // ゲームシーンに入ったときの処理
        if (clientsCompleted.Count == NetworkManager.Singleton.ConnectedClients.Count)
        {
            // 全クライアントがシーンに入った, サーバでマネージャーを生成
            C_GameManager gameManager;
            C_GameStateManager gameStateManager;
            C_PlayerManager playerManager;
            SpawnManagers(out gameManager, out gameStateManager, out playerManager);

            //gameManager.Initialize(gameStateManager, playerManager);

        }
    }

    /// <summary>
    /// マネージャーたちを生成
    /// </summary>
    private void SpawnGameManager()
    {
        // リストからプレハブをゲット
        NetworkPrefabsList prefabs = NetworkManager.Singleton.NetworkConfig.Prefabs.NetworkPrefabsLists.Find(list => list.name == "GameSceneNetworkPrefabsList");
        Debug.LogError($"Spawn Game Manager {prefabs.PrefabList.Count}");
        NetworkPrefab prefab = prefabs.PrefabList.First(p => p.Prefab.name == "GameManager");
        Debug.LogError($"{prefab}");
        if (prefab.Prefab.GetComponent<NetworkObject>())
            Debug.LogError("have net");
        GameObject targetObj = Instantiate(prefab.Prefab);
        NetworkObject networkObj = targetObj.GetComponent<NetworkObject>();
        networkObj.Spawn();

        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            bool isObserver = networkObj.IsNetworkVisibleTo(client.ClientId);
            Debug.LogError($"Client {client.ClientId} is observer? : {isObserver}");

            if (!isObserver)
            {
                // 強制加入試試看，這能確認是否為 Visibility 問題
                networkObj.NetworkShow(client.ClientId);
            }
        }
    }

    /// <summary>
    /// マネージャーたちを生成
    /// </summary>
    private void SpawnManagers(out C_GameManager gameManager, out C_GameStateManager gameStateManager, out C_PlayerManager playerManager)
    {
        // リストからプレハブをゲット
        NetworkPrefabsList prefabs = NetworkManager.Singleton.NetworkConfig.Prefabs.NetworkPrefabsLists.Find(list => list.name == "GameSceneNetworkPrefabsList");
        Dictionary<string, GameObject> prefabMap = prefabs.PrefabList.ToDictionary(p => p.Prefab.name, p => p.Prefab);

        // それぞれのプレハブを生成
        gameManager = SpawnManager(prefabMap, "GameManager").GetComponent<C_GameManager>();
        gameStateManager = SpawnManager(prefabMap, "GameStateManager").GetComponent<C_GameStateManager>();
        playerManager = SpawnManager(prefabMap, "PlayerManager").GetComponent<C_PlayerManager>();
    }

    /// <summary>
    /// 指定されたオブジェクトを生成
    /// </summary>
    /// <param name="map">プレハブマップ</param>
    /// <param name="name">指定されたオブジェクトのID</param>
    private GameObject SpawnManager(Dictionary<string, GameObject> map, string name)
    {
        if (map.TryGetValue(name, out GameObject target))
        {
            GameObject targetObj = Instantiate(target);
            targetObj.GetComponent<NetworkObject>().Spawn();
            return targetObj;
        }
        return null;
    }
}
