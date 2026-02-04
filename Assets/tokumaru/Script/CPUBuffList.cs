using System.Collections.Generic;
using UnityEngine;

public class CPUBuffList : MonoBehaviour
{
    /// <summary>
    /// 対象のタグ
    /// </summary>
    [SerializeField]
    private string _targetTag;

    /// <summary>
    /// バフマネージャー
    /// </summary>
    private BuffManager manager;



    /// <summary>
    /// バフリスト
    /// </summary>
    private List<C_Buff> _myChilds;

    void Awake()
    {
        manager = FindFirstObjectByType<BuffManager>();
        _myChilds = new List<C_Buff>();
        if (!manager)
            Debug.LogError("buff Manager didnot find");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (manager)
        {
            manager.OnAddBuff += HandleBuffAdded;
            manager.OnRemoveBuff += HandleBuffRemoved;
        }

    }

    void OnDestroy()
    {
        if (manager)
        {
            manager.OnAddBuff -= HandleBuffAdded;
            manager.OnRemoveBuff -= HandleBuffRemoved;
        }
    }

    /// <summary>
    /// バフを付与された処理
    /// </summary>
    /// <param name="buff">バフ</param>
    private void HandleBuffAdded(C_Buff buff)
    {
        if (buff.TargetTag.Equals(_targetTag))
        {
            _myChilds.Add(buff);
        }
    }

    /// <summary>
    /// バフを外された処理
    /// </summary>
    /// <param name="buff">バフ</param>
    private void HandleBuffRemoved(C_Buff buff)
    {
        if (buff.TargetTag.Equals(_targetTag))
        {
            if (_myChilds.Contains(buff))
            {
                _myChilds.Remove(buff);
            }
        }
    }

    //得丸陽生　20260202 start
    public int GetListLength()
    {
        return _myChilds.Count;
    }
    //得丸陽生　20260202 end
}
