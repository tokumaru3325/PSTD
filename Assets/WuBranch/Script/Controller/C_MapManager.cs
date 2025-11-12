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


    async UniTaskVoid Awake()
    {
        // Application.streamingAssetsPath はAssets下のStreamingAssetsフォルダを指す
        // 読み専用の安全なディレクトリです。(プラットフォーム共通)
        // Path.Combine は、2番目の引数が / や \ で始まると、それを「絶対パス（またはルートからのパス）」とみなしてしまい、1番目の引数を無視します。
        string path = Path.Combine(Application.streamingAssetsPath, "MapData/Stage1.csv");
        // (注意: 実行前にこのパスにファイルを配置しておく必要があります)
        ReadMap(path);
    }

    async UniTaskVoid Start()
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
            foreach(var col in row)
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
    private List<List<int>> GetPath()
    {
        return Map.GetPath();
    }

}
