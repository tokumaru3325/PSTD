using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class V_SlotReel : MonoBehaviour
{
    [Tooltip("生成する図の大きさ")]
    [SerializeField]
    private Vector2 _imageSize = new Vector2(270, 190);

    [Tooltip("スクロールの最大回転速度")]
    [SerializeField]
    private float _maxSpinSpeed = 150f;

    [Tooltip("スクロールの最小回転速度")]
    [SerializeField]
    private float _minSpinSpeed = 50f;

    [Tooltip("スクロールの回転加速度")]
    [SerializeField]
    private float _startSpinAcceleration = 10f;

    [Tooltip("停止の加速度")]
    [SerializeField]
    private float _deceleration = 30f;

    [Tooltip("完全停止まるまでの距離")]
    [SerializeField]
    private float _distToStop = 5.0f;

    [Tooltip("完全停止まるまでの時間(秒)")]
    [SerializeField]
    private float _timeToStop = 3.0f;

    [Tooltip("完全停止まるまでの最小スピード")]
    [SerializeField]
    private float _minSpeedToStop = 50f;

    /// <summary>
    /// 一ロールにあるの全部の図
    /// </summary>
    private List<RectTransform> _reelItems = new List<RectTransform>();

    /// <summary>
    /// 回転開始フラグ
    /// </summary>
    private bool _isSpinning = false;

    /// <summary>
    /// 止まるフラグ
    /// </summary>
    private bool _isStopping = false;

    /// <summary>
    /// 目標を探すフラグ
    /// </summary>
    private bool _isSeeking = false;

    /// <summary>
    /// 一ロールの高さ
    /// </summary>
    private float _totalHeight;

    /// <summary>
    /// この値より低くと一番高いところに移動(循環させる)
    /// </summary>
    private float _threshold;

    /// <summary>
    /// 停止時の高さ、背景によって必ず0ではないため
    /// </summary>
    private float _stopOffset;

    /// <summary>
    /// 停止する時のインデックス
    /// </summary>
    private int _targetIndex;

    /// <summary>
    /// 今の回転速度
    /// </summary>
    private float _currentSpinSpeed = 0f;

    /// <summary>
    /// 初期化、もらったテクスチャを使ってロールを生成
    /// </summary>
    /// <param name="sprites">テクスチャ</param>
    /// <param name="offset">停止時の高さ</param>
    public void Initialize(Sprite[] sprites, float offset)
    {
        // あるものをクリア
        foreach (Transform child in transform)
            Destroy(child.gameObject);
        _reelItems.Clear();

        _stopOffset = offset;

        // テクスチャがない場合、何もしない
        if (sprites == null || sprites.Length == 0)
            return;

        // 高さを計算
        _totalHeight = sprites.Length * _imageSize.y;
        // 循環するための高さを計算
        _threshold = -(2 * _imageSize.y) + _stopOffset;

        // Imageを生成
        for (int i = 0; i < sprites.Length; i++)
        {
            GameObject obj = new GameObject($"Symbol_{i}");
            obj.transform.SetParent(transform, false);

            Image img = obj.AddComponent<Image>();
            img.sprite = sprites[i];

            RectTransform rt = obj.GetComponent<RectTransform>();
            // アンカーを先に設定してから位置を設置
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = _imageSize;
            rt.anchoredPosition = new Vector2(0, (i - 1) * _imageSize.y + _stopOffset);

            _reelItems.Add(rt);
        }
    }

    void Update()
    {
        if (_isSpinning)
        {
            // どんどん速くなるが、最大には越えない
            if (!_isSeeking)
                _currentSpinSpeed = Mathf.Clamp(_currentSpinSpeed + _startSpinAcceleration, 0, _maxSpinSpeed);
            else
                _currentSpinSpeed = Mathf.Clamp(_currentSpinSpeed - _deceleration, _minSpinSpeed, _maxSpinSpeed);

            MoveReel(_currentSpinSpeed * Time.deltaTime);

            if (_isSeeking)
            {
                CheckIfTargetIsReady();
            }
        }
        else if (_isStopping)
        {
            SnapToTarget();
        }
    }

    /// <summary>
    /// スクロール
    /// </summary>
    /// <param name="distance">移動距離</param>
    private void MoveReel(float distance)
    {
        foreach (var rt in _reelItems)
        {
            // 下に移動
            Vector2 pos = rt.anchoredPosition;
            pos.y -= distance;

            if (pos.y <= _threshold)
            {
                pos.y += _totalHeight;
            }

            rt.anchoredPosition = pos;
        }
    }

    /// <summary>
    /// スクロール開始
    /// </summary>
    public void StartSpin()
    {
        _isSpinning = true;
        _isSeeking = false;
        _isStopping = false;
    }

    /// <summary>
    /// スクロール停止
    /// </summary>
    /// <param name="targetIndex">表示したい目標</param>
    public void StopSpin(int targetIndex)
    {
        if (!_isSpinning) return;

        // targetIndexの有効性を確認
        if (targetIndex < 0 || targetIndex >= _reelItems.Count)
        {
            Debug.LogWarning($"無効なTarget Index: {targetIndex}");
            targetIndex = 0;
        }

        _targetIndex = targetIndex;

        // すぐに止まらない
        _isSeeking = true;
    }

    /// <summary>
    /// 目標が停止するゾーンに入ったかを確認
    /// </summary>
    private void CheckIfTargetIsReady()
    {
        RectTransform targetItem = _reelItems[_targetIndex];
        float currentY = targetItem.anchoredPosition.y;

        // 判断方法：
        // _stopOffsetの所に止まらせたいので、
        // 検知範囲はstopOffsetから2倍の画像の高さ
        // かつ、今の回転スピードが300以下
        if (currentY >= _stopOffset && currentY <= _stopOffset + 2 * _imageSize.y && _currentSpinSpeed <= 300f)
        {
            // スクロール停止
            _isSpinning = false;
            _isSeeking = false;
            _isStopping = true;
        }
    }

    /// <summary>
    /// ターゲットをstopOffsetまで移動させる
    /// </summary>
    private void SnapToTarget()
    {
        RectTransform targetItem = _reelItems[_targetIndex];
        float currentY = targetItem.anchoredPosition.y;

        // 目標はstopOffset
        float targetY = _stopOffset;

        // stopOffsetまでの距離を計算
        float diff = Math.Abs(targetY - currentY);
        // 完全停止までスピード
        float stopSpeed = diff / _timeToStop;
        // 今のスピードと比べてより小さい方を使用
        float speed = Mathf.Max(Mathf.Min(_currentSpinSpeed, stopSpeed), _minSpeedToStop);

        // 目標に近つくと
        if (Mathf.Abs(diff) < _distToStop)
        {
            // 強制的に移動させる
            MoveReel(diff);
            _isStopping = false;
            _currentSpinSpeed = 0;
        }
        else
        {
            // 引き継ぎ移動
            MoveReel(speed * Time.deltaTime);
        }
    }
}
