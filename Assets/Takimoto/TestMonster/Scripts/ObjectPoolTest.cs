using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class ObjectPoolTest : MonoBehaviour
{
    [SerializeField] GameObject prefabObj;
    List<TestMonsterParent> pool;

    //最初にいくつプールに貯めておくか
    public void CreatePool(int maxCount)
    {
        pool = new List<TestMonsterParent>();

        for (int i = 0; i < maxCount; i++)
        {
            GameObject gameObject = Instantiate(prefabObj);
            TestMonsterParent obj = gameObject.GetComponent<TestMonsterParent>();
            obj.gameObject.SetActive(false);
            pool.Add(obj);
        }
    }

    //使う時に場所を指定して表示する
    public TestMonsterParent GetObj(Vector3 position)
    {
        //使ってないものを探す
        for(int i = 0; i < pool.Count; i++)
        {
            if (pool[i].gameObject.activeSelf == false)
            {
                TestMonsterParent Monster = pool[i];
                Monster.gameObject.transform.position = position;
                Monster.gameObject.SetActive(true);
                Monster.ElapsedTime = 0;

                Debug.Log("Visibility");
                return Monster;                
            }
        }

        //全部使っていたら
        GameObject newObj = Instantiate(prefabObj, position, Quaternion.identity);
        if (newObj)
        {
            TestMonsterParent newMonster = newObj.GetComponent<TestMonsterParent>();
            newMonster.gameObject.SetActive(true);
            newMonster.ElapsedTime = 0;
            pool.Add(newMonster);
            Debug.Log("Create");
            return newMonster;
        }

        return null;
    }
}
