using Cysharp.Threading.Tasks.Triggers;
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[Serializable]
enum MoveDir
{
    Left,
    Right,
    Up,
    Down
}

public class V_CameraMover : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IPointerExitHandler
{
    /// <summary>
    /// 方向
    /// </summary>
    [SerializeField]
    private MoveDir _direction;

    /// <summary>
    /// カメラを移動させるもの
    /// </summary>
    [SerializeField]
    private C_CameraMover _cameraMover;

    private Button _button;

    /// <summary>
    /// 押したか
    /// </summary>
    private bool _isPressed;


    void Start()
    {
        _isPressed = false;
        _button = GetComponent<Button>();
        _cameraMover.OnArriveSide += OnArriveSide;
    }

    void Update()
    {
        if (_isPressed)
        {
            HandleMoveDirection();
        }
    }

    /// <summary>
    /// 押したの処理
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerDown(PointerEventData eventData)
    {
        _isPressed = true;
    }

    /// <summary>
    /// 離したの処理
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerUp(PointerEventData eventData)
    {
        _isPressed = false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {

    }

    /// <summary>
    /// 移動方向の処理
    /// </summary>
    private void HandleMoveDirection()
    {
        switch (_direction)
        {
            case MoveDir.Left:
                _cameraMover.MoveTarget(Vector3.left);
                break;
            case MoveDir.Right:
                _cameraMover.MoveTarget(Vector3.right);
                break;
            case MoveDir.Up:
                _cameraMover.MoveTarget(Vector3.up);
                break;
            case MoveDir.Down:
                _cameraMover.MoveTarget(Vector3.down);
                break;
        }
    }

    private void OnArriveSide(byte side)
    {
        //Debug.Log($"Get result {side}");
        // 左：8、上：4、右：2、下：1
        byte result = 0x00;
        switch (_direction)
        {
            case MoveDir.Left:
                result = (byte)(side & 0x08);
                break;
            case MoveDir.Right:
                result = (byte)(side & 0x02);
                break;
            case MoveDir.Up:
                result = (byte)(side & 0x04);
                break;
            case MoveDir.Down:
                result = (byte)(side & 0x01);
                break;
        }

        _button.interactable = (result == 0);
    }
}
