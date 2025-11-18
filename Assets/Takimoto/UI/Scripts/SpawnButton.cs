using UnityEngine;
using UnityEngine.UI;

public class SpawnButton : MonoBehaviour
{
    public TestMonsterParent Monster;
    public Button Button;

    [SerializeField] ObjectPoolTest objectPoolTest;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //ボタンの画像をモンスターのアイコンに変える
        Button.image.sprite = Monster.MonsterIcon;

        objectPoolTest.CreatePool(10);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnButtonDown_Spawn()
    {
        //Monsterをスポーンさせる
        //Instantiate(Monster, Vector3.zero, Quaternion.identity);

        Vector3 position = Vector3.zero;
        //position.x = Random.Range(-50, 50);
        //position.y = Random.Range(-50,50);
        //position.z = Random.Range(-50, 50);
        objectPoolTest.GetObj(position);
    }

    public void Create()
    {
        //TestMonsterPool.Instance.GetEnemy();
        
    }

    public void Clear()
    {
        TestMonsterPool.Instance.ClearEnemy();
    }
}
