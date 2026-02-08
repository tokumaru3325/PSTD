using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class C_TitleSlot : MonoBehaviour
{

    [Tooltip("ビュー")]
    [SerializeField]
    private V_TitleSlot _view;

    [Tooltip("各ロールのコントローラー")]
    [SerializeField]
    private C_SlotReel[] _reelControllers;

    [Tooltip("スロット初期データ")]
    [SerializeField]
    private M_TitleSlotData _data;

    public bool CustomControll => _model.CustomControll;

    private M_TitleSlot _model;

    /// <summary>
    /// 今回の結果
    /// </summary>
    private int[] _currentResults;

    /// <summary>
    /// 停止したロールの数
    /// </summary>
    private int _finishedReelCount;

    /// <summary>
    /// 完全停止した後の処理
    /// </summary>
    public Action<SlotResultType> OnFinished;

    void Awake()
    {
        if (!_data)
        {
            Debug.LogError("Title slot's data didnot set!");
            return;
        }

        _model = new M_TitleSlot(_data);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _currentResults = new int[_reelControllers.Length];
        _view.SetReelsSprite(_data.Reels);

        // 各ロールの初期化
        for (int index = 0; index < _reelControllers.Length; index++)
        {
            var controller = _reelControllers[index];
            controller.OnStopped += OnSingleReelStopped;
            controller.Initialize(controller.GetComponent<V_SlotReel>(), index, _data.Reels[index].Sprites, _model.StopHeightOffset);
        }

        //[2026/01/27] プリンス start
        SoundManager.Instance.PlayTitleBGM();
        //[2026/01/27] プリンス end
        _model.CurrentPlayCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            Debug.Log("必ず当たる");
            _model.ForceCorrect = true;
        }
    }

    public bool IsRunning => _model.IsRunning;

    /// <summary>
    /// ロールを回させる
    /// </summary>
    public void OnClickStart()
    {
        if (_model.IsRunning)
            return;

        //[2026/01/27] プリンス start
        //SoundManager.Instance.StartSlotSpinSE();
        //Debug.LogWarning("play spin SE");
        //[2026/01/27] プリンス end

        _model.IsRunning = true;
        _model.ForceCorrect = false;
        _model.CurrentPlayCount++;
        _finishedReelCount = 0;

        // MaxPlayBeforeForceWin回目の時、強制的に
        if (_model.CurrentPlayCount >= _model.MaxPlayBeforeForceWin)
        {
            _model.ForceCorrect = true;
            _model.CurrentPlayCount = 0;
        }

        // 各ロールを回させる
        foreach (var reel in _reelControllers)
            reel.StartSpin();
    }

    /// <summary>
    /// 全部のロールを一気に停止
    /// </summary>
    public void OnStopAllReel(bool randomize)
    {
        if (!_model.IsRunning)
            return;

        // もしランダムの結果が欲しいなら、ここで決める
        int[] finalTarget;
        if (randomize)
        {
            int[] symbolCounts = _view.ReelSprites.Select(r => r.Sprites.Length).ToArray();
            finalTarget = _model.GenerateRandomResult(_reelControllers.Length, symbolCounts);
        }
        else
        {
            finalTarget = _model.StopIndices;
        }

        StartCoroutine(StopSequence(finalTarget));
    }

    /// <summary>
    /// 個別のロールを停止させる
    /// </summary>
    /// <param name="index">ロールのインデックス</param>
    public void StopReel(int index)
    {
        if (!_model.CustomControll)
            return;

        if (!_model.IsRunning)
            return;

        if (index < 0 || index >= _reelControllers.Length)
            return;

        int targetIndex = _model.ForceCorrect ? _model.StopIndices[index] : -1;
        _reelControllers[index].StopSpin(targetIndex);
    }

    /// <summary>
    /// 止める
    /// </summary>
    /// <param name="result">止まってほしい結果</param>
    private IEnumerator StopSequence(int[] targets)
    {
        //[2026/01/27] プリンス start
        //SoundManager.Instance.StopSlotSpinSE();
        //Debug.LogWarning("STOP spin SE");
        //[2026/01/27] プリンス end
        for (int index = 0; index < _reelControllers.Length; index++)
        {
            _reelControllers[index].StopSpin(targets[index]);
            yield return new WaitForSeconds(_model.StopDelay);
        }
    }

    /// <summary>
    /// ロールが停止した
    /// </summary>
    /// <param name="reelID">ロールのID</param>
    /// <param name="resultIndex">止まっている画像のインデックス</param>
    private void OnSingleReelStopped(int reelID, int resultIndex)
    {
        if (reelID < 0 || reelID >= _reelControllers.Length)
            return;

        Debug.Log($"ID: {reelID}, result: {resultIndex}");
        _currentResults[reelID] = resultIndex;
        _finishedReelCount++;

        if (_finishedReelCount == _reelControllers.Length)
        {
            var result = _model.CheckResult(_currentResults);
            if (result == SlotResultType.Start)
                _view.StartWinEffect();

            OnFinished?.Invoke(result);
            _model.IsRunning = false;
        }
    }

    /// <summary>
    /// 欲しい結果を設定
    /// </summary>
    /// <param name="result">結果</param>
    public void SetResult(int[] result)
    {
        if (result.Length != _reelControllers.Length)
        {
            Debug.LogError("設定したい結果の数がロールの数とあっていない");
            return;
        }
        _model.StopIndices = result;
    }
}
