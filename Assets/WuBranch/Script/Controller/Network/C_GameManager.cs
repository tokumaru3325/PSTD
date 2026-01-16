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
            //_gameStateManager.ChangeStateRpc(GameStates.Prepare);
        }
    }

    /// <summary>
    /// 初期化
    /// </summary>
    public void Initialize(C_GameStateManager stateManager, C_PlayerManager playerManager)
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
