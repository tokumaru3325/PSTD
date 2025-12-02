using UnityEngine;

public class M_GlobalVariable
{
    public MultiRoleType MultiRole { get; private set; }

    /// <summary>
    /// マルチの役割を設定
    /// </summary>
    /// <param name="role">モード</param>
    public void SetRole(MultiRoleType role)
    {
        MultiRole = role;
    }
}
