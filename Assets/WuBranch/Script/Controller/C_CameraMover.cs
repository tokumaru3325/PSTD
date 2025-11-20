using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public struct CameraSide
{
    [SerializeField]
    public float Left;
    [SerializeField]
    public float Right;
    [SerializeField]
    public float Up;
    [SerializeField]
    public float Down;
}

/// <summary>
/// カメラの移動
/// </summary>
public class C_CameraMover : MonoBehaviour
{
    /// <summary>
    /// ターゲット
    /// </summary>
    private Vector3 _target;

    [SerializeField]
    [Tooltip("カメラの滑らかさ（値が小さいほど速く追従）")]
    private float _smoothTime = 0.3f;

    [SerializeField]
    private float _moveSpeed = 10.0f;

    /// <summary>
    /// カメラの移動範囲
    /// </summary>
    [SerializeField]
    [Tooltip("カメラの移動範囲")]
    CameraSide _side;

    /// <summary>
    /// SmoothDampが内部で使う速度変数（変更不要）
    /// </summary>
    private Vector3 velocity = Vector3.zero;

    /// <summary>
    /// カメラのz軸のデフォルト値
    /// </summary>
    private float _myZOffset;

    /// <summary>
    /// 移動動作
    /// </summary>
    private InputAction _moveAction;

    /// <summary>
    /// サイドにたどり着いたの通知
    /// </summary>
    public event Action<byte> OnArriveSide;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        _myZOffset = transform.localPosition.z;
        _target = Vector3.zero + new Vector3(0, 0, _myZOffset);
    }

    void Start()
    {
        _moveAction = InputSystem.actions.FindAction("Move");
    }

    private void Update()
    {
        Vector2 direction = _moveAction.ReadValue<Vector2>();
        MoveTarget(direction);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector2 me = (Vector2)transform.position;
        Vector2 target = (Vector2)_target;
        transform.position = Vector3.SmoothDamp(transform.position, _target, ref velocity, _smoothTime);
        // ターゲットに近づいた
        if (Vector2.Distance(me, target) >= 0.001f)
            HandleArriveSide();
    }

    /// <summary>
    /// ターゲットの位置を設定
    /// </summary>
    /// <param name="pos">位置</param>
    public void SetTargetPos(Vector3 pos)
    {
        _target = pos;
        _target.z = _myZOffset;
    }

    /// <summary>
    /// ターゲットを移動させる
    /// </summary>
    /// <param name="Direction">移動方向</param>
    public void MoveTarget(Vector3 Direction)
    {
        // z軸方向の移動はさせない
        Direction.z = 0f;
        // ターゲットを移動させ
        _target += (_moveSpeed * Time.fixedDeltaTime * Direction);

        AdjustInMovementRange();
    }

    /// <summary>
    /// 移動範囲内に修正
    /// </summary>
    private void AdjustInMovementRange()
    {
        _target.x = Mathf.Clamp(_target.x , _side.Left, _side.Right);
        _target.y = Mathf.Clamp(_target.y, _side.Down, _side.Up);
    }

    /// <summary>
    /// 4方向のサイドそれぞれにたどり着いたか
    /// </summary>
    private void HandleArriveSide()
    {
        // 4方向のサイドそれぞれにたどり着いたか
        // 左：8、上：4、右：2、下：1
        byte dir = 0x0;
        if (Mathf.Abs(transform.position.x - _side.Left) <= 0.01f)
            dir |= 0x08;
        if (Mathf.Abs(transform.position.x - _side.Right) <= 0.01f)
            dir |= 0x02;
        if (Mathf.Abs(transform.position.y - _side.Up) <= 0.01f)
            dir |= 0x04;
        if (Mathf.Abs(transform.position.y - _side.Down) <= 0.01f)
            dir |= 0x01;

        Notify(dir);
    }

    /// <summary>
    /// 通知
    /// </summary>
    /// <param name="result">4方向の結果</param>
    private void Notify(byte result)
    {
        OnArriveSide?.Invoke(result);
    }
}
