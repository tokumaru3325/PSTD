using System;
using System.Collections.Generic;
using System.Linq;
using Steamworks;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// 城タイプのプレハブ
/// </summary>
[Serializable]
public struct TowerPrefabInfo
{
    /// <summary>
    /// タワーの種類
    /// </summary>
    [SerializeField]
    public CastleType type;

    /// <summary>
    /// タワープレハブ
    /// </summary>
    [SerializeField]
    public NetworkObject prefab;
}

/// <summary>
/// マルチの時各キャラクターのタグ
/// </summary>
[Serializable]
public struct MultiPlayerTags
{
    /// <summary>
    /// マルチのキャラ
    /// </summary>
    [SerializeField]
    public MultiRoleType role;

    /// <summary>
    /// タグ
    /// </summary>
    [SerializeField]
    public string tag;
}

public class C_PlayerManager : NetworkBehaviour
{
    /// <summary>
    /// プレイヤープレハブ
    /// </summary>
    [SerializeField]
    private TowerPrefabInfo[] _playerPrefabs;

    /// <summary>
    /// プレイヤーインスタンスリスト
    /// </summary>
    private Dictionary<ulong, NetworkObject> _playerInstances;

    /// <summary>
    /// プレイヤーSteamIDリスト
    /// </summary>
    private Dictionary<ulong, CSteamID> _playerSteamIDs;

    /// <summary>
    /// 共通変数
    /// </summary>
    private C_GlobalVariable _globalVariable;

    /// <summary>
    /// マップ管理者
    /// </summary>
    private C_MapManager _mapManager;

    /// <summary>
    /// サーバプレイヤーのタグ
    /// </summary>
    [SerializeField]
    private MultiPlayerTags[] _multiPlayerTags;

    void Awake()
    {
        Debug.LogError($"PlayerManager Awake, server {IsServer}");
        _globalVariable = FindFirstObjectByType<C_GlobalVariable>();
        if (!_globalVariable)
            Debug.LogError("Didnot find GlobalVariable!");
        _playerInstances = new Dictionary<ulong, NetworkObject>();
        _playerSteamIDs = new Dictionary<ulong, CSteamID>();
        _mapManager = FindFirstObjectByType<C_MapManager>();
        if (!_mapManager)
            Debug.LogError("Didnot find MapManager!");
    }

    public override void OnNetworkSpawn()
    {
        Debug.LogError($"PlayerManager On network spawn, server {IsServer}");
        Initialize();
    }

    /// <summary>
    /// 初期化
    /// </summary>
    private void Initialize()
    {
        InitMyPlayerID();
    }

    /// <summary>
    /// すべてのクライアントに自分のプレイヤーID初期化
    /// </summary>
    private void InitMyPlayerID()
    {
        ulong playerID = NetworkManager.Singleton.LocalClientId;
        ulong steamID = SteamUser.GetSteamID().m_SteamID;
        Debug.LogError($"client, Init my playerID: {playerID}, steamID: {steamID}");
        AddPlayerIDRpc(playerID, steamID);
    }

    /// <summary>
    /// サーバでプレイヤーID追加
    /// </summary>
    /// <param name="playerID">netCodeのプレイヤID</param>
    /// <param name="steamID">steamID</param>
    [Rpc(SendTo.Server)]
    private void AddPlayerIDRpc(ulong playerID, ulong steamID)
    {
        Debug.LogError($"server, Add playerID: {playerID}, steamID: {steamID}");
        if (!_playerSteamIDs.ContainsKey(playerID))
        {
            Debug.LogError($"server, first add playerID: {playerID}");
            _playerSteamIDs.Add(playerID, new CSteamID(steamID));
        }
    }

    /// <summary>
    /// サーバで全プレイヤー生成
    /// </summary>
    [Rpc(SendTo.Server)]
    public void SpawnAllPlayersRpc()
    {
        if (!_globalVariable)
        {
            Debug.LogError("Didnot find GlobalVariable!");
            return;
        }

        ulong serverID = NetworkManager.ServerClientId;
        // プレイヤーを全員生成
        foreach (ulong ID in NetworkManager.Singleton.ConnectedClientsIds)
        {
            Debug.Log($"create ID's player: {ID}");
            if (_playerSteamIDs.ContainsKey(ID))
            {
                CSteamID steamID = _playerSteamIDs[ID];
                CastleType selectedCastle = _globalVariable.GetPlayerCastle(steamID);
                bool isServer = ID == serverID;
                Debug.LogError($"Get player {ID} castle type: {selectedCastle}");
                _playerInstances.Add(ID, CreatePlayerInstance(ID, selectedCastle, isServer));
            }
        }
    }

    /// <summary>
    /// プレイヤーインスタンス生成
    /// </summary>
    /// <param name="ownerID">所有者ID</param>
    /// <param name="castleType">選んだ城</param>
    /// <param name="isServer">サーバか</param>
    /// <returns>プレイヤーインスタンス</returns>
    private NetworkObject CreatePlayerInstance(ulong ownerID, CastleType castleType, bool isServer)
    {
        // プレイヤープレハブ取得
        NetworkObject _playerPrefab = GetPlayerPrefab(castleType);
        if (!_playerPrefab)
        {
            Debug.LogError($"Didnot find player prefab of castle type: {castleType}");
            return null;
        }

        // プレイヤーインスタンス生成
        NetworkObject playerInstance = Instantiate(_playerPrefab);
        if (_mapManager)
        {
            M_PlayerPosInfo playerPos = _mapManager.UseOnePlayerPos();
            playerInstance.transform.position = _mapManager.ConvertToUnityPos(playerPos.MapPos);
            playerInstance.transform.localRotation = playerPos.Dir;
        }
        else
            playerInstance.transform.position = Vector3.zero;
        playerInstance.tag = isServer ? GetMyTag(MultiRoleType.Host) : GetMyTag(MultiRoleType.Client);
        playerInstance.SpawnAsPlayerObject(ownerID, true);
        playerInstance.transform.SetParent(this.transform);
        return playerInstance;
    }

    /// <summary>
    /// プレイヤープレハブ取得
    /// </summary>
    /// <param name="type">城の種類</param>
    /// <returns>プレイヤープレハブ</returns>
    private NetworkObject GetPlayerPrefab(CastleType type)
    {
        for (int i = 0; i < _playerPrefabs.Length; i++)
        {
            if (_playerPrefabs[i].type == type)
            {
                return _playerPrefabs[i].prefab;
            }
        }
        return null;
    }

    /// <summary>
    /// キャラクターによって、タグをゲット
    /// </summary>
    /// <param name="role">キャラクター</param>
    /// <returns>タグ</returns>
    public string GetMyTag(MultiRoleType role)
    {
        var tags = _multiPlayerTags.Where(_ => _.role == role);
        return tags.FirstOrDefault().tag;
    }
}
