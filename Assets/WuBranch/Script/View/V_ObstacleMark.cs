using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class V_ObstacleMark : MonoBehaviour
{

    /// <summary>
    /// 指す目標
    /// </summary>
    private Vector3 _target;

    [Tooltip("UIの画面端からの内側余白")]
    public float borderMargin = 50f;

    [Tooltip("マークのデフォルト方向. 上: 90,右: 0")]
    public float rotationOffset = 0f;

    [Header("アニメショーン設定")]
    [Tooltip("Image")]
    [SerializeField]
    private Transform _targetImage;

    [Tooltip("移動距離")]
    [SerializeField]
    private float moveDistance = 10f;

    [Tooltip("移動時間")]
    [SerializeField]

    private float animDuration = 0.5f;

    /// <summary>
    /// このUIを置いたCanvas
    /// </summary>
    private Canvas parentCanvas;

    /// <summary>
    /// カメラ
    /// </summary>
    private Camera _mainCamera;
    /// <summary>
    /// CanvasのレンダーモードがScreen Space-Cameraのため
    /// </summary>
    private Camera _uiCamera;

    /// <summary>
    /// 自身のトランスフォーム
    /// </summary>
    private RectTransform _rectTransform;

    /// <summary>
    /// 実際表示するcanvas
    /// </summary>
    private RectTransform _canvasRectTransform;

    /// <summary>
    /// 目標を設定したか
    /// </summary>
    private bool _IsSettedTarget = false;

    /// <summary>
    /// アニメーション
    /// </summary>
    private Tweener _moveTween;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _mainCamera = Camera.main;
        _rectTransform = GetComponent<RectTransform>();

        parentCanvas = GetComponentInParent<Canvas>();
        if (parentCanvas != null)
        {
            _uiCamera = parentCanvas.worldCamera;
            _canvasRectTransform = parentCanvas.GetComponent<RectTransform>();
        }
        InitAnim();
    }

    private void InitAnim()
    {
        // アニメショーン
        if (_targetImage != null)
        {
            // 初期位置
            _targetImage.localPosition = Vector3.zero;

            // x軸に沿って往復する動画を作成
            // SetLink: オブジェクトが削除されたら、アニメーションを止める
            // SetAutoKill(false): 一時停止/再開できるように
            _moveTween = _targetImage.DOLocalMoveX(moveDistance, animDuration)
                .SetRelative(true)      // ローカル移動
                .SetLoops(-1, LoopType.Yoyo) // ループ
                .SetEase(Ease.InOutSine)
                .SetLink(gameObject)
                .SetAutoKill(false)
                .Pause();
        }
    }

    void Update()
    {
        PointToTarget();
    }

    /// <summary>
    /// 目標を指す
    /// </summary>
    private void PointToTarget()
    {
        if (!_IsSettedTarget || _mainCamera == null || _uiCamera == null || _canvasRectTransform == null)
            return;

        // 世界座標をモニター座標に変更
        Vector3 targetScreenPos = _mainCamera.WorldToScreenPoint(_target);

        // 画面内だったら表示しない
        if (targetScreenPos.x > 0 && targetScreenPos.x < Screen.width && targetScreenPos.y > 0 && targetScreenPos.y < Screen.height)
        {
            _targetImage.gameObject.SetActive(false);
            _moveTween.Pause();
            return;
        }
        else
        {
            _targetImage.gameObject.SetActive(true);
            _moveTween.Play();
        }

        Vector3 clampedPos = targetScreenPos;

        // 画面にとどまる
        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
        clampedPos.x = Mathf.Clamp(clampedPos.x, borderMargin, Screen.width - borderMargin);
        clampedPos.y = Mathf.Clamp(clampedPos.y, borderMargin, Screen.height - borderMargin);
        // 設置
        Vector2 localPoint;
        // 計算した座標をUIのローカル座標に変換
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvasRectTransform, clampedPos, _uiCamera, out localPoint);
        _rectTransform.localPosition = localPoint;
        // 方向
        Vector3 dir = targetScreenPos - screenCenter;
        // 角度
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        _rectTransform.rotation = Quaternion.Euler(0, 0, angle + rotationOffset);
    }

    /// <summary>
    /// 目標を設定
    /// </summary>
    /// <param name="target"></param>
    public void SetTarget(Vector3 target)
    {
        _target = target;
        _IsSettedTarget = true;
    }

    /// <summary>
    /// 目標をゲット
    /// </summary>
    /// <returns></returns>
    public Vector3 GetTarget()
    {
        return _target;
    }

    void OnDestroy()
    {
        _moveTween?.Kill();
    }

}
