using Unity.Netcode;
using UnityEditor.SceneManagement;
using UnityEngine;

public class C_Room : MonoBehaviour
{

    /// <summary>
    /// 共通変数
    /// </summary>
    private C_GlobalVariable _globalVariable;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _globalVariable = FindFirstObjectByType<C_GlobalVariable>();
        if (_globalVariable.GetRoomRole() == MultiRoleType.Client)
        {
            gameObject.AddComponent<C_Client>();
        }
        else if (_globalVariable.GetRoomRole() == MultiRoleType.Host)
        {
            gameObject.AddComponent<C_Host>();
        }
    }
}
