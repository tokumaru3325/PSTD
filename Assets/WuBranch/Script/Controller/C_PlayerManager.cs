using System;
using System.Collections.Generic;
using Steamworks;
using Unity.Netcode;
using UnityEngine;

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

        // プレイヤーを全員生成
        foreach (ulong ID in NetworkManager.Singleton.ConnectedClientsIds)
        {
            Debug.Log($"create ID's player: {ID}");
            if (_playerSteamIDs.ContainsKey(ID))
            {
                CSteamID steamID = _playerSteamIDs[ID];
                CastleType selectedCastle = _globalVariable.GetPlayerCastle(steamID);
                Debug.LogError($"Get player {ID} castle type: {selectedCastle}");
                _playerInstances.Add(ID, CreatePlayerInstance(ID, selectedCastle));
            }
        }
    }

    /// <summary>
    /// プレイヤーインスタンス生成
    /// </summary>
    /// <param name="ownerID">所有者ID</param>
    /// <returns>プレイヤーインスタンス</returns>
    private NetworkObject CreatePlayerInstance(ulong ownerID, CastleType castleType)
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
}
