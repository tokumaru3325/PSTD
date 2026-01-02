using System.Text;
using Unity.Netcode;
using UnityEngine;

public class C_Host : MonoBehaviour
{
    /// <summary>
    /// 対象のマルチタイプ
    /// </summary>
    [SerializeField]
    private MultiRoleType _target;

    /// <summary>
    /// コントローラーですか
    /// </summary>
    private bool _isController = false;

    /// <summary>
    /// 共通変数
    /// </summary>
    private C_GlobalVariable _globalVariable;

    private void Awake()
    {
        _globalVariable = FindFirstObjectByType<C_GlobalVariable>();
        _isController = _globalVariable.GetRoomRole() == _target;
    }

    void Start()
    {

    }


}
