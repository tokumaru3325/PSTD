using System.Collections.Generic;
using UnityEngine;

public class FireEffect : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    float currentY = 0.0f;
    float currentX = 0.0f;

    List<Fire> firePool;
    public GameObject obj;

    int poolMax = 30;

    [SerializeField] Transform canvas;

    //[SerializeField] public static GameObject coinPrefab;


    void Start()
    {
    }

    public void CreatePool()
    {
        firePool = new List<Fire>();
        for (int i = 0; i < poolMax; i++)
        {
            GameObject coin = Instantiate(obj, canvas);
            Fire sc = coin.GetComponent<Fire>();
            if (sc)
            {
                sc.gameObject.SetActive(false);
                firePool.Add(sc);
            }
        }
    }

    Fire GetObject()
    {
        for (int i = 0; i < firePool.Count; i++)
        {
            if (!firePool[i].gameObject.activeSelf)
            {
                Fire fire = firePool[i];
                fire.Initialize(canvas.position);
                fire.gameObject.SetActive(true);
                return fire;
            }
        }

        GameObject newFire = Instantiate(obj);
        if (newFire)
        {
            Fire fire = newFire.GetComponent<Fire>();
            fire.Initialize(canvas.position);
            fire.gameObject.SetActive(true);
            firePool.Add(fire);
            return fire;
        }

        return null;
    }
    void Release(Fire obj)
    {

        for (int i = 0; i < firePool.Count; i++)
        {
            if (firePool[i] == obj)
            {
                firePool[i].gameObject.SetActive(false);
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
            Fire coin = GetObject();
            Fire sc = coin.GetComponent<Fire>();
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
