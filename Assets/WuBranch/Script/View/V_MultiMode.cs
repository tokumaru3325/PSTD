using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class V_MultiMode : MonoBehaviour
{
    /// <summary>
    /// 一人モードボタン
    /// </summary>
    [SerializeField]
    private Button _singleBtn;

    /// <summary>
    /// マルチモードボタン
    /// </summary>
    [SerializeField]
    private Button _multiBtn;

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
    /// モード画面
    /// </summary>
    [SerializeField]
    private GameObject _modeScene;

    /// <summary>
    /// マルチ画面
    /// </summary>
    [SerializeField]
    private GameObject _multiScene;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    async void Start()
    {
        if (_singleBtn)
            _singleBtn.onClick.AddListener(OpenSingle);
        if (_multiBtn)
            _multiBtn.onClick.AddListener(OpenMulti);
        if (_hostBtn)
            _hostBtn.onClick.AddListener(CreateLobby);
        if (_clientBtn)
            _clientBtn.onClick.AddListener(SearchLobby);
        if (_backBtn)
            _backBtn.onClick.AddListener(OpenMode);
    }

    public void OpenSingle()
    {
        SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }

    public void OpenMulti()
    {
        _modeScene.SetActive(false);
        _multiScene.SetActive(true);
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
    /// モードシーン
    /// </summary>
    public void OpenMode()
    {
        _modeScene.SetActive(true);
        _multiScene.SetActive(false);
    }
}
