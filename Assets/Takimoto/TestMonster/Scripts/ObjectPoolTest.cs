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
            //[2025/11/18]　プリンス　Start

            obj.Initialize();
            //[2025/11/18]　プリンス　End
            obj.gameObject.SetActive(false);
            pool.Add(obj);
        }
    }

    //使う時に場所を指定して表示する
    public UnitPresenter GetObj(Vector3 position)
    {
        //使ってないものを探す
        for(int i = 0; i < pool.Count; i++)
        {
            if (pool[i].gameObject.activeSelf == false)
            {
                UnitPresenter Unit = pool[i];
                Unit.gameObject.transform.position = position;
                Unit.Initialize();
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
            newUnit.Initialize();
            //[2025/11/18]　プリンス　End
            newUnit.gameObject.SetActive(true);
         //   newMonster.ElapsedTime = 0;
            pool.Add(newUnit);
            
            return newUnit;
        }

        return null;
    }


}
