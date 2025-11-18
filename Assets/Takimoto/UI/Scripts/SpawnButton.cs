using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class SpawnButton : MonoBehaviour
{
    public TestMonsterParent Monster;
    public Button Button;
    public Image Image;

    private float Timer = 0.0f;

    [SerializeField] ObjectPoolTest objectPoolTest;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //ボタンの画像をモンスターのアイコンに変える
        Button.image.sprite = Monster.MonsterIcon;

        objectPoolTest.CreatePool(10);
        Image.fillAmount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(Button.interactable == false)
        {
            Timer += Time.deltaTime;
            Image.fillAmount += Time.deltaTime / Monster.MonsterCoolDown;
            if(Timer > Monster.MonsterCoolDown)
            {
                Button.interactable = true;
                Image.fillAmount = 0;
            }
        }
    }

    public void OnButtonDown_Spawn()
    {
        //Monsterをスポーンさせる
        Vector3 position = Vector3.zero;
        objectPoolTest.GetObj(position);

        Button.interactable = false;
        Timer = 0.0f;
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
