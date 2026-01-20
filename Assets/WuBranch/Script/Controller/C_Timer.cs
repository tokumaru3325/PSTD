using UnityEngine;
using System;
using System.Threading;
using Cysharp.Threading.Tasks;

public class C_Timer : IDisposable
{
    /// <summary>
    /// フレームごと実行するイベント(残り秒数を返す)
    /// </summary>
    public Action<float> OnTick;

    /// <summary>
    /// カウントダウン終了のイベント
    /// </summary>
    public Action OnComplete;

    /// <summary>
    /// 総秒数
    /// </summary>
    private float _duration;

    /// <summary>
    /// キャンセルトークン
    /// </summary>
    private CancellationTokenSource _timerCts;

    /// <summary>
    /// カウントダウンの秒数を設定
    /// </summary>
    /// <param name="duration">秒数</param>
    public void SetTime(float duration)
    {
        _duration = duration;
    }

    /// <summary>
    /// カウント開始
    /// </summary>
    /// <param name="ignoreTimeScale">ゲームポーズを無視するか (TimeScale)</param>
    public void StartTimer(CancellationToken linkToToken, bool ignoreTimeScale)
    {
        // 既に実行している場合、先に止める
        StopTimer();

        // キャンセルトークンを生成
        _timerCts = CancellationTokenSource.CreateLinkedTokenSource(linkToToken);

        // 起動
        TimerRoutine(_duration, ignoreTimeScale, _timerCts.Token).Forget();
    }

    /// <summary>
    /// カウントダウン停止
    /// </summary>
    public void StopTimer()
    {
        if (_timerCts != null)
        {
            _timerCts.Cancel();
            _timerCts.Dispose();
            _timerCts = null;
        }
    }

    private async UniTaskVoid TimerRoutine(float duration, bool ignoreTimeScale, CancellationToken token)
    {
        float remainingTime = duration;

        // 初回が表示できるように
        OnTick?.Invoke(remainingTime);

        try
        {
            while (remainingTime > 0)
            {
                // 次のフレームを待つ。PlayerLoopTiming.Updateを使うとこで、Update段階で実行されることを確保する
                await UniTask.Yield(PlayerLoopTiming.Update, token);

                // 計算
                float delta = ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
                remainingTime -= delta;

                // 
                if (remainingTime < 0) remainingTime = 0;

                // 更新イベントを実行
                OnTick?.Invoke(remainingTime);
            }

            // カウントダウン終了
            OnComplete?.Invoke();
        }
        catch (OperationCanceledException)
        {
            // StopTimerによってキャンセルした時、ここに入る
        }
        finally
        {
            // リソースを解放
            if (_timerCts != null && _timerCts.Token == token)
            {
                _timerCts.Dispose();
                _timerCts = null;
            }
        }
    }

    // カウントダウンを停止する(MissingReferenceExceptionを防ぐ)
    public void Dispose()
    {
        StopTimer();
    }
}
