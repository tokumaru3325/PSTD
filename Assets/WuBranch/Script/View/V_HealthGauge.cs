using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Runtime.CompilerServices;
using UnityEngine.InputSystem;

public class V_HealthGauge : MonoBehaviour
{
    [SerializeField]
    private Image _healthImage;

    [SerializeField]
    private Image _burnImage;

    /// <summary>
    /// アニメーションの継続時間
    /// </summary>
    [SerializeField]
    private float _duration = 0.5f;

    /// <summary>
    /// 現在の体力率
    /// </summary>
    private float _currentHealthRate = 1.0f;

    void Start()
    {
        // 初期化
        _currentHealthRate = 1.0f;
        _healthImage.fillAmount = _currentHealthRate;
        _burnImage.fillAmount = _currentHealthRate;
    }

    public void SetGauge(float targetRate)
    {
        _currentHealthRate = targetRate;
        _healthImage.DOFillAmount(targetRate, _duration).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            _burnImage.DOFillAmount(targetRate, _duration * 0.5f).SetDelay(0.25f);
        });
    }

    public void HideGauge()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.DOSizeDelta(new Vector2(-160, -20), 0.5f).SetEase(Ease.OutBack);
    }

    public void ShowGauge()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.DOSizeDelta(new Vector2(0, 0), 0.5f).SetEase(Ease.OutBack);
    }
}
