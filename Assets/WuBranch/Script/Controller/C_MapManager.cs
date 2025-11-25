using Cysharp.Threading.Tasks;
using System.IO;
using Unity.VisualScripting;
using System.Collections.Generic;
using UnityEngine;


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

    async UniTaskVoid Awake()
    {
        Map = new M_Map();
        // Application.streamingAssetsPath はAssets下のStreamingAssetsフォルダを指す
        // 読み専用の安全なディレクトリです。(プラットフォーム共通)
        // Path.Combine は、2番目の引数が / や \ で始まると、それを「絶対パス（またはルートからのパス）」とみなしてしまい、1番目の引数を無視します。
        string path = Path.Combine(Application.streamingAssetsPath, "MapData/Stage1.csv");
        // (注意: 実行前にこのパスにファイルを配置しておく必要があります)
        ReadMap(path);
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

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
        List<List<int>> tmpMap = new List<List<int>>();
        foreach (var row in source)
        {
            Debug.Log("[ " + string.Join(" | ", row) + " ]");
            List<int> colData = new List<int>();
            foreach (var col in row)
            {
                colData.Add(int.Parse(col));
            }
            tmpMap.Add(colData);
        }
        Map.SetPath(tmpMap);
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
}
