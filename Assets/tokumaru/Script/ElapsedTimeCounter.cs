using UnityEngine;
using System;
using System.Threading;
using Cysharp.Threading.Tasks;


public class ElapsedTimeCounter : IDisposable
{
    /// <summary>
    /// 毎フレーム経過時間を通知するイベント
    /// </summary>
    public Action<float> OnTick;

    /// <summary>
    /// 経過時間が一定値に達したときのイベント
    /// </summary>
    public Action<float> OnReachedTime;

    /// <summary>
    /// TimeScale を無視するかどうか
    /// </summary>
    private bool _ignoreTimeScale;

    /// <summary>
    /// 現在の経過時間
    /// </summary>
    public float ElapsedTime { get; private set; }

    /// <summary>
    /// キャンセルトークン
    /// </summary>
    private CancellationTokenSource _cts;

    /// <summary>
    /// 経過時間の監視を開始
    /// </summary>
    public void StartTracking(CancellationToken linkToToken, bool ignoreTimeScale)
    {
        StopTracking();

        _ignoreTimeScale = ignoreTimeScale;
        _cts = CancellationTokenSource.CreateLinkedTokenSource(linkToToken);

        TrackingRoutine(_cts.Token).Forget();
    }

    /// <summary>
    /// 経過時間の監視を停止
    /// </summary>
    public void StopTracking()
    {
        if (_cts != null)
        {
            _cts.Cancel();
            _cts.Dispose();
            _cts = null;
        }
    }

    /// <summary>
    /// 経過時間をリセット
    /// </summary>
    public void ResetTime()
    {
        ElapsedTime = 0f;
    }

    private async UniTaskVoid TrackingRoutine(CancellationToken token)
    {
        try
        {
            while (true)
            {
                await UniTask.Yield(PlayerLoopTiming.Update, token);

                float delta = _ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
                ElapsedTime += delta;

                OnTick?.Invoke(ElapsedTime);
            }
        }
        catch (OperationCanceledException)
        {
            // キャンセル時の処理
        }
        finally
        {
            if (_cts != null && _cts.Token == token)
            {
                _cts.Dispose();
                _cts = null;
            }
        }
    }

    public void Dispose()
    {
        StopTracking();
    }

}
