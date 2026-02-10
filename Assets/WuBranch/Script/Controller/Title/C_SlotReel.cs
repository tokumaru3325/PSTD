using System;
using System.Collections;
using UnityEngine;

public class C_SlotReel : MonoBehaviour
{

    [Tooltip("スロットロールの初期データ")]
    [SerializeField]
    private M_SlotReelData _data;

    private M_SlotReel _model;
    private ISlotReelView _view;

    //[2026/01/30] プリンス start
    /// <summary>
    /// 今いくつリールが回転している(SE用)
    /// </summary>
    private int activeReelSpinning = 0;
    //[2026/01/30] プリンス end

    /// <summary>
    /// 完全停止した後の処理
    /// </summary>
    public Action<int, int> OnStopped;

    /// <summary>
    /// 初期化
    /// </summary>
    /// <param name="view"></param>
    /// <param name="reelID">ロールのID</param>
    /// <param name="sprites">表示する全画像</param>
    /// <param name="offset">初期高さ</param>
    public void Initialize(ISlotReelView view, int reelID, Sprite[] sprites, float offset)
    {
        if (!_data)
        {
            Debug.LogError("Title slot's data did not set!");
            return;
        }

        _view = view;
        _model = new M_SlotReel(reelID, _data);
        _view.Initialize(_data.ImageSize, sprites, offset);
    }

    /// <summary>
    /// スクロール開始
    /// </summary>
    public void StartSpin()
    {
        _model.IsSpinning = true;
        _model.IsSeeking = false;
        _model.IsStopping = false;

        //[2026/01/30] プリンス start
        activeReelSpinning++;
        if (activeReelSpinning == 1)
        {
            StartCoroutine(SpinClicks(GetClickInterval));
        }
        //[2026/01/30] プリンス end
    }

    /// <summary>
    /// スクロールを停止させる
    /// </summary>
    /// <param name="targetIndex">表示したい目標</param>
    public void StopSpin(int targetIndex)
    {
        if (!_model.IsSpinning)
            return;

        // -1はプレイヤがコントロールするので、スクロールをすぐに停止させる
        if (targetIndex == -1)
        {
            _model.IsSpinning = false;
            _model.IsStopping = true;
        }
        // targetIndexの有効性を確認
        else if (targetIndex < 0 || targetIndex >= _view.GetItemCount())
        {
            Debug.LogWarning($"無効なTarget Index: {targetIndex}");
            targetIndex = 0;
            _model.IsSeeking = true;
        }
        else
        {
            // それ以外は目標までゆっくりまわる
            _model.IsSeeking = true;
        }
        _model.TargetIndex = targetIndex;
    }

    void Update()
    {
        if (_model.IsSpinning)
        {
            _model.UpdateSpeed();
            _view.MoveReel(_model.CurrentSpinSpeed * Time.deltaTime);

            if (_model.IsSeeking)
                CheckTargetReady();
        }
        else if (_model.IsStopping)
        {
            // 選ばれた結果に誘導
            if (_model.TargetIndex != -1)
                SnapToTarget();
            // プレイヤがコントロール
            else
                SnapToGrid();
        }
    }

    /// <summary>
    /// 目標が停止するゾーンに入ったかを確認
    /// </summary>
    private void CheckTargetReady()
    {
        float currentY = _view.GetItemY(_model.TargetIndex);

        // 判断方法：
        // _stopOffsetの所に止まらせたいので、
        // 検知範囲はstopOffsetから2倍の画像の高さ
        // かつ、今の回転スピードが300以下
        if (currentY >= 0f && currentY <= 2 * _view.ImageSize.y && _model.CurrentSpinSpeed <= 300f)
        {
            // スクロール停止
            _model.IsSpinning = false;
            _model.IsSeeking = false;
            _model.IsStopping = true;
        }
    }

    /// <summary>
    /// ターゲットをstopOffsetまで移動させる
    /// </summary>
    private void SnapToTarget()
    {
        // ターゲット今いる位置
        float currentY = _view.GetItemY(_model.TargetIndex);
        // 目標はstopOffset
        float targetY = _view.StopOffset;
        // stopOffsetまでの距離を計算
        float diff = Math.Abs(targetY - currentY);
        // 完全停止までスピード
        float stopSpeed = diff / _model.TimeToStop;
        // 今のスピードと比べてより小さい方を使用
        float speed = Mathf.Max(Mathf.Min(_model.CurrentSpinSpeed, stopSpeed), _model.MinSpeedToStop);

        // 目標に近つくと
        if (diff < _model.DistToStop)
        {
            Debug.Log($"{gameObject.name} stop");
            ForceMove(diff);
            // 通知
            OnStopped?.Invoke(_model.ReelID, _model.TargetIndex);
        }
        else
        {
            // 引き継ぎ移動
            _view.MoveReel(speed * Time.deltaTime);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void SnapToGrid()
    {
        // 一番目の画像をターゲットとして計算する
        float currentY = _view.GetFirstItemY();
        // まずオフセットを差し引き、0 を中心とした座標系に戻します
        // 一番近いグリッドを見つけます
        // オフセットを加え戻します
        float targetY = Mathf.Round((currentY - _view.StopOffset) / _view.ImageSize.y) * _view.ImageSize.y + _view.StopOffset;

        float diff = targetY - currentY;
        float moveStep = diff * 10f * Time.deltaTime;

        if (Mathf.Abs(diff) < _model.DistToStop)
        {
            // 差を補填
            ForceMove(-diff);
            // 通知
            OnStopped?.Invoke(_model.ReelID, _view.FindTarget());
        }
        else
        {
            _view.MoveReel(-moveStep);
        }
    }

    /// <summary>
    /// 強制的に移動させる
    /// </summary>
    /// <param name="diff">移動させる距離</param>
    private void ForceMove(float diff)
    {
        // 強制的に移動させる
        _view.MoveReel(diff);
        _model.IsStopping = false;
        _model.CurrentSpinSpeed = 0;
        // 2026.01.30 ウー start 止まる時音がずれるので修正
        //[2026/01/30] プリンス start
        activeReelSpinning--;
        SoundManager.Instance.PlaySE(SoundId.Impact, new SEPlayParams { clipIndex = 1 });
        if (activeReelSpinning <= 0)
        {
            activeReelSpinning = 0;
            //   SoundManager.Instance.StopSlotSpinSE();
        }
        //[2026/01/30] プリンス end
        // 2026.01.30 ウー end
    }

    //[2026/01/30] プリンス start
    public float GetClickInterval()
    {
        float t = Mathf.InverseLerp(_model.MinSpinSpeed, _model.MaxSpinSpeed, _model.CurrentSpinSpeed);
        return Mathf.Lerp(0.25f, 0.0f, t);
    }

    IEnumerator SpinClicks(Func<float> getInterval)
    {
        SoundManager.Instance.PlaySE(SoundId.Impact, new SEPlayParams { clipIndex = 2, ignoreCooldown = true, ignoreFrameGuard = true });
        while (_model.IsSpinning)
        {
            SoundManager.Instance.PlaySE(SoundId.SlotClick, new SEPlayParams { clipIndex = 0 });
            yield return new WaitForSeconds(getInterval());
        }
        // 2026.01.30 ウー start 止まる時音がずれるので修正
        //SoundManager.Instance.PlaySE(SoundId.Impact, new SEPlayParams { clipIndex = 1, ignoreCooldown = true, ignoreFrameGuard = true });
        // 2026.01.30 ウー end
    }
    //[2026/01/30] プリンス end
}
