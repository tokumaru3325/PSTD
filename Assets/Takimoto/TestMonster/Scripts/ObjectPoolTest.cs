using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using Unity.VisualScripting;
//using static UnityEditor.Progress;

public class ObjectPoolTest : MonoBehaviour
{
    [SerializeField] GameObject prefabObj;

    [SerializeField]
    public UnitData UnitData; //ScriptableObjectをインスペクターに設定する //[2025/12/21] プリンス：SpawnButtonから移した
    [SerializeField]
    public BuffDataBase BuffData;

    List<UnitPresenter> pool;

    //最初にいくつプールに貯めておくか
    public void CreatePool(int maxCount)
    {
        pool = new List<UnitPresenter>();

        for (int i = 0; i < maxCount; i++)
        {
            GameObject gameObject = Instantiate(prefabObj);
            UnitPresenter obj = gameObject.GetComponent<UnitPresenter>();
            obj.SetPool(this);
            obj.gameObject.SetActive(false);
            pool.Add(obj);
        }
    }

    //使う時に場所を指定して表示する
    public UnitPresenter GetObj(Vector3 position, UnitData data, Vector3 enemyPos, string playerTag) //[2025/12/02] プリンス : "playerTag"追加
    {
        //使ってないものを探す
        for (int i = 0; i < pool.Count; i++)
        {
            if (pool[i].gameObject.activeSelf == false)
            {
                UnitPresenter Unit = pool[i];
            //    if(playerTag == "Player1")
                Unit.Initialize(data, BuffData, position, enemyPos, playerTag); //[2025/12/02] プリンス : "playerTag"追加
                Unit.gameObject.SetActive(true);

                return Unit;                
            }
        }

        //全部使っていたら
        GameObject newObj = Instantiate(prefabObj, position, Quaternion.identity);
        if (newObj)
        {
            UnitPresenter newUnit = newObj.GetComponent<UnitPresenter>();
            //[2025/11/18]　プリンス　Start
            newUnit.Initialize(data, BuffData, position, enemyPos, playerTag); //[2025/12/02] プリンス : "playerTag"追加
            newUnit.SetPool(this);
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
        for (int i = 0; i < pool.Count; i++)
        {
            if (pool[i] == unit)
            {
                pool[i].gameObject.SetActive(false);
                //reset animator, vfx, etc.
                return;
            }
        }
    }
    //[2025/11/20]　プリンス　End
}
