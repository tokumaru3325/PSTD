using System;
using System.Collections;
using System.Linq;
using Unity.VisualScripting;
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

[Serializable]
public class WantResult
{
    [Tooltip("指定された画像を結果にする(それぞれロールの画像のインデックス,0 ~ 画像の数-1)")]
    [SerializeField]
    public int[] Indices;
}

/// <summary>
/// スロットの結果
/// </summary>
public enum ResultType
{
    Miss,
    Start,
    Close
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

    [Tooltip("プレイヤーがコントロールするか")]
    public bool CustomControll = false;

    [Tooltip("結果はランダムにするか")]
    [SerializeField]
    private bool _randomizeResult = true;

    /// <summary>
    /// PSTDを組んだらゲーム開始、ｘｘｘｘを組んだらゲーム終了
    /// </summary>
    [Tooltip("ゲーム開始の組み合わせ")]
    [SerializeField]
    private WantResult[] _startResults;

    [Tooltip("ゲーム終了の組み合わせ")]
    [SerializeField]
    private WantResult[] _closeResults;

    [Tooltip("指定された画像を結果にする(それぞれロールの画像のインデックス,0 ~ 画像の数-1)")]
    [SerializeField]
    private int[] _stopIndices;

    /// <summary>
    /// 回る中ですか
    /// </summary>
    public bool IsRunning { get; private set; }

    /// <summary>
    /// 完全停止したか
    /// </summary>
    /// <value></value>
    public Action<ResultType> OnFinished;

    /// <summary>
    /// 停止したロールの数
    /// </summary>
    private int _reelFinishedNum;

    /// <summary>
    /// 各ロールの結果
    /// </summary>
    private int[] _reelResult;

    /// <summary>
    /// 強制的に成功させるフラグ
    /// </summary>
    private bool _forceCorrect;

    [Tooltip("何回遊んだら強制的に当たる")]
    [SerializeField]
    private int _maxPlay = 3;

    /// <summary>
    /// 今遊んだ回数
    /// </summary>
    private int _currentPlay = 0;

    /// <summary>
    /// あたりのエフェクト
    /// </summary>
    private V_BlinkEffect _winEffect;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (_reels != null)
        {
            _reelResult = new int[_reels.Length];
            if (_stopIndices == null || _stopIndices.Length != _reels.Length)
            {
                _stopIndices = new int[_reels.Length];
            }

            for (int index = 0; index < _reels.Length; index++)
            {
                _reels[index].Reel.OnStoped += OnReelStop;
                _reels[index].Reel.Initialize(_reels[index].Sprites, _stopHeightOffset);
            }
        }
        //[2026/01/27] プリンス start
        SoundManager.Instance.PlayTitleBGM();
        //[2026/01/27] プリンス end
        _currentPlay = 0;
        _winEffect = GetComponentInChildren<V_BlinkEffect>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            Debug.Log("必ず当たる");
            _forceCorrect = true;
        }
    }

    /// <summary>
    /// ロールを回られる
    /// </summary>
    public void OnClickStart()
    {
        if (IsRunning)
            return;

        SpinRoutine();
        _reelFinishedNum = 0x00;
        _forceCorrect = false;
        _currentPlay++;
        if (_currentPlay >= _maxPlay)
        {
            _forceCorrect = true;
            _currentPlay = 0;
        }
    }

    /// <summary>
    /// 全部のロールを一気に停止
    /// </summary>
    public void OnStopAllReel()
    {
        if (!IsRunning)
            return;

        // もしランダムの結果が欲しいなら、ここで決める
        int[] result = _randomizeResult ? GenerateRandomResult() : _stopIndices;

        StartCoroutine(StopRoutine(result));
    }

    public void StopReel(int index)
    {
        if (!CustomControll)
            return;

        if (!IsRunning)
            return;

        if (index < 0 || index >= _reels.Length)
            return;

        int targetIndex = _forceCorrect ? _stopIndices[index] : -1;
        _reels[index].Reel.StopSpin(targetIndex);
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
        //[2026/01/27] プリンス start
        //SoundManager.Instance.StartSlotSpinSE();
        //Debug.LogWarning("play spin SE");
        //[2026/01/27] プリンス end
        IsRunning = true;

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
        //[2026/01/27] プリンス start
        //SoundManager.Instance.StopSlotSpinSE();
        //Debug.LogWarning("STOP spin SE");
        //[2026/01/27] プリンス end
        for (int i = 0; i < _reels.Length; i++)
        {
            int targetIndex = result[i];
            _reels[i].Reel.StopSpin(targetIndex);

            yield return new WaitForSeconds(_stopDelay);
        }
    }

    /// <summary>
    /// ロールが停止した
    /// </summary>
    private void OnReelStop(int reelID, int resultIndex)
    {
        if (reelID < 0 || reelID >= _reelResult.Length)
            return;

        Debug.Log($"ID: {reelID}, result: {resultIndex}");
        _reelResult[reelID] = resultIndex;
        _reelFinishedNum++;

        if (_reelFinishedNum == _reels.Length)
        {
            ResultType result = CheckResult(_reelResult);
            if (result == ResultType.Start)
                _winEffect.StartBlinking();
            OnFinished?.Invoke(result);
            IsRunning = false;
        }
    }

    /// <summary>
    /// 各ロールの結果を確認
    /// </summary>
    /// <param name="reelResult">各ロールの結果</param>
    /// <returns>true: あたり、false: 当たってない</returns>
    private ResultType CheckResult(int[] reelResult)
    {
        if (CheckIsContain(_closeResults, reelResult))
            return ResultType.Close;
        else if (CheckIsContain(_startResults, reelResult))
            return ResultType.Start;
        else
            return ResultType.Miss;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="wants"></param>
    /// <param name="reelResult"></param>
    /// <returns></returns>
    private bool CheckIsContain(WantResult[] wants, int[] reelResult)
    {
        for (int index = 0; index < reelResult.Length; index++)
        {
            if (!wants[index].Indices.Contains(reelResult[index]))
                return false;
        }
        return true;
    }
}
