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
    /// 部屋の情報
    /// </summary>
    private M_RoomFrontData _data;

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
}
