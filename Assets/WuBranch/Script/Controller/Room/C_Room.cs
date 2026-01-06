using System;
using System.Collections.Generic;
using Steamworks;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class C_Room : MonoBehaviour
{
    /// <summary>
    /// メンバ情報が変更された通知
    /// </summary>
    public Action<CSteamID, M_RoomPlayerData> OnMemberUpdated;

    /// <summary>
    /// 部屋の情報が変更された通知
    /// </summary>
    public Action<string> OnLobbyUpdated;

    /// <summary>
    /// メンバが入室か退室の通知
    /// </summary>
    public Action<CSteamID, string> OnMemberChanged;

    /// <summary>
    /// 部屋内いるプレイヤ全員の役割
    /// <プレイヤID, 役割>
    /// </summary>
    public Dictionary<CSteamID, MultiRoleType> RoleMap { get; private set; }

    /// <summary>
    /// 共通変数
    /// </summary>
    private C_GlobalVariable _globalVariable;

    /// <summary>
    /// ルームのID
    /// </summary>
    private CSteamID _roomID;

    /// <summary>
    /// 部屋内の情報が変更された(メンバも含める)
    /// </summary>
    private Callback<LobbyDataUpdate_t> _onLobbyDataUpdated;

    /// <summary>
    /// メンバが入室か退室
    /// </summary>
    private Callback<LobbyChatUpdate_t> _onLobbyChatUpdate;

    // 毎回プレイヤーの名前をゲットしたくないので、このリストを使って記録する。
    /// <summary>
    /// プレイヤーの名前をゲットする必要があるリスト
    /// </summary>
    private List<CSteamID> _needGetName;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _needGetName = new List<CSteamID>();
        RoleMap = new Dictionary<CSteamID, MultiRoleType>();
        // 初期化するためのデータが用意したか
        bool bInitResult = false;
        _globalVariable = FindFirstObjectByType<C_GlobalVariable>();
        if (_globalVariable)
        {
            _roomID = _globalVariable.GetRoomID();
            //
            if (_roomID != CSteamID.Nil)
            {
                bInitResult = true;
            }
        }

        if (!bInitResult)
            NetworkManager.Singleton.SceneManager.LoadScene("RoomList", LoadSceneMode.Single);
    }

    void Start()
    {
        if (SteamManager.Initialized)
        {
            _onLobbyDataUpdated = Callback<LobbyDataUpdate_t>.Create(OnLobbyUpdate);
            _onLobbyChatUpdate = Callback<LobbyChatUpdate_t>.Create(OnLobbyChatUpdate);
            InitRoleMap();
            InitRoom();
        }
    }

    /// <summary>
    /// 部屋情報が変更された処理
    /// </summary>
    /// <param name="callbackData">データ</param>
    private void OnLobbyUpdate(LobbyDataUpdate_t callbackData)
    {
        if (Convert.ToBoolean(callbackData.m_bSuccess))
        {
            // この部屋内の情報かどうか確認する
            if (_roomID == new CSteamID(callbackData.m_ulSteamIDLobby))
            {
                // 部屋の情報が変更された
                if (callbackData.m_ulSteamIDLobby == callbackData.m_ulSteamIDMember)
                {
                    GetLobbyData(_roomID);
                }
                // メンバの情報が変更された
                else
                {
                    CSteamID memberID = new CSteamID(callbackData.m_ulSteamIDMember);
                    if (_needGetName.Contains(memberID))
                        GetMemberName(_roomID, memberID);
                    GetMemberData(_roomID, memberID);
                }
            }
        }
    }

    /// <summary>
    /// メンバが入室か退室するの処理
    /// </summary>
    /// <param name="callbackData"></param>
    private void OnLobbyChatUpdate(LobbyChatUpdate_t callbackData)
    {
        CSteamID changedPlayer = new CSteamID(callbackData.m_ulSteamIDUserChanged);
        if (callbackData.m_rgfChatMemberStateChange == (uint)EChatMemberStateChange.k_EChatMemberStateChangeEntered)
        {
            // 参加
            // 役割マップに追加
            RoleMap.Add(changedPlayer, MultiRoleType.Client);
            // クライアントが入室するとき、クライアントが自身の情報を初期化するより、ホスト側が先にOnLobbyChatUpdateを呼ぶ可能性がある
            // そうなると、ずっとクライアントの名前が空白になるため、リストに記録して再び名前をゲットするようにした
            _needGetName.Add(changedPlayer);
            GetMemberName(_roomID, changedPlayer);
            GetMemberData(_roomID, changedPlayer);
        }
        else if (callbackData.m_rgfChatMemberStateChange == (uint)EChatMemberStateChange.k_EChatMemberStateChangeLeft)
        {
            // 退室
            NotifyMemberChange(changedPlayer, "");
            M_RoomPlayerData data = new() { CastleIndex = CastleType.Null, State = GameReadyState.Null };
            NotifyMemberUpdate(changedPlayer, data);
            // 役割マッから削除
            if (RoleMap.ContainsKey(changedPlayer))
                RoleMap.Remove(changedPlayer);
        }
    }

    /// <summary>
    /// プレイヤ全員の役割を初期化
    /// </summary>
    private void InitRoleMap()
    {
        // ホスト
        CSteamID owner = SteamMatchmaking.GetLobbyOwner(_roomID);
        RoleMap.Add(owner, MultiRoleType.Host);
        // クライアント
        int memberNums = SteamMatchmaking.GetNumLobbyMembers(_roomID);
        for (int index = 0; index < memberNums; index++)
        {
            CSteamID player = SteamMatchmaking.GetLobbyMemberByIndex(_roomID, index);
            // ホストだったら無視
            if (player == owner)
                continue;

            RoleMap.Add(player, MultiRoleType.Client);
        }
    }

    /// <summary>
    /// 部屋を初期化
    /// </summary>
    private void InitRoom()
    {
        // 部屋の情報
        GetLobbyData(_roomID);

        // 自身の情報
        //CSteamID myID = SteamUser.GetSteamID();
        //InitSelfInfo(myID);

        // メンバの情報
        int memberNum = SteamMatchmaking.GetNumLobbyMembers(_roomID);
        // 自身以外の人が既にいる
        //if (memberNum >= 1)
        //{
        for (int memberIndex = 0; memberIndex < memberNum; memberIndex++)
        {
            CSteamID memberID = SteamMatchmaking.GetLobbyMemberByIndex(_roomID, memberIndex);
            // 自身以外の人の情報を取得
            //if (memberID != myID)
            //{
            GetMemberName(_roomID, memberID);
            GetMemberData(_roomID, memberID);
            //}
        }
        //}
    }

    /// <summary>
    /// 自身の情報を初期化
    /// </summary>
    private void InitSelfInfo(CSteamID myID)
    {
        M_RoomPlayerData data = new() { CastleIndex = CastleType.Castle1, State = GameReadyState.Preparing };
        NotifyMemberChange(myID, _globalVariable.GetMyName());
        NotifyMemberUpdate(myID, data);
        // SteamMatchmaking.SetLobbyMemberData(_ID, RoomParams.MEMBER_NAME_KEY, _globalVariable.GetMyName());
        // SteamMatchmaking.SetLobbyMemberData(_ID, RoomParams.MEMBER_CASTLE_KEY, ((int)CastleType.Castle1).ToString());
        // SteamMatchmaking.SetLobbyMemberData(_ID, RoomParams.MEMBER_STATE_KEY, ((int)GameReadyState.Preparing).ToString());
    }

    /// <summary>
    /// 部屋情報を取得
    /// </summary>
    /// <param name="lobbyID">部屋ID</param>
    private void GetLobbyData(CSteamID lobbyID)
    {
        // 部屋名
        string roomName = SteamMatchmaking.GetLobbyData(lobbyID, RoomParams.ROOM_NAME_KEY);
        NotifyRoomUpdate(roomName);
    }

    /// <summary>
    /// メンバ名を取得(入室か退室)
    /// </summary>
    /// <param name="lobbyID">部屋ID</param>
    /// <param name="memberID">メンバID</param>
    private void GetMemberName(CSteamID lobbyID, CSteamID memberID)
    {
        string memberName = SteamMatchmaking.GetLobbyMemberData(lobbyID, memberID, RoomParams.MEMBER_NAME_KEY);
        NotifyMemberChange(memberID, memberName);
        if (_needGetName.Contains(memberID) && !memberName.Equals(""))
            _needGetName.Remove(memberID);
    }

    /// <summary>
    /// メンバの新しい情報を取得
    /// </summary>
    /// <param name="lobbyID">部屋ID</param>
    /// <param name="memberID">メンバID</param>
    private void GetMemberData(CSteamID lobbyID, CSteamID memberID)
    {
        M_RoomPlayerData data;
        // 城の更新
        string castleString = SteamMatchmaking.GetLobbyMemberData(lobbyID, memberID, RoomParams.MEMBER_CASTLE_KEY);
        data.CastleIndex = (CastleType)int.Parse(castleString);
        // 状態の更新
        string stateString = SteamMatchmaking.GetLobbyMemberData(lobbyID, memberID, RoomParams.MEMBER_STATE_KEY);
        data.State = (GameReadyState)int.Parse(stateString);
        // 通知
        NotifyMemberUpdate(memberID, data);
    }

    /// <summary>
    /// 別の城に変更した
    /// </summary>
    /// <param name="type">新しい城</param>
    public void ChangeCastle(CastleType type)
    {
        SteamMatchmaking.SetLobbyMemberData(_roomID, RoomParams.MEMBER_CASTLE_KEY, ((int)type).ToString());
    }

    /// <summary>
    /// 準備状態を変更
    /// </summary>
    /// <param name="state">準備状態</param>
    public void ChangeState(GameReadyState state)
    {
        SteamMatchmaking.SetLobbyMemberData(_roomID, RoomParams.MEMBER_STATE_KEY, ((int)state).ToString());
    }

    /// <summary>
    /// メンバが変更されたことを通知する
    /// </summary>
    /// <param name="name">メンバ名</param>
    private void NotifyMemberChange(CSteamID memberID, string name)
    {
        OnMemberChanged?.Invoke(memberID, name);
    }

    /// <summary>
    /// メンバ情報が変更されたことを通知する
    /// </summary>
    /// <param name="castle">メンバ情報</param>
    private void NotifyMemberUpdate(CSteamID memberID, M_RoomPlayerData data)
    {
        OnMemberUpdated?.Invoke(memberID, data);
    }

    /// <summary>
    /// 部屋情報が変更されたことを通知する
    /// </summary>
    /// <param name="name">部屋名</param>
    private void NotifyRoomUpdate(string name)
    {
        OnLobbyUpdated?.Invoke(name);
    }

    /// <summary>
    /// 退室
    /// </summary>
    public void LeaveRoom()
    {
        SteamMatchmaking.LeaveLobby(_roomID);
        SceneManager.LoadScene("RoomList", LoadSceneMode.Single);
    }
}
