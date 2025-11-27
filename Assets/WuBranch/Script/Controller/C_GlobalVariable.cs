using UnityEngine;

/// <summary>
/// シーンをまたいでの共通変数
/// </summary>
public class C_GlobalVariable : MonoBehaviour
{
    private M_GlobalVariable _datas;

    void Awake()
    {
        // 唯一にする
        C_GlobalVariable[] list = FindObjectsByType<C_GlobalVariable>(FindObjectsSortMode.None);
        if (list.Length >= 2)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        _datas = new M_GlobalVariable();
    }

    /// <summary>
    /// マルチの役割を設定
    /// </summary>
    /// <param name="role">役割</param>
    public void SetMultiRole(MultiRoleType role)
    {
        _datas.SetRole(role);
    }

    /// <summary>
    /// ゲームモードをゲット
    /// </summary>
    /// <returns></returns>
    public MultiRoleType GetMultiRole()
    {
        return _datas.MultiRole;
    }
}
