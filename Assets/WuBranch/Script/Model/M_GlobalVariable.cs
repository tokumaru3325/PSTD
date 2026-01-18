using System.Collections.Generic;
using Steamworks;
using UnityEngine;

public class M_GlobalVariable
{
    /// <summary>
    /// マルチモードの役割
    /// </summary>
    /// <value></value>
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

    /// <summary>
    /// 選択された城のタイプ一覧
    /// </summary>
    /// <value></value>
    public Dictionary<CSteamID, CastleType> SelectedCastles { get; private set; }

    public M_GlobalVariable()
    {
        MultiRole = MultiRoleType.None;
        MyName = "";
        RoomID = CSteamID.Nil;
        SelectedCastles = new Dictionary<CSteamID, CastleType>();
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

    /// <summary>
    /// プレイヤーが選択した城のタイプを追加または更新
    /// </summary>
    /// <param name="playerID">プレイヤーID</param>
    /// <param name="castleType">城のタイプ</param>
    public void AddPlayerSelectedCastle(CSteamID playerID, CastleType castleType)
    {
        if (SelectedCastles.ContainsKey(playerID))
        {
            SelectedCastles[playerID] = castleType;
        }
        else
        {
            SelectedCastles.Add(playerID, castleType);
        }
    }

    /// <summary>
    /// プレイヤーの選択した城のタイプを削除
    /// </summary>
    /// <param name="playerID">プレイヤーID</param>
    public void DelectedSelectedCastle(CSteamID playerID)
    {
        if (SelectedCastles.ContainsKey(playerID))
        {
            SelectedCastles.Remove(playerID);
        }
    }

    /// <summary>
    /// 選択された城のタイプ一覧をクリア
    /// </summary>
    public void ClearSelectedCastles()
    {
        SelectedCastles.Clear();
    }
}
