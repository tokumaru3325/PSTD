using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(C_RoomList))]
public class V_RoomList : MonoBehaviour
{
    /// <summary>
    /// バックボタン
    /// </summary>
    [SerializeField]
    private Button _backBtn;

    /// <summary>
    /// リロードボタン
    /// </summary>
    [SerializeField]
    private Button _reloadBtn;

    /// <summary>
    /// 作成ボタン
    /// </summary>
    [SerializeField]
    private Button _createBtn;

    /// <summary>
    /// 検索入力欄
    /// </summary>
    [SerializeField]
    private TMP_InputField _searchInput;

    /// <summary>
    /// 受付を表示するところ
    /// </summary>
    [SerializeField]
    private GameObject _content;

    /// <summary>
    /// 部屋を作るパネル
    /// </summary>
    [SerializeField]
    private GameObject _createPanel;

    /// <summary>
    /// コントローラ
    /// </summary>
    private C_RoomList _myController;

    /// <summary>
    /// 部屋を探すもの
    /// </summary>
    private C_RoomSeeker _seeker;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (_backBtn)
            _backBtn.onClick.AddListener(BackToSelectMode);
        if (_reloadBtn)
            _reloadBtn.onClick.AddListener(ReloadRoomList);
        if (_createBtn)
            _createBtn.onClick.AddListener(OpenCreateRoomScene);
        if (_searchInput)
            _searchInput.onEndEdit.AddListener(OnSearchEditEnd);
        _myController = GetComponent<C_RoomList>();
        _myController.OnCreated += OnCreatedRoomFront;
        _seeker = FindFirstObjectByType<C_RoomSeeker>();
    }

    /// <summary>
    /// 前の画面に戻る
    /// </summary>
    public void BackToSelectMode()
    {
        SceneManager.LoadScene("ModeSelect", LoadSceneMode.Single);
    }

    /// <summary>
    /// リロード
    /// </summary>
    private void ReloadRoomList()
    {
        string condition = "";
        if (_searchInput)
            condition = _searchInput.text;
        _seeker.FindRoom(condition);
    }

    /// <summary>
    /// 部屋を作るパネルを開け
    /// </summary>
    private void OpenCreateRoomScene()
    {
        if (_createPanel)
            _createPanel.SetActive(true);
    }

    /// <summary>
    /// 新しい受付が作成されたときの処理
    /// </summary>
    /// <param name="room">受付</param>
    private void OnCreatedRoomFront(GameObject room)
    {
        room.transform.SetParent(_content.transform);
    }

    /// <summary>
    /// キーワード入力完了
    /// </summary>
    /// <param name="text"></param>
    private void OnSearchEditEnd(string text)
    {
        ReloadRoomList();
    }
}
