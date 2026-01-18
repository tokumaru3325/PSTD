using Cysharp.Threading.Tasks;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.Netcode;

public class C_MapManager : MonoBehaviour
{
    /// <summary>
    /// マップ
    /// </summary>
    public M_Map Map { get; private set; }

    /// <summary>
    /// マップ座標(0,0)の初期Unity位置、左上が0,0(Unityの座標だと(-18.5, 8.5))
    /// </summary>
    [SerializeField]
    private float MAP_INIT_POS_X;
    [SerializeField]
    private float MAP_INIT_POS_Y;

    /// <summary>
    /// マップにあるすべてのプレイヤ位置
    /// キー: マップ座標, 値: 使用中かどうか
    /// </summary>
    private List<M_PlayerPosInfo> _playerPos;

    /// <summary>
    /// マップ読み込み完了フラグ
    /// </summary>
    /// <value></value>
    public bool IsComplete { get; private set; }

    //デバッグ用
    public bool IsPathVisible { get; private set; }

    void Awake()
    {
        // 唯一にする
        C_MapManager[] list = FindObjectsByType<C_MapManager>(FindObjectsSortMode.None);
        if (list.Length >= 2)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        Initialize();
    }

    void Start()
    {
        IsPathVisible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsPathVisible)
        {
            DrawPath();
        }
    }

    /// <summary>
    /// 初期化
    /// </summary>
    private void Initialize()
    {
        Map = new M_Map();
        _playerPos = new List<M_PlayerPosInfo>();
        IsComplete = false;
        // Application.streamingAssetsPath はAssets下のStreamingAssetsフォルダを指す
        // 読み専用の安全なディレクトリです。(プラットフォーム共通)
        // Path.Combine は、2番目の引数が / や \ で始まると、それを「絶対パス（またはルートからのパス）」とみなしてしまい、1番目の引数を無視します。
        string path = Path.Combine(Application.streamingAssetsPath, "MapData/Stage1.csv");
        // (注意: 実行前にこのパスにファイルを配置しておく必要があります)
        _ = ReadMap(path);
    }

    //[START] 2025/12/14 プリンス：デバッガーから使えるように、関数化した
    public void DrawPath()
    {
        List<M_MapPosition> route = C_PathSearch.GetPath(GetAllRoute(), new M_MapPosition(4, 5), new M_MapPosition(31, 16));
        for (int index = 0; index < route.Count; index++)
        {
            Color txtColor = Color.blue;
            if (index == 0 || index == route.Count - 1)
                txtColor = Color.red;
            Vector3 pos = ConvertToUnityPos(route[index]);
            Debug.DrawLine(pos + Vector3.left, pos - Vector3.left, txtColor, 0f, false);
        }
    }

    public void SetPathVisibility(bool visible)
    {
        IsPathVisible = visible;
    }
    //[END] 2025/12/14 プリンス

    /// <summary>
    /// マップを読み込む
    /// </summary>
    /// <param name="path">マップのパス</param>
    /// <returns></returns>
    private async UniTaskVoid ReadMap(string path)
    {
        try
        {
            Debug.Log($"ファイルの読み込みを開始します: {path}");

            // シングルトンインスタンス経由で非同期メソッドを呼び出す
            // this.GetCancellationTokenOnDestroy() で、このオブジェクトが
            // 破棄されたら自動的にファイル読み込みをキャンセルできます。
            string[][] data = await C_FileManager.Instance.LoadDataAsync(
                path,
                FileType.CSV,
                this.GetCancellationTokenOnDestroy()
            );

            // この行はワーカースレッドからメインスレッドに戻った後に実行されます
            if (data != null)
            {
                Debug.Log($"データの読み込みが完了しました。{data.Length} 行");
                ConvertData(data);
                Debug.Log($"マップデータへの変換が完了しました。");
                IsComplete = true;
            }
        }
        catch (FileNotFoundException ex)
        {
            Debug.LogWarning(ex.Message);
        }
        catch (IOException ex)
        {
            Debug.LogError($"致命的なI/Oエラー: {ex.Message}");
        }
    }

    /// <summary>
    /// データ変換
    /// </summary>
    /// <param name="data">ソース</param>
    private void ConvertData(string[][] source)
    {
        List<List<int>> tmpRoute = new List<List<int>>();
        int rowIndex = 0;
        foreach (var row in source)
        {
            Debug.Log("[ " + string.Join(" | ", row) + " ]");
            List<int> colData = new List<int>();
            int colIndex = 0;
            foreach (var col in row)
            {
                int value = int.Parse(col);
                RecordPlayerPos(value, colIndex, rowIndex);
                // プレイヤ位置もたどり着けるので、プレイヤ位置を一般道に変換
                int routePiece = value == (int)PathStructure.PlayerR || value == (int)PathStructure.PlayerL ? (int)PathStructure.Road : value;
                colData.Add(routePiece);
                // 次の列へ
                colIndex++;
            }
            tmpRoute.Add(colData);
            rowIndex++;
        }
        Map.SetPath(tmpRoute);
    }

    /// <summary>
    /// プレイヤ位置を記録
    /// </summary>
    /// <param name="value"></param>
    /// <param name="colIndex"></param>
    /// <param name="rowIndex"></param>
    private void RecordPlayerPos(int value, int colIndex, int rowIndex)
    {
        if (value != (int)PathStructure.PlayerR && value != (int)PathStructure.PlayerL)
            return;

        bool isLeft = value == (int)PathStructure.PlayerL;
        Debug.Log($"Record player pos at {colIndex}, {rowIndex}");
        _playerPos.Add(new M_PlayerPosInfo(new M_MapPosition(colIndex, rowIndex), false, isLeft));
    }

    /// <summary>
    /// ルートをゲット
    /// </summary>
    /// <returns>ルート</returns>
    public List<List<int>> GetAllRoute()
    {
        return Map.GetPath();
    }

    /// <summary>
    /// マップ座標に変換
    /// </summary>
    /// <param name="posX">Unity座標X</param>
    /// <param name="posY">Unity座標y</param>
    /// <returns>マップ座標</returns>
    public M_MapPosition ConvertToMapPos(float posX, float posY)
    {
        M_MapPosition targetPos;
        targetPos.X = Mathf.RoundToInt(posX - MAP_INIT_POS_X);
        targetPos.Y = Mathf.RoundToInt(-(posY - MAP_INIT_POS_Y));
        return targetPos;
    }

    /// <summary>
    /// マップ座標に変換
    /// </summary>
    /// <param name="position">Unity座標</param>
    /// <returns>マップ座標</returns>
    public M_MapPosition ConvertToMapPos(Vector3 position)
    {
        M_MapPosition targetPos;
        targetPos.X = Mathf.RoundToInt(position.x - MAP_INIT_POS_X);
        targetPos.Y = Mathf.RoundToInt(-(position.y - MAP_INIT_POS_Y));
        return targetPos;
    }

    /// <summary>
    /// Unity座標に変換
    /// </summary>
    /// <param name="position">マップ座標</param>
    /// <returns>Unity座標</returns>
    public Vector3 ConvertToUnityPos(M_MapPosition position)
    {
        Vector3 targetPos = Vector3.zero;
        targetPos.x = position.X + MAP_INIT_POS_X;
        targetPos.y = -position.Y + MAP_INIT_POS_Y;
        return targetPos;
    }

    /// <summary>
    /// 行けるかどうか
    /// </summary>
    /// <param name="pos">Unity座標</param>
    /// <returns>true: 行ける, false: 行けない</returns>
    public bool CanGo(Vector3 pos)
    {
        M_MapPosition mapPos = ConvertToMapPos(pos);
        return Map.GetPathCost(mapPos.X, mapPos.Y) != (int)PathStructure.Blocked;
    }

    /// <summary>
    /// 行けるかどうか
    /// </summary>
    /// <param name="pos">マップ座標</param>
    /// <returns>true: 行ける, false: 行けない</returns>
    public bool CanGo(M_MapPosition pos)
    {
        return Map.GetPathCost(pos.X, pos.Y) != (int)PathStructure.Blocked;
    }

    /// <summary>
    /// 経路コストをゲット
    /// </summary>
    /// <param name="pos">マップ座標</param>
    /// <returns>コスト</returns>
    public int GetPathCost(M_MapPosition pos)
    {
        return Map.GetPathCost(pos.X, pos.Y);
    }

    /// <summary>
    /// 未使用のプレイヤ位置を1つ使う
    /// </summary>
    /// <returns>プレイヤ位置</returns>
    public M_PlayerPosInfo UseOnePlayerPos()
    {
        var unusedPlayerPos = _playerPos.Where(_ => !_.IsUsed);
        Debug.Log($"unused player pos count: {unusedPlayerPos.Count()}");
        int index = Random.Range(0, unusedPlayerPos.Count());
        Debug.Log($"get index: {index}");
        M_PlayerPosInfo pos = unusedPlayerPos.ElementAt(index);
        pos.IsUsed = true;
        return pos;
    }
}
