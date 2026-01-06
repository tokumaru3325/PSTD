using System.Collections.Generic;
using Steamworks;
using UnityEngine;

public struct M_RoomData
{
    /// <summary>
    /// 部屋ID
    /// </summary>
    public CSteamID LobbyID;

    /// <summary>
    /// 部屋名
    /// </summary>
    public string Name;

    /// <summary>
    /// パスワード
    /// </summary>
    public string Password;

    /// <summary>
    /// 人数上限
    /// </summary>
    public int MaxMembers;

    /// <summary>
    /// 今の人数
    /// </summary>
    public int MemberNums;

}
