using System.Collections.Generic;
using UnityEngine;

public class LightningSpellPool : MonoBehaviour
{
    [SerializeField] GameObject lightningSpellPrefab;

    public static LightningSpellPool Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    List<LightningSpell> pool;

    public void CreatePool(int maxCount)
    {
        pool = new List<LightningSpell>();

        for (int i = 0; i < maxCount; i++)
        {
            GameObject gameObject = Instantiate(lightningSpellPrefab);
            LightningSpell obj = gameObject.GetComponent<LightningSpell>();
            obj.gameObject.SetActive(false);
            pool.Add(obj);
        }
    }

    //使う時に場所を指定して表示する
    public LightningSpell GetObj(Vector3 position, Vector3 enemyPos)
    {
        //使ってないものを探す
        for (int i = 0; i < pool.Count; i++)
        {
            if (pool[i].gameObject.activeSelf == false)
            {
                LightningSpell arrow = pool[i];
                arrow.Initialize(position, enemyPos);
                arrow.gameObject.SetActive(true);

                return arrow;
            }
        }

        //全部使っていたら
        GameObject newObj = Instantiate(lightningSpellPrefab, position, Quaternion.identity);
        if (newObj)
        {
            LightningSpell newArrow = newObj.GetComponent<LightningSpell>();
            newArrow.Initialize(position, enemyPos);
            newArrow.gameObject.SetActive(true);
            pool.Add(newArrow);

            return newArrow;
        }

        return null;
    }

    public void Release(LightningSpell arrow)
    {
        for (int i = 0; i < pool.Count; i++)
        {
            if (pool[i] == arrow)
            {
                pool[i].gameObject.SetActive(false);
                return;
            }
        }
    }
}
