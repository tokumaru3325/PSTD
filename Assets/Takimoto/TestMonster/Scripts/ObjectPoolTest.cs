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
            obj.model.MoveDirection = Vector3.right;
            obj.model.MoveSpeed = 3.0f;
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
                UnitPresenter Monster = pool[i];
                Monster.gameObject.transform.position = position;
                Monster.gameObject.SetActive(true);
             //   Monster.ElapsedTime = 0;

                Debug.Log("Visibility");
                return Monster;                
            }
        }

        //全部使っていたら
        GameObject newObj = Instantiate(prefabObj, position, Quaternion.identity);
        if (newObj)
        {
            UnitPresenter newMonster = newObj.GetComponent<UnitPresenter>();
            //[2025/11/18]　プリンス　Start
            newMonster.Initialize();
            newMonster.model.MoveDirection = Vector3.right;
            newMonster.model.MoveSpeed = 3.0f;
            //[2025/11/18]　プリンス　End
            newMonster.gameObject.SetActive(true);
         //   newMonster.ElapsedTime = 0;
            pool.Add(newMonster);
            Debug.Log("Create");
            return newMonster;
        }

        return null;
    }


}
