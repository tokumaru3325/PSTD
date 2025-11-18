using UnityEngine;
using UnityEngine.Pool;

public class TestMonsterPool : MonoBehaviour
{
    private static TestMonsterPool _instance;
    public static TestMonsterPool Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = FindObjectOfType<TestMonsterPool>();
            }
            return _instance;
        }
    }

    [SerializeField] private TestMonsterParent _enemyPrefab;  // オブジェクトプールで管理するオブジェクト
    private ObjectPool<TestMonsterParent> _enemyPool;  // オブジェクトプール本体

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _enemyPool = new ObjectPool<TestMonsterParent>(
            createFunc: () => OnCreateObject(),
            actionOnGet: (obj) => OnGetObject(obj),
            actionOnRelease: (obj) => OnReleaseObject(obj),
            actionOnDestroy: (obj) => OnDestroyObject(obj),
            collectionCheck: true,
            defaultCapacity: 3,
            maxSize: 10
            );
    }

    //プールからオブジェクトを取得する
    public TestMonsterParent GetEnemy()
    {
        return _enemyPool.Get();
    }

    //プールの中身を空にする
    public void ClearEnemy()
    {
        _enemyPool.Clear();
    }

    //プールに入れるインスタンスを新しく生成する際に行う処理
    private TestMonsterParent OnCreateObject()
    {
        return Instantiate(_enemyPrefab, transform);
    }

    //プールからインスタンスを取得した際に行う処理
    private void OnGetObject(TestMonsterParent monster)
    {
        monster.transform.position = Random.insideUnitSphere * 5;
        monster.Initialize(() => _enemyPool.Release(monster));
        monster.gameObject.SetActive(true);
    } 

    //プールにインスタンスを返却した際に行う処理
    private void OnReleaseObject(TestMonsterParent monster)
    {
        Debug.Log("Release");
    }

    //プールから削除される際に行う処理
    private void OnDestroyObject(TestMonsterParent monster)
    {
        Destroy(monster.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
