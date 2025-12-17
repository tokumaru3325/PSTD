using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(C_Host))]
public class V_Host : MonoBehaviour
{
    /// <summary>
    /// プレイヤー名表示欄
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI _playerNameTxt;

    /// <summary>
    /// 城
    /// </summary>
    [SerializeField]
    private V_CastleViewer _castleViewer;

    /// <summary>
    /// コントローラ
    /// </summary>
    private C_Host _myController;

    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _myController = GetComponent<C_Host>();

    }

    // Update is called once per frame
    void Update()
    {

    }
}
