using UnityEngine;
using System.IO;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;

/// <summary>
/// 読み込むファイルの種類を定義します。
/// </summary>
public enum FileType
{
    TXT,
    CSV
}

public class C_FileManager
{
    #region Singleton Implementation

    private static readonly Lazy<C_FileManager> instance =
        new Lazy<C_FileManager>(() => new C_FileManager());

    public static C_FileManager Instance => instance.Value;

    private C_FileManager()
    {
    }

    #endregion

    // 削除する文字（空白とカンマ）を定義する正規表現
    private static readonly Regex _removeCharsRegex = new Regex(@"[\s,]", RegexOptions.Compiled);

    /// <summary>
    /// ファイルを非同期 (UniTask) で読み込み、加工したデータを二次配列で返します。
    /// 処理はワーカースレッドで実行されます。
    /// </summary>
    /// <param name="filePath">読み込むファイルのフルパス</param>
    /// <param name="fileType">ファイルの種類 (TXT または CSV)</param>
    /// <param name="cancellationToken">キャンセル用トークン (任意)</param>
    /// <returns>加工済みの string[][] データ</returns>
    public async UniTask<string[][]> LoadDataAsync(string filePath, FileType fileType, CancellationToken cancellationToken = default)
    {
        // UniTask.RunOnThreadPool を使い、重いファイルI/O処理全体を
        // Unityのメインスレッド外 (ワーカースレッド) で実行します。
        return await UniTask.RunOnThreadPool(async () =>
        {
            if (!File.Exists(filePath))
            {
                // メインスレッドでなくても Debug.LogWarning や
                // 例外のスローは可能です (例外は await した側に伝播します)
                throw new FileNotFoundException("指定されたファイルが見つかりません。", filePath);
            }

            var allRows = new List<string[]>();

            try
            {
                using (var sr = new StreamReader(filePath))
                {
                    string line;

                    // CancellationToken が要求されたか確認
                    cancellationToken.ThrowIfCancellationRequested();

                    // ReadLineAsync は標準の Task<string> を返しますが、
                    // await は UniTask 内でも Task を待機できます。
                    while ((line = await sr.ReadLineAsync()) != null)
                    {
                        // キャンセル確認 (ループのたびに確認)
                        cancellationToken.ThrowIfCancellationRequested();

                        string[] sourceColumns;

                        // 1. ファイルタイプに応じて行を分割
                        if (fileType == FileType.CSV)
                        {
                            sourceColumns = line.Split(',');
                        }
                        else // FileType.TXT
                        {
                            sourceColumns = new string[] { line };
                        }

                        // 2. 各セルを加工
                        var processedColumns = new string[sourceColumns.Length];
                        for (int i = 0; i < sourceColumns.Length; i++)
                        {
                            processedColumns[i] = _removeCharsRegex.Replace(sourceColumns[i], "");
                        }

                        allRows.Add(processedColumns);
                    }
                }

                // ワーカースレッドの結果 (string[][]) を返す
                return allRows.ToArray();
            }
            catch (OperationCanceledException)
            {
                // キャンセルされた場合はログを出力（または何もしない）
                UnityEngine.Debug.Log("ファイル読み込みがキャンセルされました。");
                return null; // または空の配列
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError($"ファイルの読み込み中にエラーが発生しました: {ex.Message}");
                throw new IOException($"ファイル読み込みエラー: {filePath}", ex);
            }

        }, cancellationToken: cancellationToken); // RunOnThreadPool にもトークンを渡す
    }
}
