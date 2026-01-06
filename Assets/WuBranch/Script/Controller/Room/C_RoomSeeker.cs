using UnityEngine;
using Steamworks;
using System;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class C_RoomSeeker : MonoBehaviour
{
    /// <summary>
    /// ロビーが見つかった
    /// </summary>
    private CallResult<LobbyMatchList_t> _onLobbyMatched;

    /// <summary>
    /// ロビーが見つかった通知, 全部の受付データ
    /// </summary>
    public Action<List<M_RoomFrontData>> OnFoundRoomData;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //SteamManagerの初期化が完了していたら
        if (SteamManager.Initialized)
        {
            FindRoom();
        }
    }

    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.T))
        // {
        //     M_RoomFrontData fakeData = new M_RoomFrontData(CSteamID.Nil, "TestRoom", "1", "Leader", 2, 1);
        //     OnFoundRoomData?.Invoke(fakeData);
        // }
    }

    /// <summary>
    /// 部屋を探す
    /// </summary>
    public void FindRoom(string condition = "")
    {
        _onLobbyMatched = CallResult<LobbyMatchList_t>.Create(OnMatchLobby);
        SteamMatchmaking.AddRequestLobbyListResultCountFilter(RoomParams.MAX_LOBBY_COUNT);
        SteamMatchmaking.AddRequestLobbyListStringFilter(RoomParams.GAME_ID_KEY, RoomParams.GAME_ID_VALUE, ELobbyComparison.k_ELobbyComparisonEqual);
        SteamMatchmaking.AddRequestLobbyListStringFilter(RoomParams.VERSION_KEY, RoomParams.VERSION_VALUE, ELobbyComparison.k_ELobbyComparisonEqual);
        if (condition != "")
            SteamMatchmaking.AddRequestLobbyListStringFilter(RoomParams.ROOM_NAME_KEY, condition, ELobbyComparison.k_ELobbyComparisonEqualToOrGreaterThan);
        SteamAPICall_t hSteamAPICall = SteamMatchmaking.RequestLobbyList();
        _onLobbyMatched.Set(hSteamAPICall);
    }

    /// <summary>
    /// ロビーが見つかったときの処理
    /// </summary>
    /// <param name="param">ロビーのデータ</param>
    /// <param name="bIOFailure"></param>
    private void OnMatchLobby(LobbyMatchList_t param, bool bIOFailure)
    {
        uint lobbyCount = param.m_nLobbiesMatching;
        List<M_RoomFrontData> roomDatas = new();
        for (int index = 0; index < lobbyCount; index++)
        {
            M_RoomFrontData data = CollectRoomFrontData(index);
            roomDatas.Add(data);
        }
        OnFoundRoomData?.Invoke(roomDatas);
    }

    /// <summary>
    /// 部屋の情報を集め
    /// </summary>
    /// <param name="lobbyIndex">部屋のインデックス</param>
    private M_RoomFrontData CollectRoomFrontData(int lobbyIndex)
    {
        // ロビーIDをゲット
        CSteamID lobbyID = SteamMatchmaking.GetLobbyByIndex(lobbyIndex);
        // 指定のロビーに設定されたメタデータキーの数を取得
        string roomName = SteamMatchmaking.GetLobbyData(lobbyID, RoomParams.ROOM_NAME_KEY);
        string roomPassword = SteamMatchmaking.GetLobbyData(lobbyID, RoomParams.ROOM_PASSWORD_KEY);
        string roomLeader = SteamMatchmaking.GetLobbyData(lobbyID, RoomParams.ROOM_LEADER_KEY);
        int membersNum = SteamMatchmaking.GetNumLobbyMembers(lobbyID);
        int maxMembers = SteamMatchmaking.GetLobbyMemberLimit(lobbyID);
        return new M_RoomFrontData(lobbyID, roomName, roomPassword, roomLeader, maxMembers, membersNum);
    }
}
