using System;
using UnityEngine;

public class M_ObstacleStone
{

    /// <summary>
    /// 今の体力
    /// </summary>
    /// <value></value>
    public float HP { get; private set; }

    /// <summary>
    /// 最大体力
    /// </summary>
    private float _maxHP;

    /// <summary>
    /// 表示順番
    /// </summary>
    /// <value></value>
    public int ZOrder { get; private set; }

    /// <summary>
    /// 体力が変化したときに発火するイベント
    /// </summary>
    public Action<float, float> OnHPChanged;

    public M_ObstacleStone()
    {
        _maxHP = 100f;
        ZOrder = 30;
        Initialize();
    }

    public M_ObstacleStone(float MaxHP, int order)
    {
        _maxHP = MaxHP;
        ZOrder = order;
        Initialize();
    }

    public M_ObstacleStone(M_Obstacle data)
    {
        _maxHP = data.MaxHealth;
        ZOrder = data.ZOrder;
        Initialize();
    }

    /// <summary>
    /// 最大体力を設定
    /// </summary>
    /// <param name="max">最大体力</param>
    public void SetMaxHealth(float max)
    {
        _maxHP = max;
    }

    /// <summary>
    /// 表示順番を設定
    /// </summary>
    /// <param name="order">表示順番</param>
    public void SetOrder(int order)
    {
        ZOrder = order;
    }

    /// <summary>
    /// 初期化
    /// </summary>
    public void Initialize()
    {
        HP = _maxHP;
        OnHPChanged?.Invoke(HP, _maxHP);
    }

    /// <summary>
    /// 今の体力を設定
    /// </summary>
    /// <param name="hp">新しい体力</param>
    public void SetHP(float hp)
    {
        HP = Mathf.Clamp(hp, 0, _maxHP);
        OnHPChanged?.Invoke(HP, _maxHP);
    }
}
