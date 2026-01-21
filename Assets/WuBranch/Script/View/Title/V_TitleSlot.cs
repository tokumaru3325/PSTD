using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class ReelInfo
{
    /// <summary>
    /// ロール
    /// </summary>
    [SerializeField]
    public V_SlotReel Reel;

    /// <summary>
    /// 画像
    /// </summary>
    [SerializeField]
    public Sprite[] Sprites;
}

public class V_TitleSlot : MonoBehaviour
{
    [Tooltip("ロールデータ")]
    [SerializeField]
    private ReelInfo[] _reels;

    [Tooltip("各ロールを止まる時の時間差")]
    [SerializeField]
    private float _stopDelay = 1.0f;

    [Tooltip("止まる位置")]
    [SerializeField]
    private float _stopHeightOffset = 0f;

    [Tooltip("結果はランダムにするか")]
    [SerializeField]
    private bool _randomizeResult = true;

    [Tooltip("指定された画像を結果にする(それぞれロールの画像のインデックス,0 ~ 画像の数-1)")]
    [SerializeField]
    public int[] _stopIndices;

    /// <summary>
    /// 回る中ですか
    /// </summary>
    private bool _isRunning = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (_reels != null)
        {
            if (_stopIndices == null || _stopIndices.Length != _reels.Length)
            {
                _stopIndices = new int[_reels.Length];
            }

            for (int index = 0; index < _reels.Length; index++)
            {
                _reels[index].Reel.Initialize(_reels[index].Sprites, _stopHeightOffset);
            }
        }
    }

    public void OnClickStart()
    {
        if (_isRunning)
            return;

        SpinRoutine();
    }

    public void OnClickStop()
    {
        if (!_isRunning)
            return;

        // もしランダムの結果が欲しいなら、ここで決める
        int[] result = _randomizeResult ? GenerateRandomResult() : _stopIndices;

        StartCoroutine(StopRoutine(result));
    }

    /// <summary>
    /// 欲しい結果を設定
    /// </summary>
    /// <param name="result">結果</param>
    public void SetResult(int[] result)
    {
        if (result.Length != _reels.Length)
        {
            Debug.LogError("設定したい結果の数がロールの数とあっていない");
            return;
        }
        _stopIndices = result;
        _randomizeResult = false;
    }

    /// <summary>
    /// ランダムの結果を生成
    /// </summary>
    private int[] GenerateRandomResult()
    {
        int[] result = new int[_stopIndices.Length];
        for (int i = 0; i < _stopIndices.Length; i++)
        {
            result[i] = UnityEngine.Random.Range(0, _reels[i].Sprites.Length);
        }
        Debug.Log("今回の結果: " + string.Join(", ", result));
        return result;
    }

    /// <summary>
    /// 回り始める
    /// </summary>
    void SpinRoutine()
    {
        _isRunning = true;
        for (int i = 0; i < _reels.Length; i++)
        {
            _reels[i].Reel.StartSpin();
        }
    }

    /// <summary>
    /// 止める
    /// </summary>
    /// <param name="result">結果</param>
    /// <returns></returns>
    IEnumerator StopRoutine(int[] result)
    {
        for (int i = 0; i < _reels.Length; i++)
        {
            int targetIndex = result[i];
            _reels[i].Reel.StopSpin(targetIndex);

            yield return new WaitForSeconds(_stopDelay);
        }
        _isRunning = false;
    }
}
