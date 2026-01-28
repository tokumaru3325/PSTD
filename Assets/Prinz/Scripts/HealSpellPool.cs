using System.Collections.Generic;
using UnityEngine;

public class HealSpellPool : MonoBehaviour
{
    [SerializeField] GameObject lightningSpellPrefab;

    public static HealSpellPool Instance { get; private set; }

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

    List<HealSpell> pool;

    public void CreatePool(int maxCount)
    {
        pool = new List<HealSpell>();

        for (int i = 0; i < maxCount; i++)
        {
            GameObject gameObject = Instantiate(lightningSpellPrefab);
            HealSpell obj = gameObject.GetComponent<HealSpell>();
            obj.gameObject.SetActive(false);
            pool.Add(obj);
        }
    }

    //使う時に場所を指定して表示する
    public HealSpell GetObj(Vector3 position, Vector3 enemyPos)
    {
        //使ってないものを探す
        for (int i = 0; i < pool.Count; i++)
        {
            if (pool[i].gameObject.activeSelf == false)
            {
                HealSpell arrow = pool[i];
                arrow.Initialize(position, enemyPos);
                arrow.gameObject.SetActive(true);

                return arrow;
            }
        }

        //全部使っていたら
        GameObject newObj = Instantiate(lightningSpellPrefab, position, Quaternion.identity);
        if (newObj)
        {
            HealSpell newArrow = newObj.GetComponent<HealSpell>();
            newArrow.Initialize(position, enemyPos);
            newArrow.gameObject.SetActive(true);
            pool.Add(newArrow);

            return newArrow;
        }

        return null;
    }

    public void Release(HealSpell spell)
    {
        spell.gameObject.SetActive(false);
    }
}
