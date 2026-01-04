using UnityEngine;

public struct M_RoomPlayerData
{
    /// <summary>
    /// 選んだ城
    /// </summary>
    public CastleType CastleIndex;

    /// <summary>
    /// プレイヤーの状態
    /// <ID, 状態>
    /// </summary>
    public GameReadyState State;
}
