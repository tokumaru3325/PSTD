using UnityEngine;

public class M_GlobalVariable
{
    public MultiRoleType MultiRole { get; private set; }

    /// <summary>
    /// プレイヤーの名前
    /// </summary>
    public string MyName { get; private set; }

    /// <summary>
    /// マルチの役割を設定
    /// </summary>
    /// <param name="role">モード</param>
    public void SetRole(MultiRoleType role)
    {
        MultiRole = role;
    }

    /// <summary>
    /// プレイヤーの名前を設定
    /// </summary>
    /// <param name="name">名前</param>
    public void SetMyName(string name)
    {
        MyName = name;
    }
}
