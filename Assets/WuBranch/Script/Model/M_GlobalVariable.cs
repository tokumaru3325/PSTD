using Steamworks;
using UnityEngine;

public class M_GlobalVariable
{
    public MultiRoleType MultiRole { get; private set; }

    /// <summary>
    /// プレイヤーの名前
    /// </summary>
    public string MyName { get; private set; }

    /// <summary>
    /// マルチモードのルームID
    /// </summary>
    /// <value></value>
    public CSteamID RoomID { get; private set; }

    public M_GlobalVariable()
    {
        MultiRole = MultiRoleType.None;
        MyName = "";
        RoomID = CSteamID.Nil;
    }

    /// <summary>
    /// マルチの役割を設定
    /// </summary>
    /// <param name="role">モード</param>
    public void SetRole(MultiRoleType role)
    {
        MultiRole = role;
    }

    /// <summary>
    /// プレイヤーの名前を設定
    /// </summary>
    /// <param name="name">名前</param>
    public void SetMyName(string name)
    {
        MyName = name;
    }

    /// <summary>
    /// ルームIDを設定
    /// </summary>
    /// <param name="ID">ルームID</param>
    public void SetRoomID(CSteamID ID)
    {
        RoomID = ID;
    }
}
