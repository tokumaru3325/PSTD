using UnityEngine;
using UnityEngine.UI;

public class V_CastleViewer : MonoBehaviour
{
    /// <summary>
    /// 城の表示画像
    /// </summary>
    [SerializeField]
    private Image _viewr;

    /// <summary>
    /// 全部の城の画像
    /// </summary>
    [SerializeField]
    private Sprite[] _castleSprites;

    /// <summary>
    /// 現在の城のインデックス
    /// </summary>
    private int _currentIndex;

    /// <summary>
    /// 前の城ボタン
    /// </summary>
    [SerializeField]
    private Button _preBtn;

    /// <summary>
    /// 次の城ボタン
    /// </summary>
    [SerializeField]
    private Button _nextBtn;

    void Start()
    {
        _currentIndex = 0;
        UpdateCastle();
    }

    /// <summary>
    /// 前の城
    /// </summary>
    public void PreCastle()
    {
        _currentIndex--;
        if (_currentIndex < 0)
        {
            _currentIndex = _castleSprites.Length - 1;
        }
        UpdateCastle();
    }

    /// <summary>
    /// 次の城
    /// </summary>
    public void NextCastle()
    {
        _currentIndex++;
        if (_currentIndex >= _castleSprites.Length)
        {
            _currentIndex = 0;
        }
        UpdateCastle();
    }

    /// <summary>
    /// 城の更新
    /// </summary>
    private void UpdateCastle()
    {
        _viewr.sprite = _castleSprites[_currentIndex];
    }

    /// <summary>
    /// クライアントモード時の表示設定
    /// </summary>
    public void InClientMode()
    {
        _preBtn.gameObject.SetActive(false);
        _nextBtn.gameObject.SetActive(false);
    }

    /// <summary>
    /// ホストモード時の表示設定
    /// </summary>
    public void InHostMode()
    {
        _preBtn.gameObject.SetActive(true);
        _nextBtn.gameObject.SetActive(true);
    }
}
