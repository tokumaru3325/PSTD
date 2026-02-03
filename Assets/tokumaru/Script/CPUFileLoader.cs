using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using UnityEngine;

public class CPUFileLoader : IDisposable

{
    /// <summary>
    /// 読み込み間隔（秒）
    /// </summary>
    private float _interval;

    /// <summary>
    /// 次に読み込む時間
    /// </summary>
    private float _nextTime;

    /// <summary>
    /// 経過時間トラッカー
    /// </summary>
    private ElapsedTimeCounter _tracker;

    /// <summary>
    /// キャンセルトークン
    /// </summary>
    private CancellationTokenSource _cts;

    /// <summary>
    /// ファイルパス
    /// </summary>
    private string _filePath;

    /// <summary>
    /// ファイル内容を処理するイベント
    /// </summary>
    public Action<string[][]> OnFileLoaded;

    public CPUFileLoader(string filePath, float intervalSeconds)
    {
        _filePath = filePath;

        _tracker = new ElapsedTimeCounter();
    }

    public void Start(CancellationToken linkToToken)
    {
        Stop();

        _cts = CancellationTokenSource.CreateLinkedTokenSource(linkToToken);

        LoadAndExecute().Forget();

        //// 経過時間トラッカー開始
        //_tracker.OnTick += OnTick;
        //_tracker.StartTracking(_cts.Token, ignoreTimeScale);
    }

    private void OnTick(float elapsed)
    {
        if (elapsed >= _nextTime)
        {
            _nextTime += _interval;
            LoadAndExecute().Forget();
        }
    }

    private async UniTaskVoid LoadAndExecute()
    {
        try
        {
            var data = await C_FileManager.Instance.LoadDataAsync(
                _filePath,
                FileType.CSV,
                _cts.Token
            );

            if (data != null)
            {
                OnFileLoaded?.Invoke(data);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"ファイル読み込みエラー: {ex.Message}");
        }
    }

    public void Stop()
    {
        if (_cts != null)
        {
            _cts.Cancel();
            _cts.Dispose();
            _cts = null;
        }

        _tracker?.StopTracking();
    }

    public void Dispose()
    {
        Stop();
        _tracker?.Dispose();
    }

}
