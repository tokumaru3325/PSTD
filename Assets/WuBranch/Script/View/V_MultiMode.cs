using TMPro;
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
    /// 名前の入力
    /// </summary>
    [SerializeField]
    private TMP_InputField _nameInput;

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

    /// <summary>
    /// 共通変数
    /// </summary>
    private C_GlobalVariable _globalVariable;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    async void Start()
    {
        if (_singleBtn)
            _singleBtn.onClick.AddListener(OpenSingle);
        if (_multiBtn)
            _multiBtn.onClick.AddListener(OpenMulti);
        if (_hostBtn)
            _hostBtn.onClick.AddListener(SearchLobby);
        if (_clientBtn)
            _clientBtn.onClick.AddListener(SearchLobby);
        if (_backBtn)
            _backBtn.onClick.AddListener(OpenMode);
        if (_nameInput)
            _nameInput.onValueChanged.AddListener(OnNameInputChanged);
        _globalVariable = FindFirstObjectByType<C_GlobalVariable>();
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
        _globalVariable.SetMyName(_nameInput.text);
        _nameInput.text = "";
        SceneManager.LoadScene("RoomCreate", LoadSceneMode.Single);
    }

    /// <summary>
    /// ロビーを探すシーンに行く
    /// </summary>
    public void SearchLobby()
    {
        _globalVariable.SetMyName(_nameInput.text);
        _nameInput.text = "";
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

    public void OnNameInputChanged(string value)
    {
        if (value.Length == 0)
        {
            _hostBtn.interactable = false;
            _clientBtn.interactable = false;
        }
        else
        {
            _hostBtn.interactable = true;
            _clientBtn.interactable = true;
        }
    }
}
