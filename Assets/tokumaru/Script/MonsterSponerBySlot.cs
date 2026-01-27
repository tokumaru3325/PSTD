using TMPro;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

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

    public void CallBack_Spawn(UnitID type, string playertag)
    {
        //Monsterをスポーンさせる
        if (playertag != _playerTag) return;
        Vector3 mySpawnPos = GetSpawnPos(_player.gameObject);
        Vector3 enemyPos = GetSpawnPos(_enemy.gameObject);

        unitObjectPool.GetObj(type, mySpawnPos, _unit[(int)type], enemyPos, playertag); //[2025/11/20]　プリンス　: 「, Unit」を追加した -> 適切のデータをユニットに与える
        _player.Money -= _unit[(int)type].BaseUnitCost;

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
