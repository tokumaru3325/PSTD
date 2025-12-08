using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(C_RoomList))]
public class V_RoomList : MonoBehaviour
{
    /// <summary>
    /// バックボタン
    /// </summary>
    [SerializeField]
    private Button _backBtn;

    /// <summary>
    /// 部屋を表示するところ
    /// </summary>
    [SerializeField]
    private GameObject _content;

    /// <summary>
    /// コントローラ
    /// </summary>
    private C_RoomList _myController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (_backBtn)
            _backBtn.onClick.AddListener(BackToSelectMode);
        _myController = GetComponent<C_RoomList>();
        _myController.OnCreated += OnCreatedNewRoom;
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// 前の画面に戻る
    /// </summary>
    public void BackToSelectMode()
    {
        SceneManager.LoadScene("Title", LoadSceneMode.Single);
    }

    private void OnCreatedNewRoom(GameObject room)
    {
        room.transform.SetParent(_content.transform);
    }
}
