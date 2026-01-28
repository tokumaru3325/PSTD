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

    // 2026.01.27 ウー start
    // private UnitData _knightUnit;
    // private UnitData _archerUnit;
    // private UnitData _mageUnit;
    // 2026.01.27 ウー end

    //26.1.7 滝本海大　start

    //[SerializeField]
    //public ObjectPoolTest KnightObjectPool;
    //[SerializeField]
    //public ObjectPoolTest ArcherObjectPool;
    //[SerializeField]
    //public ObjectPoolTest MageObjectPool;

    [SerializeField]
    public UnitObjectPool UnitObjectPool;

    //26.1.7 滝本海大　end

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
        SubscribeToOnPlayerDeath();
        Initialize();
    }

    private void SubscribeToOnPlayerDeath()
    {
        M_Tower.OnPlayerDeath += OnGameFinished;
    }
    private bool _IsGameFinished = false;
    private void OnGameFinished(string deadplayer)
    {
        _IsGameFinished = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (_IsGameFinished) return;
        _spawnTimer += Time.deltaTime;

        if (_spawnTimer > _spawnCoolDown)
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
        // 2026.01.27 ウー start
        // _knightUnit = UnitObjectPool.KnightData;
        // _archerUnit = UnitObjectPool.ArcherData;
        // _mageUnit = UnitObjectPool.MageData;
        // 2026.01.27 ウー end
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
        // 2026.01.23 ウー start
        //UnitObjectPool.GetObj(UnitID.Knight, _CPUSpawnPos, _knightUnit, _soloPlayerSpawnPos, _CPUTag);
        //UnitObjectPool.GetObj(UnitID.Knight, _CPUSpawnPos, _knightUnit, _soloPlayerSpawnPos, _CPUTag, _soloPlayerTag);
        // 2026.01.25 ウー start
        // 2026.01.27 ウー start
        //UnitObjectPool.GetObj(UnitID.Knight, _CPUSpawnPos, _knightUnit, _soloPlayerSpawnPos, _CPUTag, _soloPlayerTag, PathStrategy.Shortest);
        UnitObjectPool.GetObj(UnitID.Knight, _CPUSpawnPos, _soloPlayerSpawnPos, _CPUTag, _soloPlayerTag, PathStrategy.Shortest);
        // 2026.01.27 ウー end
        // 2026.01.25 ウー end
        // 2026.01.23 ウー end
    }
    private void CPUSpawnArcher()
    {
        // 2026.01.23 ウー start
        //UnitObjectPool.GetObj(UnitID.Archer, _CPUSpawnPos, _archerUnit, _soloPlayerSpawnPos, _CPUTag);
        // 2026.01.25 ウー start
        //UnitObjectPool.GetObj(UnitID.Archer, _CPUSpawnPos, _archerUnit, _soloPlayerSpawnPos, _CPUTag, _soloPlayerTag);
        // 2026.01.27 ウー start
        //UnitObjectPool.GetObj(UnitID.Archer, _CPUSpawnPos, _archerUnit, _soloPlayerSpawnPos, _CPUTag, _soloPlayerTag, PathStrategy.Shortest);
        UnitObjectPool.GetObj(UnitID.Archer, _CPUSpawnPos, _soloPlayerSpawnPos, _CPUTag, _soloPlayerTag, PathStrategy.Shortest);
        // 2026.01.27 ウー end
        // 2026.01.25 ウー end
        // 2026.01.23 ウー end
    }
    private void CPUSpawnMage()
    {
        // 2026.01.23 ウー start
        //UnitObjectPool.GetObj(UnitID.Mage, _CPUSpawnPos, _mageUnit, _soloPlayerSpawnPos, _CPUTag);
        // 2026.01.25 ウー start
        //UnitObjectPool.GetObj(UnitID.Mage, _CPUSpawnPos, _mageUnit, _soloPlayerSpawnPos, _CPUTag, _soloPlayerTag);
        // 2026.01.27 ウー start
        //UnitObjectPool.GetObj(UnitID.Mage, _CPUSpawnPos, _mageUnit, _soloPlayerSpawnPos, _CPUTag, _soloPlayerTag, PathStrategy.Shortest);
        UnitObjectPool.GetObj(UnitID.Mage, _CPUSpawnPos, _soloPlayerSpawnPos, _CPUTag, _soloPlayerTag, PathStrategy.Shortest);
        // 2026.01.27 ウー end
        // 2026.01.25 ウー end
        // 2026.01.23 ウー end
    }
}
