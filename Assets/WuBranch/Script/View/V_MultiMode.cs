using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class V_MultiMode : MonoBehaviour
{
    /// <summary>
    /// ロビーを作るボタン
    /// </summary>
    [SerializeField]
    private Button _hostBtn;

    /// <summary>
    /// ロビーを探すボタン
    /// </summary>
    [SerializeField]
    private Button _clientBtn;

    /// <summary>
    /// バックボタン
    /// </summary>
    [SerializeField]
    private Button _backBtn;

    /// <summary>
    /// ひとつ前の画面
    /// </summary>
    [SerializeField]
    private GameObject _preScene;

    void Awake()
    {
        gameObject.SetActive(false);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (_hostBtn)
            _hostBtn.onClick.AddListener(CreateLobby);
        if (_clientBtn)
            _clientBtn.onClick.AddListener(SearchLobby);
        if (_backBtn)
            _backBtn.onClick.AddListener(BackToTitle);
    }

    /// <summary>
    /// ロビーを作るシーンに行く
    /// </summary>
    public void CreateLobby()
    {
        SceneManager.LoadScene("RoomCreate", LoadSceneMode.Single);
    }

    /// <summary>
    /// ロビーを探すシーンに行く
    /// </summary>
    public void SearchLobby()
    {
        SceneManager.LoadScene("RoomList", LoadSceneMode.Single);
    }

    /// <summary>
    /// 前の画面に戻る
    /// </summary>
    public void BackToTitle()
    {
        gameObject.SetActive(false);
        _preScene.SetActive(true);
    }
}
