using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class C_GameManager : NetworkBehaviour
{

    /// <summary>
    /// ゲームステートマネージャー
    /// </summary>
    [SerializeField]
    private NetworkObject _gameStateManagerPrefab;

    /// <summary>
    /// プレイヤーマネージャー
    /// </summary>
    [SerializeField]
    private NetworkObject _playerManagerPrefab;

    /// <summary>
    /// ゲーム状態管理者
    /// </summary>
    private C_GameStateManager _gameStateManager;

    /// <summary>
    /// プレイヤ管理者
    /// </summary>
    private C_PlayerManager _playerManager;

    void Awake()
    {
        Debug.LogError($"GameManager Awake, server {IsServer}");
    }


    public override void OnNetworkSpawn()
    {
        Debug.LogError($"GameManager OnNetworkSpawn, server {IsServer}");
        if (IsServer)
        {
            SpawnManagers();
            Initialize();
            //_gameStateManager.ChangeStateRpc(GameStates.Prepare);
        }
    }

    /// <summary>
    /// マネージャーたちを生成
    /// </summary>
    private void SpawnManagers()
    {
        // リストからプレハブをゲット
        NetworkPrefabsList prefabs = NetworkManager.Singleton.NetworkConfig.Prefabs.NetworkPrefabsLists.Find(list => list.name == "GameSceneNetworkPrefabsList");
        Dictionary<string, GameObject> prefabMap = prefabs.PrefabList.ToDictionary(p => p.Prefab.name, p => p.Prefab);

        // それぞれのプレハブを生成
        _gameStateManager = SpawnManager(prefabMap, "GameStateManager").GetComponent<C_GameStateManager>();
        _playerManager = SpawnManager(prefabMap, "PlayerManager").GetComponent<C_PlayerManager>();
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

    /// <summary>
    /// 初期化
    /// </summary>
    private void Initialize()
    {
        if (IsServer)
        {
            // 初期化
            if (_gameStateManager)
            {
                _gameStateManager.OnGameStateChanged += OnGameStateChanged;
            }
        }
    }

    /// <summary>
    /// ゲーム状態変更時の処理
    /// </summary>
    /// <param name="newState">新しい状態</param>
    private void OnGameStateChanged(GameStates newState)
    {
        if (newState == GameStates.Prepare)
        {
            if (_playerManager)
            {
                Debug.LogError("Start spawn all players");
                _playerManager.SpawnAllPlayersRpc();
            }
        }
    }
}
