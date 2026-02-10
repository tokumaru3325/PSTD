using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// 2026.01.21 ウー start
[Serializable]
public class BtnImg
{
    [Tooltip("押した状態の画像")]
    [SerializeField]
    public Sprite pressed;

    [Tooltip("普通の状態の画像")]
    [SerializeField]
    public Sprite unPress;
}
// 2026.01.21 ウー end

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
    private C_TitleSlot _slot;

    /// <summary>
    /// キャンセル
    /// </summary>
    private CancellationTokenSource _cancellationTokenSource;

    [Tooltip("スタートボタン")]
    [SerializeField]
    private Image _startBtn;

    [Tooltip("スタートボタンの画像")]
    [SerializeField]
    private BtnImg _startImg;

    [Tooltip("各ロールの止まるボタン")]
    [SerializeField]
    private Image[] _reelController;

    [Tooltip("各ロールの画像")]
    [SerializeField]
    private BtnImg[] _reelsImg;
    // 2026.01.21 ウー end

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 2026.01.21 ウー start
        if (_slot)
        {
            _slot.OnFinished += OnSlotFinished;
            if (!_slot.CustomControll)
                StartSlot(_startDelay).Forget();
        }

        // 2026.01.21 ウー end
    }

    public void OnButtonDown_Start()
    {
        // 2026.01.21 ウー start
        if (_slot.CustomControll)
        {
            if (_slot.IsRunning)
                return;

            _slot.OnClickStart();
            _startBtn.sprite = _startImg.pressed;
            for (int index = 0; index < _reelController.Length; index++)
            {
                _reelController[index].sprite = _reelsImg[index].unPress;
            }
        }
        else
        {
            // 2026.01.21 ウー start
            CancelAction();
            // 2026.01.21 ウー end
            SceneManager.LoadScene("GameCopyT", LoadSceneMode.Single);
        }
        // 2026.01.21 ウー end
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
    /// ロールを停止させる
    /// </summary>
    /// <param name="index">ロールのインデックス</param>
    public void StopReel(int index)
    {
        _slot.StopReel(index);
        _reelController[index].sprite = _reelsImg[index].pressed;
    }

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
            _slot.OnStopAllReel(true);
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
    private void OnSlotFinished(SlotResultType result)
    {
        // 次回
        if (!_slot.CustomControll)
            StartSlot(_nextDelay).Forget();
        else
        {
            _startBtn.sprite = _startImg.unPress;
            if (result == SlotResultType.Start)
                ChangeToGameScene().Forget();
            else if (result == SlotResultType.Close)
                CloseGame().Forget();
        }
    }

    /// <summary>
    /// ゲームシーンに行く
    /// </summary>
    private async UniTaskVoid ChangeToGameScene()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: this.GetCancellationTokenOnDestroy());
        C_PathSearch.ResetSearchMap();
        SceneManager.LoadScene("GameCopyT", LoadSceneMode.Single);
    }

    /// <summary>
    /// ゲーム終了
    /// </summary>
    private async UniTaskVoid CloseGame()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: this.GetCancellationTokenOnDestroy());
        Application.Quit();
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
