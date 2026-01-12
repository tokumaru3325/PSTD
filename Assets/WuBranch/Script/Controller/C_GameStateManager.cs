using System;
using Unity.Netcode;
using UnityEngine;

public class C_GameStateManager : NetworkBehaviour
{
    /// <summary>
    /// 現在のゲーム状態
    /// </summary>
    private NetworkVariable<GameStates> _currentState;

    /// <summary>
    /// ゲーム状態変更イベント
    /// </summary>
    public event Action<GameStates> OnGameStateChanged;

    /// <summary>
    /// ゲーム状態終了イベント
    /// </summary>
    public event Action<GameStates> OnGameStateEnded;

    void Awake()
    {
        Debug.LogError($"GameStateManager Awake, server {IsServer}");
        _currentState = new NetworkVariable<GameStates>(GameStates.Wait);
    }

    public override void OnNetworkSpawn()
    {
        Debug.LogError($"GameStateManager OnNetworkSpawn, server {IsServer}");
        if (IsClient && !IsServer)
        {
            _currentState.OnValueChanged += OnClientGameStateChanged;
        }
    }

    /// <summary>
    /// サーバでゲーム状態変更
    /// </summary>
    /// <param name="newState">ゲーム状態</param>
    [Rpc(SendTo.Server)]
    public void ChangeStateRpc(GameStates newState)
    {
        Debug.LogError($"change state from {_currentState.Value} to {newState}");
        NotifyStateEnded(_currentState.Value);
        _currentState.Value = newState;
        NotifyStateChanged(newState);
    }

    /// <summary>
    /// サーバでゲーム状態終了通知
    /// </summary>
    private void NotifyStateEnded(GameStates curState)
    {
        OnGameStateEnded?.Invoke(curState);
    }

    /// <summary>
    /// サーバでゲーム状態変更通知
    /// </summary>
    private void NotifyStateChanged(GameStates newState)
    {
        OnGameStateChanged?.Invoke(newState);
    }

    /// <summary>
    /// クライアントでゲーム状態変更通知
    /// </summary>
    /// <param name="oldState"></param>
    /// <param name="newState"></param>
    private void OnClientGameStateChanged(GameStates oldState, GameStates newState)
    {
        NotifyStateEnded(oldState);
        NotifyStateChanged(newState);
    }
}
