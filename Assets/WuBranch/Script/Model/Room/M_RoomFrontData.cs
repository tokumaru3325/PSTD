using Steamworks;
using UnityEngine;


public class M_RoomFrontData
{
    /// <summary>
    /// 部屋ID
    /// </summary>
    public CSteamID LobbyID { get; private set; }

    /// <summary>
    /// 部屋名
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// パスワードはあるか
    /// </summary>
    public bool HavePwd { get; private set; }

    /// <summary>
    /// 部屋主の名前
    /// </summary>
    public string LeaderName { get; private set; }

    /// <summary>
    /// 人数上限
    /// </summary>
    public int MaxMembers { get; private set; }

    /// <summary>
    /// 今の人数
    /// </summary>
    public int MemberNums { get; private set; }

    public M_RoomFrontData(CSteamID ID, string name, string pwd, string leader, int maxMembers, int memberNum)
    {
        LobbyID = ID;
        Name = name;
        HavePwd = pwd.Equals("1");
        LeaderName = leader;
        MaxMembers = maxMembers;
        MemberNums = memberNum;
    }
}
