using UnityEngine;
//using static UnityEditor.Experimental.GraphView.GraphView;
using System.Collections;

public class CPU : MonoBehaviour
{
    [SerializeField]
    public GameObject KnightSpawner;
    [SerializeField]
    public GameObject ArcherSpawner;
    [SerializeField]
    public GameObject MageSpawner;

    private UnitData _knightUnit;
    private UnitData _archerUnit;
    private UnitData _mageUnit;

    [SerializeField]
    public ObjectPoolTest KnightObjectPool;
    [SerializeField]
    public ObjectPoolTest ArcherObjectPool;
    [SerializeField]
    public ObjectPoolTest MageObjectPool;

    private Player _CPU;
    private Player _soloPlayer;

    private Vector3 _CPUSpawnPos;
    private Vector3 _soloPlayerSpawnPos;

    [SerializeField]
    private string _CPUTag = "Player2";
    [SerializeField]
    private string _soloPlayerTag = "Player1";

    const string SPAWN_TAG = "SpawnPos";

    [SerializeField]
    private float _spawnCoolDown;

    [SerializeField]
    private float _spawnDelay;

    private float _spawnTimer = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _CPU = GameObject.FindGameObjectWithTag(_CPUTag).GetComponent<Player>();
        _soloPlayer = GameObject.FindGameObjectWithTag(_soloPlayerTag).GetComponent<Player>();
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        _spawnTimer += Time.deltaTime;

        if( _spawnTimer > _spawnCoolDown )
        {
            SpawnEachOnce(_spawnDelay);
            _spawnTimer = 0;
        }
    }


    private void Initialize()
    {
        InitializeSpawnPositions();
        InitializeUnitData();
    }

    private void InitializeSpawnPositions()
    {
        _CPUSpawnPos = GetSpawnPos(_CPU.gameObject);
        _soloPlayerSpawnPos = GetSpawnPos(_soloPlayer.gameObject);
    }
    private void InitializeUnitData()
    {
        _knightUnit = KnightObjectPool.UnitData;
        _archerUnit = ArcherObjectPool.UnitData;
        _mageUnit = MageObjectPool.UnitData;
    }

    private Vector3 GetSpawnPos(GameObject player)
    {
        var childs = player.GetComponentsInChildren<Transform>();
        foreach (var child in childs)
        {
            if (child.tag.Equals(SPAWN_TAG))
            {
                return child.transform.position;
            }
        }

        return new(-999, -999, -999);
    }

    private void SpawnEachOnce(float delay)
    {
        StartCoroutine(SpawnWithDelay(delay));
    }
    private IEnumerator SpawnWithDelay(float delay)
    {
        CPUSpawnKnight();
        yield return new WaitForSeconds(delay);
        CPUSpawnArcher();
        yield return new WaitForSeconds(delay);
        CPUSpawnMage();
    }

    private void CPUSpawnKnight()
    {
        KnightObjectPool.GetObj(_CPUSpawnPos, _knightUnit, _soloPlayerSpawnPos, _CPUTag);
    }
    private void CPUSpawnArcher()
    {
        ArcherObjectPool.GetObj(_CPUSpawnPos, _archerUnit, _soloPlayerSpawnPos, _CPUTag);
    }
    private void CPUSpawnMage()
    {
        MageObjectPool.GetObj(_CPUSpawnPos, _mageUnit, _soloPlayerSpawnPos, _CPUTag);
    }
}
