using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleButton : MonoBehaviour
{
    // 2026.01.21 ウー start
    [Tooltip("最初に起動するまでの時間")]
    [SerializeField]
    private float _startDelay = 5f;

    [Tooltip("止まるまでの時間")]
    [SerializeField]
    private float _stopDelay = 3f;

    [Tooltip("再び回るまでの時間")]
    [SerializeField]
    private float _nextDelay = 3f;

    /// <summary>
    /// スロット
    /// </summary>
    [SerializeField]
    private V_TitleSlot _slot;

    /// <summary>
    /// キャンセル
    /// </summary>
    private CancellationTokenSource _cancellationTokenSource;
    // 2026.01.21 ウー end

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 2026.01.21 ウー start
        if (_slot)
        {
            _slot.OnFinished += OnSlotFinished;
            StartSlot(_startDelay).Forget();
        }
        // 2026.01.21 ウー end
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnButtonDown_Start()
    {
        // 2026.01.21 ウー start
        CancelAction();
        // 2026.01.21 ウー end
        SceneManager.LoadScene("ModeSelect", LoadSceneMode.Single);
    }

    public void OnButtonDown_Quit()
    {
        // 2026.01.21 ウー start
        CancelAction();
        // 2026.01.21 ウー end
        Application.Quit();
    }

    // 2026.01.21 ウー start
    /// <summary>
    /// スロットを回せる
    /// </summary>
    /// <param name="delay">ディレイ秒数</param>
    private async UniTaskVoid StartSlot(float delay)
    {
        _cancellationTokenSource = new CancellationTokenSource();
        CancellationToken token = _cancellationTokenSource.Token;

        try
        {
            await UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: token);
            _slot.OnClickStart();
            await UniTask.Delay(TimeSpan.FromSeconds(_stopDelay), cancellationToken: token);
            _slot.OnClickStop();
        }
        catch (OperationCanceledException exp)
        {
            Debug.Log("Delay was cancelled!");
        }
        finally
        {
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
        }
    }

    /// <summary>
    /// スロットが停止した
    /// </summary>
    private void OnSlotFinished()
    {
        // 次回
        StartSlot(_nextDelay).Forget();
    }

    /// <summary>
    /// 待つのをキャンセル
    /// </summary>
    public void CancelAction()
    {
        // Check if the source exists and is not already cancelled
        if (_cancellationTokenSource != null && !_cancellationTokenSource.IsCancellationRequested)
        {
            _cancellationTokenSource.Cancel();
        }
    }

    /// <summary>
    /// 破壊された
    /// </summary>
    private void OnDestroy()
    {
        // Ensure cancellation and cleanup when the GameObject is destroyed
        CancelAction();
        _cancellationTokenSource?.Dispose();
    }
    // 2026.01.21 ウー end
}
