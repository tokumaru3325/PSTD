using UnityEngine;

public static class RoomParams
{
    /// <summary>
    /// ゲームIDのキー
    /// </summary>
    public const string GAME_ID_KEY = "PSTD";

    /// <summary>
    /// ゲームIDのバリュー
    /// </summary>
    public const string GAME_ID_VALUE = "JEC_CI_Astral_PSTD";

    /// <summary>
    /// ゲームバージョンのキー
    /// </summary>
    public const string VERSION_KEY = "Version";

    /// <summary>
    /// ゲームバージョンのバリュー
    /// </summary>
    public const string VERSION_VALUE = "DEV_0";

    // <summary>
    /// 一回サーチすることで返すロビーの最大数
    /// </summary>
    public const int MAX_LOBBY_COUNT = 30;

    /// <summary>
    /// ロビーのメタデータキーの最大文字数。
    /// </summary>
    public const int METADATA_KEY_SIZE = 255;

    /// <summary>
    /// ロビーのメタデータが持てる最大のサイズ（バイト数）。
    /// </summary>
    public const int METADATA_VALUE_SIZE = 8192;

    /// <summary>
    /// 部屋の名前のキー
    /// </summary>
    public const string ROOM_NAME_KEY = "RoomName";

    /// <summary>
    /// 部屋のパスワードのキー
    /// </summary>
    public const string ROOM_PASSWORD_KEY = "RoomPassword";

    /// <summary>
    /// 部屋主の名前のキー
    /// </summary>
    public const string ROOM_LEADER_KEY = "Leader";
}
