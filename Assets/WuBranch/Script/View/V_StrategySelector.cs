using System;
using UnityEngine;
using UnityEngine.UI;

public class V_StrategySelector : MonoBehaviour
{
    /// <summary>
    /// 今の戦術
    /// </summary>
    public PathStrategy CurrentStrategy { get; private set; }

    [SerializeField]
    private Toggle[] _toggles;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CurrentStrategy = PathStrategy.Shortest;
        for (int index = 0; index < _toggles.Length; index++)
        {
            _toggles[index].onValueChanged.AddListener(ChangeStrategy);
        }
    }

    /// <summary>
    /// 戦術を変更
    /// </summary>
    /// <param name="strategy">新しい戦術</param>
    public void ChangeStrategy(bool isOn)
    {
        if (!isOn)
            return;

        for (int index = 0; index < _toggles.Length; index++)
        {
            if (_toggles[index].isOn)
            {
                CurrentStrategy = (PathStrategy)index;
            }
        }
    }
}
