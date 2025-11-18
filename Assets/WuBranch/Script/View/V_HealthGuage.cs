using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Runtime.CompilerServices;
using UnityEngine.InputSystem;

public class V_HealthGuage : MonoBehaviour
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

    /// <summary>
    /// 振動の強さ
    /// </summary>
    [SerializeField]
    private float _strength = 20.0f;

    /// <summary>
    /// 振動の細かさ
    /// </summary>
    [SerializeField]
    private int _vibrate = 100;

    void Start()
    {
        // 初期化
        _currentHealthRate = 1.0f;
        _healthImage.fillAmount = _currentHealthRate;
        _burnImage.fillAmount = _currentHealthRate;
    }

    void Update()
    {
    }

    public void SetGuage(float targetRate)
    {
        _currentHealthRate = targetRate;
        _healthImage.DOFillAmount(targetRate, _duration).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            _burnImage.DOFillAmount(targetRate, _duration * 0.5f).SetDelay(0.25f);
        });
        transform.DOShakePosition(_duration * 0.5f, _strength, _vibrate);
    }
}
