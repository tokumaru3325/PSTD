using UnityEngine;

public class MonsterSponerBySlot : MonoBehaviour
{
    [SerializeField]
    private UnitData[] _unit; //ScriptableObjectをインスペクターに設定する //[2025/12/21] プリンス：ObjectPoolTestに移した


    private Player _player;

    [SerializeField] UnitObjectPool unitObjectPool;

    [SerializeField] Vector3 spawnPosition;

    [SerializeField]
    private string _playerTag;

    [SerializeField]
    private string _enemyTag;

    private Player _enemy;

    const string SPAWN_TAG = "SpawnPos";

    [SerializeField]
    private V_StrategySelector _policyMaker;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _player = GameObject.FindGameObjectWithTag(_playerTag).GetComponent<Player>();
        _enemy = GameObject.FindGameObjectWithTag(_enemyTag).GetComponent<Player>();

        SlotSceneManager.AddFuncToMonsterSlot(CallBack_Spawn);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CallBack_Spawn(string playertag, int n)
    {
        if (_player == null) return;
        //Monsterをスポーンさせる
        if (playertag != _playerTag) return;
        Vector3 mySpawnPos = GetSpawnPos(_player.gameObject);
        Vector3 enemyPos = GetSpawnPos(_enemy.gameObject);

        // 2026.01.27 ウー start
        //unitObjectPool.GetObj(type, mySpawnPos, _unit[(int)type], enemyPos, playertag,); //[2025/11/20]　プリンス　: 「, Unit」を追加した -> 適切のデータをユニットに与える
        UnitID type = UnitID.Knight;
        PathStrategy strategy = _policyMaker ? _policyMaker.CurrentStrategy : PathStrategy.Shortest;
        UnitPresenter ps;
        int count = 0;
        switch (n)
        {
            case 0:
                type = UnitID.Knight;
                ps = unitObjectPool.GetObj(type, mySpawnPos, enemyPos, playertag, _enemyTag, strategy); //[2025/11/20]　プリンス　: 「, Unit」を追加した -> 適切のデータをユニットに与える
                ps.MakeItBoss(2);
                count = 0;
                break;
            case 1:
                type = UnitID.Archer;
                ps = unitObjectPool.GetObj(type, mySpawnPos, enemyPos, playertag, _enemyTag, strategy); //[2025/11/20]　プリンス　: 「, Unit」を追加した -> 適切のデータをユニットに与える
                ps.MakeItBoss(2);
                count = 1;
                break;
            case 2:
                type = UnitID.Archer;
                ps = unitObjectPool.GetObj(type, mySpawnPos, enemyPos, playertag, _enemyTag, strategy); //[2025/11/20]　プリンス　: 「, Unit」を追加した -> 適切のデータをユニットに与える
                ps.MakeItBoss(3);
                count = 1;
                break;
            case 3:
                type = UnitID.Knight;
                ps = unitObjectPool.GetObj(type, mySpawnPos, enemyPos, playertag, _enemyTag, strategy); //[2025/11/20]　プリンス　: 「, Unit」を追加した -> 適切のデータをユニットに与える
                ps.MakeItBoss(3);
                count = 0;
                break;
            case 4:
                type = UnitID.Mage;
                ps = unitObjectPool.GetObj(type, mySpawnPos, enemyPos, playertag, _enemyTag, strategy); //[2025/11/20]　プリンス　: 「, Unit」を追加した -> 適切のデータをユニットに与える
                ps.MakeItBoss(3);
                count = 2;
                break;
        }


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
}
