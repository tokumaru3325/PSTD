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
    /// 共通変数
    /// </summary>
    private C_GlobalVariable _variables;

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
        _variables = FindFirstObjectByType<C_GlobalVariable>();
    }

    /// <summary>
    /// ロビーを作る
    /// </summary>
    public void CreateLobby()
    {
        if (_variables)
        {
            _variables.SetMultiRole(MultiRoleType.Host);
            SceneManager.LoadScene("Multi", LoadSceneMode.Single);
        }
        else
            Debug.LogError("Did not find variable!");
    }

    /// <summary>
    /// ロビーを探す
    /// </summary>
    public void SearchLobby()
    {
        if (_variables)
        {
            _variables.SetMultiRole(MultiRoleType.Client);
            SceneManager.LoadScene("Multi", LoadSceneMode.Single);
        }
        else
            Debug.LogError("Did not find variable!");
    }
}
