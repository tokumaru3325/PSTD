using Cysharp.Threading.Tasks;
using Steamworks;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// シーンをまたいでの共通変数
/// </summary>
public class C_GlobalVariable : MonoBehaviour
{
    private M_GlobalVariable _datas;

    void Awake()
    {
        // 唯一にする
        C_GlobalVariable[] list = FindObjectsByType<C_GlobalVariable>(FindObjectsSortMode.None);
        if (list.Length >= 2)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        _datas = new M_GlobalVariable();
    }

    void Update()
    {
        PressESC().Forget();
    }

    /// <summary>
    /// ESCキーが押されたら
    /// </summary>
    private async UniTask PressESC()
    {
        await UniTask.WaitUntil(() => Input.GetKeyDown(KeyCode.Escape));
        GameShutdown();
    }

    /// <summary>
    /// ゲームをシャットダウン
    /// </summary>
    private void GameShutdown()
    {
        // ネットワーク上の物も全部シャットダウン
       /* if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsClient)
        {
            SteamMatchmaking.LeaveLobby(_datas.RoomID);
            NetworkManager.Singleton.Shutdown();
        }*/
        Application.Quit();
    }

    /// <summary>
    /// 部屋の役割を設定
    /// </summary>
    /// <param name="role">役割</param>
    public void SetRoomRole(MultiRoleType role)
    {
        _datas.SetRole(role);
    }

    /// <summary>
    /// 部屋の役割を取得
    /// </summary>
    /// <returns>役割</returns>
    public MultiRoleType GetRoomRole()
    {
        return _datas.MultiRole;
    }

    /// <summary>
    /// プレイヤーの自身の名前を設定
    /// </summary>
    /// <param name="name">名前</param>
    public void SetMyName(string name)
    {
        _datas.SetMyName(name);
    }

    /// <summary>
    /// プレイヤーの自身の名前を取得
    /// </summary>
    /// <returns>名前</returns>
    public string GetMyName()
    {
        return _datas.MyName;
    }

    /// <summary>
    /// ルームIDを設定
    /// </summary>
    /// <param name="ID">ルームID</param>
    public void SetRoomID(CSteamID ID)
    {
        _datas.SetRoomID(ID);
    }

    /// <summary>
    /// ルームIDを取得
    /// </summary>
    /// <returns>ルームID</returns>
    public CSteamID GetRoomID()
    {
        return _datas.RoomID;
    }

    /// <summary>
    /// プレイヤーが選択した城のタイプを追加または更新
    /// </summary>
    /// <param name="playerID">プレイヤーID</param>
    /// <param name="castleType">城のタイプ</param>
    public void AddPlayerSelectedCastle(CSteamID playerID, CastleType castleType)
    {
        _datas.AddPlayerSelectedCastle(playerID, castleType);
    }

    /// <summary>
    /// プレイヤーの選択した城のタイプを削除
    /// </summary>
    /// <param name="playerID">プレイヤーID</param>
    /// <returns>城のタイプ</returns>
    public CastleType GetPlayerCastle(CSteamID playerID)
    {
        if (!_datas.SelectedCastles.ContainsKey(playerID))
        {
            return CastleType.Null;
        }
        return _datas.SelectedCastles[playerID];
    }
}
