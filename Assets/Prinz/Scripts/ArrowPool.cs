using System.Collections.Generic;
using UnityEngine;

public class ArrowPool : MonoBehaviour
{
    [SerializeField] GameObject arrowPrefab;

    public static ArrowPool Instance { get; private set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {        
        //////
        /// to do : maybe make an empty object to hold the pool so that units can use it. Also make it so
        /// this can hold multiple pools (fireballs, arrows, etc... -> projectiles pool)
        //////
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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

    List<Arrow> pool;

    public void CreatePool(int maxCount)
    {
        pool = new List<Arrow>();

        for (int i = 0; i < maxCount; i++)
        {
            GameObject gameObject = Instantiate(arrowPrefab);
            Arrow obj = gameObject.GetComponent<Arrow>();
            obj.gameObject.SetActive(false);
            pool.Add(obj);
        }
    }

    //使う時に場所を指定して表示する
    public Arrow GetObj(Vector3 position, Vector3 enemyPos) 
    {
        //使ってないものを探す
        for (int i = 0; i < pool.Count; i++)
        {
            if (pool[i].gameObject.activeSelf == false)
            {
                Arrow arrow = pool[i];
                arrow.Initialize(position, enemyPos);
                arrow.gameObject.SetActive(true);

                return arrow;
            }
        }

        //全部使っていたら
        GameObject newObj = Instantiate(arrowPrefab, position, Quaternion.identity);
        if (newObj)
        {
            Arrow newArrow = newObj.GetComponent<Arrow>();
            newArrow.Initialize(position, enemyPos);
            newArrow.gameObject.SetActive(true);
            pool.Add(newArrow);

            return newArrow;
        }

        return null;
    }

    public void Release(Arrow arrow)
    {
        arrow.gameObject.SetActive (false);
    }
}
