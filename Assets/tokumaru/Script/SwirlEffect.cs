using System.Collections.Generic;
using UnityEngine;

public class SwirlEffect : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    float currentY = 0.0f;
    float currentX = 0.0f;

    List<Swirl> swirlPool;
    public GameObject obj;

    int poolMax = 30;

    [SerializeField] Transform canvas;

    //[SerializeField] public static GameObject coinPrefab;


    void Start()
    {
    }

    public void CreatePool()
    {
        swirlPool = new List<Swirl>();
        for (int i = 0; i < poolMax; i++)
        {
            GameObject coin = Instantiate(obj, canvas);
            Swirl sc = coin.GetComponent<Swirl>();
            if (sc)
            {
                sc.gameObject.SetActive(false);
                swirlPool.Add(sc);
            }
        }
    }

    Swirl GetObject()
    {
        for (int i = 0; i < swirlPool.Count; i++)
        {
            if (!swirlPool[i].gameObject.activeSelf)
            {
                Swirl swirl = swirlPool[i];
                swirl.Initialize(canvas.position);
                swirl.gameObject.SetActive(true);
                return swirl;
            }
        }

        GameObject newSwirl = Instantiate(obj);
        if (newSwirl)
        {
            Swirl swirl = newSwirl.GetComponent<Swirl>();
            swirl.Initialize(canvas.position);
            swirl.gameObject.SetActive(true);
            swirlPool.Add(swirl);
            return swirl;
        }

        return null;
    }
    void Release(Swirl obj)
    {

        for (int i = 0; i < swirlPool.Count; i++)
        {
            if (swirlPool[i] == obj)
            {
                swirlPool[i].gameObject.SetActive(false);
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
        for (int i = 0; i < effectCount; i++)
        {
            //GameObject coin = Instantiate(coinPrefab, canvas);
            //Debug.Log(coin.GetComponent<Coin>());
            Swirl coin = GetObject();
            Swirl sc = coin.GetComponent<Swirl>();
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
