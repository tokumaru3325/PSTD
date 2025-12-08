using UnityEngine;
using Steamworks;
using System;
using UnityEngine.UIElements;

public class C_RoomSeeker : MonoBehaviour
{
    /// <summary>
    /// ロビーが見つかった
    /// </summary>
    private CallResult<LobbyMatchList_t> _onLobbyMatched;

    public Action<M_RoomFrontData> OnFoundRoomData;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //SteamManagerの初期化が完了していたら
        if (SteamManager.Initialized)
        {
            _onLobbyMatched = CallResult<LobbyMatchList_t>.Create(OnMatchLobby);
            SteamMatchmaking.AddRequestLobbyListResultCountFilter(RoomParams.MAX_LOBBY_COUNT);
            SteamMatchmaking.AddRequestLobbyListStringFilter(RoomParams.GAME_ID_KEY, RoomParams.GAME_ID_VALUE, ELobbyComparison.k_ELobbyComparisonEqual);
            SteamMatchmaking.AddRequestLobbyListStringFilter(RoomParams.VERSION_KEY, RoomParams.VERSION_VALUE, ELobbyComparison.k_ELobbyComparisonEqual);
            SteamAPICall_t hSteamAPICall = SteamMatchmaking.RequestLobbyList();
            _onLobbyMatched.Set(hSteamAPICall);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnMatchLobby(LobbyMatchList_t param, bool bIOFailure)
    {
        uint lobbyCount = param.m_nLobbiesMatching;
        for (int index = 0; index < lobbyCount; index++)
        {
            M_RoomFrontData data = CollectRoomFrontData(index);
            OnFoundRoomData?.Invoke(data);
        }
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
