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
    /// 部屋をコントロール物
    /// </summary>
    private C_RoomFront _myController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _background = GetComponent<Image>();
        _myController = GetComponent<C_RoomFront>();
        _myController.OnInitedData += SetScreen;
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
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _background.color = _hoverColor;
    }

    private void SetScreen()
    {
        M_RoomFrontData data = _myController.GetData();
        _roomName.text = data.Name;
        _leader.text = data.LeaderName;
        _lock.SetActive(data.HavePwd);
        _member.text = data.MemberNums + "/" + data.MaxMembers;
    }
}
