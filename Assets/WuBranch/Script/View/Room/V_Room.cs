using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class V_Room : MonoBehaviour
{
    /// <summary>
    /// バックボタン
    /// </summary>
    [SerializeField]
    private Button _backBtn;

    /// <summary>
    /// 部屋名表示テキスト
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI _roomNameTxt;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (_backBtn)
            _backBtn.onClick.AddListener(BackToSelectMode);
    }

    /// <summary>
    /// 前の画面に戻る
    /// </summary>
    public void BackToSelectMode()
    {
        NetworkManager.Singleton.SceneManager.LoadScene("RoomList", LoadSceneMode.Single);
    }


}
