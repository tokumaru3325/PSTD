using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class V_RoomCreator : MonoBehaviour
{
    /// <summary>
    /// 作成ボタン
    /// </summary>
    [SerializeField]
    private Button _createBtn;

    /// <summary>
    /// キャンセルボタン
    /// </summary>
    [SerializeField]
    private Button _cancelBtn;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (_createBtn)
            _createBtn.onClick.AddListener(CreateRoom);
        if (_cancelBtn)
            _cancelBtn.onClick.AddListener(CancelCreatingRoom);
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// 部屋を作る
    /// </summary>
    public void CreateRoom()
    {
        // データを準備

        // シーンを変更
        SceneManager.LoadScene("Room", LoadSceneMode.Single);
    }

    /// <summary>
    /// 部屋を作るのをやめる
    /// </summary>
    public void CancelCreatingRoom()
    {
        // 初期化

        // シーンを変更
        SceneManager.LoadScene("Title", LoadSceneMode.Single);
    }
}
