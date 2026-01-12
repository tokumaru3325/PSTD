using Unity.Netcode;
using UnityEngine;

public class C_GameManager : NetworkBehaviour
{
    /// <summary>
    /// ゲーム状態管理者
    /// </summary>
    private C_GameStateManager _gameStateManager;

    private C_PlayerManager _playerManager;

    void Awake()
    {
        Debug.LogError($"GameManager Awake, server {IsServer}");
        _gameStateManager = FindFirstObjectByType<C_GameStateManager>();
        if (!_gameStateManager)
            Debug.LogError("Didnot find GameStateManager!");

        _playerManager = FindFirstObjectByType<C_PlayerManager>();
        if (!_playerManager)
            Debug.LogError("Didnot find PlayerManager!");
    }


    public override void OnNetworkSpawn()
    {
        Debug.LogError($"GameManager OnNetworkSpawn, server {IsServer}");
        if (IsServer)
        {
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
