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
    /// 検索入力欄
    /// </summary>
    [SerializeField]
    private TMP_InputField _searchInput;

    /// <summary>
    /// 部屋を表示するところ
    /// </summary>
    [SerializeField]
    private GameObject _content;

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
        if (_searchInput)
            _searchInput.onEndEdit.AddListener(OnSearchEditEnd);
        _myController = GetComponent<C_RoomList>();
        _myController.OnCreated += OnCreatedNewRoom;
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
    /// 新しい部屋が作成されたときの処理
    /// </summary>
    /// <param name="room">部屋</param>
    private void OnCreatedNewRoom(GameObject room)
    {
        room.transform.SetParent(_content.transform);
    }

    private void OnSearchEditEnd(string text)
    {
        ReloadRoomList();
    }
}
