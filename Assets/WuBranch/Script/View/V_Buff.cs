using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class V_Buff : MonoBehaviour
{
    /// <summary>
    /// バフの画像
    /// </summary>
    [SerializeField]
    private Image _iconImg;

    /// <summary>
    /// バフの残り時間
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI _timeTxt;

    [Header("アニメーション設定")]
    [Tooltip("アニメショーンする対象")]
    [SerializeField]
    private GameObject _animationTarget;

    [Tooltip("移動距離")]
    [SerializeField]
    private float _startOffsetX = 500f;

    [Tooltip("移動時間(秒)")]
    [SerializeField]
    private float _moveDuration = 2f;

    [Tooltip("スライドイン効果")]
    [SerializeField]
    private Ease _slideInType = Ease.OutQuad;

    [Tooltip("拡大時間(秒)")]
    [SerializeField]
    private float _scaleDuration = 2f;

    [Tooltip("スライドアウト効果")]
    [SerializeField]
    private Ease _slideOutType = Ease.OutQuad;

    private RectTransform rectTransform;

    void Awake()
    {
        if (!_iconImg)
            Debug.LogError("Didnot find buff icon");

        if (!_timeTxt)
            Debug.LogError("Didnot find buff time");

        rectTransform = _animationTarget.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(_startOffsetX, rectTransform.anchoredPosition.y);
        rectTransform.localScale = new Vector3(1, 0, 1);
    }

    /// <summary>
    /// 残り秒数を更新
    /// </summary>
    /// <param name="time">秒数</param>
    public void UpdateTime(float time)
    {
        float min = time / 60;
        float sec = time % 60;
        if ((int)min == 0 && sec <= 10.0f)
        {
            // 小数点以下1桁を表示する
            _timeTxt.text = sec.ToString("0.#");
            if (sec <= 5.0f)
                _timeTxt.color = Color.red;
            else
                _timeTxt.color = Color.orange;
        }
        else if ((int)min == 0)
        {
            _timeTxt.text = $"{(int)sec}";
        }
        else
        {
            _timeTxt.text = $"{(int)min}:{(int)sec}";
        }
    }

    /// <summary>
    /// アイコンを設定
    /// </summary>
    /// <param name="icon"></param>
    public void SetIcon(Sprite icon)
    {
        _iconImg.sprite = icon;
    }

    /// <summary>
    /// スライドイン
    /// </summary>
    public void SlideIn()
    {
        Sequence mySequence = DOTween.Sequence();
        // 移動すると同時に拡大
        mySequence.Append(
            rectTransform.DOAnchorPosX(0, _moveDuration).SetEase(_slideInType)
        );
        // 拡大
        mySequence.Join(
            rectTransform.DOScaleY(1, _scaleDuration).SetEase(_slideInType)
        );
    }

    /// <summary>
    /// スライドアウト
    /// </summary>
    public void SlideOut()
    {
        Sequence mySequence = DOTween.Sequence();
        // 移動すると同時に縮小
        mySequence.Append(
            rectTransform.DOAnchorPosX(_startOffsetX, _moveDuration).SetEase(_slideOutType)
        );
        // 縮小
        mySequence.Join(
            rectTransform.DOScaleY(0, _scaleDuration).SetEase(_slideOutType)
        );
        // アニメショーン終了
        mySequence.OnComplete(() =>
        {
            Destroy(gameObject);
        });
    }
}
