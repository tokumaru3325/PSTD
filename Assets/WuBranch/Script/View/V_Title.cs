using UnityEngine;
using UnityEngine.UI;

public class V_Title : MonoBehaviour
{
    /// <summary>
    /// 開始ボタン
    /// </summary>
    [SerializeField]
    private Button _startBtn;

    /// <summary>
    /// 終了ボタン
    /// </summary>
    [SerializeField]
    private Button _exitBtn;

    /// <summary>
    /// 次で見せたい画面
    /// </summary>
    [SerializeField]
    private GameObject _next;

    void Awake()
    {
        gameObject.SetActive(true);
    }

    void Start()
    {
        if (_startBtn)
            _startBtn.onClick.AddListener(StartGame);
        if (_exitBtn)
            _exitBtn.onClick.AddListener(ExitGame);
    }

    /// <summary>
    /// ゲームに入る
    /// </summary>
    public void StartGame()
    {
        gameObject.SetActive(false);
        _next.SetActive(true);
    }

    /// <summary>
    /// ゲーム終了
    /// </summary>
    public void ExitGame()
    {
        Application.Quit();
    }
}
