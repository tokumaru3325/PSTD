using UnityEngine;

public  class EffectManager : MonoBehaviour
{

    [SerializeField] private CoinEffect coinEf;
    [SerializeField] private FireEffect fireEf;
    [SerializeField] private SwirlEffect swirlEf;
    [SerializeField] private PaperFubukiEffect pfEf;

    //public static CoinEffect coinEffect;
    //public static GameObject coinPrefab;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        //coinEffect = coinEf;
        //coinPrefab = coinPre;
    }
    void Start()
    {
        coinEf.CreatePool();
        fireEf.CreatePool();
        swirlEf.CreatePool();
        pfEf.CreatePool();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CoinEffectPlay(int counter)
    {
        coinEf.CreateEffect(counter);
    }

    public void FireEffectPlay(int counter)
    {
        fireEf.CreateEffect(counter);
    }

    public void SwirlEffectPlay(int counter)
    {
        swirlEf.CreateEffect(counter);
    }

    public void PaperFubukiEffectPlay(int counter)
    {
        pfEf.CreateEffect(counter);
    }
}
