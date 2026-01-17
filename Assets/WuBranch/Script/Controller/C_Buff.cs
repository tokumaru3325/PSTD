using System;
using System.Threading;

public class C_Buff
{
    /// <summary>
    /// タイマー
    /// </summary>
    private C_Timer _timer;

    /// <summary>
    /// タイプ
    /// </summary>
    /// <value></value>
    public BuffType Type { get; private set; }

    /// <summary>
    /// バフの効果
    /// </summary>
    private BuffTypeData _data;

    /// <summary>
    /// 対象
    /// </summary>
    /// <value></value>
    public string TargetTag { get; private set; }

    /// <summary>
    /// 効果終了際の動き
    /// </summary>
    public Action<C_Buff> OnCompleted;

    public C_Buff(BuffType type, string player, BuffTypeData data)
    {
        Type = type;
        _data = data;
        _timer = new C_Timer();
        _timer.SetTime(data.BuffTime);
        _timer.OnComplete += NotifyComplete;
        TargetTag = player;
    }

    /// <summary>
    /// カウントダウン開始
    /// </summary>
    public void StartCount(CancellationToken linkToToken = default, bool ignoreTimeScale = false)
    {
        _timer.StartTimer(linkToToken, ignoreTimeScale);
    }

    public void StopCount()
    {
        _timer.Dispose();
    }

    /// <summary>
    /// カウントダウン中にフレームごと実行する関数をバインド
    /// </summary>
    /// <param name="func"></param>
    public void BindTimeUpdate(Action<float> func)
    {
        _timer.OnTick += func;
    }

    /// <summary>
    /// カウントダウン終了する際に実行する関数をバインド
    /// </summary>
    /// <param name="func"></param>
    public void BindTimeComplete(Action<C_Buff> func)
    {
        OnCompleted += func;
    }

    /// <summary>
    /// 効果終了を通知
    /// </summary>
    private void NotifyComplete()
    {
        OnCompleted?.Invoke(this);
    }

    /// <summary>
    /// バフの効果をゲット
    /// </summary>
    /// <returns></returns>
    public float GetBuffValue()
    {
        return _data.BuffValue;
    }
}
