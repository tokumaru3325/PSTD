using System;
using UnityEngine;

public class M_Tower
{
    /// <summary>
    /// タワーの最大体力
    /// </summary>
    public float MAX_HP { get; private set; }

    /// <summary>
    /// タワーの現在体力
    /// </summary>
    public float HP { get; private set; }

    /// <summary>
    /// 体力が変化したときに発火するイベント
    /// </summary>
    public event Action<float> OnHPChanged;

    public M_Tower(float maxHP)
    {
        MAX_HP = maxHP;
        HP = maxHP;
    }

    /// <summary>
    /// タワーの体力を最大にリセットする
    /// </summary> 
    public void ResetHP()
    {
        HP = MAX_HP;
        OnHPChanged?.Invoke(HP);
    }

    /// <summary>
    /// タワーの体力を設定する
    /// </summary>
    /// <param name="hp">新しい体力</param>
    public void SetHP(float hp)
    {
        HP = Mathf.Clamp(hp, 0, MAX_HP);
        OnHPChanged?.Invoke(HP);
    }
}
