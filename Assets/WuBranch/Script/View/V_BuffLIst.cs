using System.Collections.Generic;
using UnityEngine;

public class V_BuffList : MonoBehaviour
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
    /// バフのプレハブ
    /// </summary>
    [SerializeField]
    private GameObject _buffPrefab;

    /// <summary>
    /// 表示する場所
    /// </summary>
    [SerializeField]
    private GameObject _container;

    /// <summary>
    /// バフリスト
    /// </summary>
    private Dictionary<C_Buff, V_Buff> _myChilds;

    void Awake()
    {
        manager = FindFirstObjectByType<BuffManager>();
        _myChilds = new Dictionary<C_Buff, V_Buff>();
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
        if (!_buffPrefab)
            Debug.LogError("didnot attach buff prefab");
    }

    void OnDestroy()
    {
        if (manager)
        {
            Debug.Log("Release buff function");
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
            V_Buff buffV = CreateBuff();
            buff.BindTimeUpdate(buffV.UpdateTime);
            buffV.SetIcon(buff.GetIcon());
            _myChilds.Add(buff, buffV);
            buffV.SlideIn();
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
            if (_myChilds.ContainsKey(buff))
            {
                if (_myChilds.TryGetValue(buff, out V_Buff buffV))
                {
                    buffV.SlideOut();
                    _myChilds.Remove(buff);
                }
            }
        }
    }

    private V_Buff CreateBuff()
    {
        if (!_buffPrefab)
            return null;

        if (!_container)
        {
            Debug.LogError($"container is null!");
            return null;
        }

        GameObject obj = Instantiate(_buffPrefab, _container.transform);
        //obj.transform.localScale = Vector3.one;
        V_Buff buff = obj.GetComponent<V_Buff>();
        //obj.transform.SetParent(_container.transform);
        return buff;
    }

    //得丸陽生　20260202 start
    public int GetListLength()
    {
        return _myChilds.Count;
    }

    //得丸陽生　20260202 end
}
