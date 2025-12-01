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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (_backBtn)
            _backBtn.onClick.AddListener(BackToSelectMode);
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
}
