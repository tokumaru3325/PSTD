using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;
using System;

public class CoinEffect : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    float currentY = 0.0f;
    float currentX = 0.0f;

    List<Coin> coinPool;
    public GameObject obj;

    int poolMax = 30;

    [SerializeField] Transform canvas;

    //[SerializeField] public static GameObject coinPrefab;


    void Start()
    {
    }

    public void CreatePool()
    {
        coinPool = new List<Coin>();
        for (int i = 0; i < poolMax; i++)
        {
            GameObject coin = Instantiate(obj, canvas);
            Coin sc = coin.GetComponent<Coin>();
            if (sc)
            {
                sc.gameObject.SetActive(false);
                coinPool.Add(sc);
            }
        }
    }

    Coin GetObject()
    {
        for(int i = 0; i < coinPool.Count; i++)
        {
            if (!coinPool[i].gameObject.activeSelf)
            {
                Coin coin = coinPool[i];
                coin.Initialize(canvas.position);
                coin.gameObject.SetActive(true);
                return coin;
            }
        }

        GameObject newCoin = Instantiate(obj);
        if (newCoin)
        {
            Coin coin = newCoin.GetComponent<Coin>();
            coin.Initialize(canvas.position);
            coin.gameObject.SetActive(true);    
            coinPool.Add(coin);
            return coin;
        }

        return null;
    }
    void Release(Coin obj)
    {

        for (int i = 0; i < coinPool.Count; i++)
        {
            if (coinPool[i] == obj)
            {
                coinPool[i].gameObject.SetActive(false);
                return;
            }
        }

    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateEffect(int effectCount)
    {
        for(int i = 0; i < effectCount; i++)
        {
            //GameObject coin = Instantiate(coinPrefab, canvas);
            //Debug.Log(coin.GetComponent<Coin>());
            Coin coin = GetObject();
            Coin sc = coin.GetComponent<Coin>();
            if (sc)
            {
                sc.release += Release;
                float powerX = UnityEngine.Random.Range(-100.0f, 100.0f);
                float powerY = UnityEngine.Random.Range(700.0f, 1001.0f);
                sc.Create(powerX, powerY, 3.0f);
                Debug.Log("とりあえずここまで来ましたけど");
            }
        }
    }
    
}
