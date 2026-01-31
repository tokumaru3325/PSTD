using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class V_BlinkEffect : MonoBehaviour
{

    [Header("フラッシュ設定")]
    [Tooltip("変更する時間")]
    [SerializeField]
    private float _fadeDuration = 0.5f;

    [Tooltip("見える時の透明度")]
    [SerializeField]

    private float _maxAlpha = 1.0f;

    /// <summary>
    /// 変更対象、自身が持ってるImage
    /// </summary>
    private Image _targetImage;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _targetImage = GetComponent<Image>();
    }

    /// <summary>
    /// エフェクト開始
    /// </summary>
    public void StartBlinking()
    {
        if (!_targetImage)
            return;

        // 透明度の初期化
        Color c = _targetImage.color;
        c.a = 0f;
        _targetImage.color = c;

        // アニメーション開始
        _targetImage.DOFade(_maxAlpha, _fadeDuration) // 目標の透明度、時間
            .SetLoops(-1, LoopType.Yoyo)           // -1 は無限ループ, Yoyo は往復で変化する
            .SetEase(Ease.InOutSine)
            .SetLink(gameObject);                  // このオブジェクトにバインド。オブジェクトが消されたら、アニメーションも止まる
    }
}
