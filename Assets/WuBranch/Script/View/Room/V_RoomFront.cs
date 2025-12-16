using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

[RequireComponent(typeof(Image)), RequireComponent(typeof(C_RoomFront))]
public class V_RoomFront : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    /// <summary>
    /// 背景
    /// </summary>
    private Image _background;

    /// <summary>
    /// 一般の色
    /// </summary>
    [SerializeField]
    private Color _normalColor;

    /// <summary>
    /// マウスが乗っているの色
    /// </summary>
    [SerializeField]
    private Color _hoverColor;

    /// <summary>
    /// 一般の色
    /// </summary>
    [SerializeField]
    private Color _pressedColor;

    /// <summary>
    /// 部屋名
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI _roomName;

    /// <summary>
    /// 部屋主
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI _leader;

    /// <summary>
    /// 人数
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI _member;

    /// <summary>
    /// ロック
    /// </summary>
    [SerializeField]
    private GameObject _lock;

    /// <summary>
    /// パスワード入力パネル
    /// </summary>
    [SerializeField]
    private V_PwdDialogue _pwdInputPanel;

    /// <summary>
    /// 部屋をコントロール物
    /// </summary>
    private C_RoomFront _myController;

    /// <summary>
    /// マウスが押した座標
    /// </summary>
    private Vector2 MouseDownPos;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _background = GetComponent<Image>();
        _myController = GetComponent<C_RoomFront>();
        _myController.OnInitedData += SetScreen;
        _pwdInputPanel = FindFirstObjectByType<V_PwdDialogue>(FindObjectsInactive.Include);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _background.color = _hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _background.color = _normalColor;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _background.color = _pressedColor;
        MouseDownPos = eventData.position;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _background.color = _hoverColor;
        if (MouseDownPos.x == eventData.position.x && MouseDownPos.y == eventData.position.y)
        {
            if (_myController.GetData().HavePwd)
                OpenPwdInputPanel();
            else
                _myController.JoinLobby();
        }
    }

    /// <summary>
    /// 画面の初期化
    /// </summary>
    private void SetScreen()
    {
        M_RoomFrontData data = _myController.GetData();
        _roomName.text = data.Name;
        _leader.text = data.LeaderName;
        _lock.SetActive(data.HavePwd);
        _member.text = data.MemberNums + "/" + data.MaxMembers;
    }

    /// <summary>
    /// パスワード入力パネルを開く
    /// </summary>
    private void OpenPwdInputPanel()
    {
        if (_pwdInputPanel)
        {
            _pwdInputPanel.Open(_myController);
        }
    }
}
