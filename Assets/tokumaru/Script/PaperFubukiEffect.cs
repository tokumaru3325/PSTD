using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PaperFubukiEffect : MonoBehaviour
{
    float currentY = 0.0f;
    float currentX = 0.0f;

    List<PaperFubuki> paperFubukiPool;
    public GameObject obj;

    int poolMax = 100;

    [SerializeField] Transform canvas;

    [SerializeField] Vector3 LeftPosition;
    [SerializeField] Vector3 RightPosition;

    [SerializeField] Sprite[] images;
    //[SerializeField] Image image;

    //[SerializeField] public static GameObject coinPrefab;


    void Start()
    {
    }

    public void CreatePool()
    {
        paperFubukiPool = new List<PaperFubuki>();
        for (int i = 0; i < poolMax; i++)
        {
            GameObject coin = Instantiate(obj, canvas);
            PaperFubuki sc = coin.GetComponent<PaperFubuki>();
            if (sc)
            {
                sc.gameObject.SetActive(false);
                paperFubukiPool.Add(sc);
            }
        }
    }

    PaperFubuki GetObject()
    {
        for (int i = 0; i < paperFubukiPool.Count; i++)
        {
            if (!paperFubukiPool[i].gameObject.activeSelf)
            {
                PaperFubuki pf = paperFubukiPool[i];
                pf.Initialize(canvas.position);
                pf.gameObject.SetActive(true);
                return pf;
            }
        }

        GameObject newPF = Instantiate(obj,canvas);
        if (newPF)
        {
            PaperFubuki pf = newPF.GetComponent<PaperFubuki>();
            pf.Initialize(canvas.position);
            pf.gameObject.SetActive(true);
            paperFubukiPool.Add(pf);
            return pf;
        }

        return null;
    }
    void Release(PaperFubuki obj)
    {

        for (int i = 0; i < paperFubukiPool.Count; i++)
        {
            if (paperFubukiPool[i] == obj)
            {
                paperFubukiPool[i].gameObject.SetActive(false);
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
            PaperFubuki coin = GetObject();
            //PaperFubuki sc = coin.GetComponent<PaperFubuki>();
            if (coin)
            {
                coin.release += Release;

                coin.rotateZ = UnityEngine.Random.Range(0.5f, 6.0f);

                Image image = coin.gameObject.GetComponent<Image>();

                float scale = UnityEngine.Random.Range(0.3f, 0.6f);
                image.transform.localScale = new Vector3(scale, scale, scale);

                int count = (int)UnityEngine.Random.Range(0.0f, (float)images.Length);
                image.sprite = images[count];

                float powerX = UnityEngine.Random.Range(10.0f, 500.0f);
                float powerY = UnityEngine.Random.Range(300.0f, 1501.0f);
                coin.Initialize(canvas.position + LeftPosition);
                coin.Create(powerX, powerY, 3.0f);
            }
        }

        for (int i = 0; i < effectCount; i++)
        {
            //GameObject coin = Instantiate(coinPrefab, canvas);
            //Debug.Log(coin.GetComponent<Coin>());
            PaperFubuki coin = GetObject();
            //PaperFubuki sc = coin.GetComponent<PaperFubuki>();
            if (coin)
            {
                coin.release += Release;

                coin.rotateZ = UnityEngine.Random.Range(0.5f, 6.0f);

                Image image = coin.gameObject.GetComponent<Image>();

                float scale = UnityEngine.Random.Range(0.3f, 0.6f);
                image.transform.localScale = new Vector3(scale, scale, scale);

                int count = (int)UnityEngine.Random.Range(0.0f, (float)images.Length);
                image.sprite = images[count];

                float powerX = UnityEngine.Random.Range(-500.0f, -10.0f);
                float powerY = UnityEngine.Random.Range(300.0f, 1500.0f);
                coin.Initialize(canvas.position + RightPosition);
                coin.Create(powerX, powerY, 3.0f);
            }
        }
    }
}
