using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;

public class ObjectPoolTest : MonoBehaviour
{
    [SerializeField] GameObject prefabObj;

    List<UnitPresenter> pool;

    //最初にいくつプールに貯めておくか
    public void CreatePool(int maxCount)
    {
        pool = new List<UnitPresenter>();

        for (int i = 0; i < maxCount; i++)
        {
            GameObject gameObject = Instantiate(prefabObj);
            UnitPresenter obj = gameObject.GetComponent<UnitPresenter>();
            obj.gameObject.SetActive(false);
            pool.Add(obj);
        }
    }

    //使う時に場所を指定して表示する
    public UnitPresenter GetObj(Vector3 position, UnitData data)
    {
        //使ってないものを探す
        for(int i = 0; i < pool.Count; i++)
        {
            if (pool[i].gameObject.activeSelf == false)
            {
                UnitPresenter Unit = pool[i];
                Unit.gameObject.transform.position = position;
                Unit.Initialize(data);
                Unit.gameObject.SetActive(true);
             //   Monster.ElapsedTime = 0;

                return Unit;                
            }
        }

        //全部使っていたら
        GameObject newObj = Instantiate(prefabObj, position, Quaternion.identity);
        if (newObj)
        {
            UnitPresenter newUnit = newObj.GetComponent<UnitPresenter>();
            //[2025/11/18]　プリンス　Start
            newUnit.Initialize(data);
            //[2025/11/18]　プリンス　End
            newUnit.gameObject.SetActive(true);
         //   newMonster.ElapsedTime = 0;
            pool.Add(newUnit);
            
            return newUnit;
        }

        return null;
    }

    //[2025/11/20]　プリンス　Start
    public void Release(UnitPresenter unit)
    {
        unit.gameObject.SetActive(false);
        //reset animator, vfx, etc.
    }
    //[2025/11/20]　プリンス　End
}
