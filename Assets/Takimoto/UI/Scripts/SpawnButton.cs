using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class SpawnButton : MonoBehaviour
{
    public TestMonsterParent Monster;
    public Button Button;
    private Image _image;

    private float _timer = 0.0f;
    private Player _player;
    private bool _bPushed = false;

    [SerializeField] ObjectPoolTest objectPoolTest;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //ボタンの画像をモンスターのアイコンに変える
        Button.image.sprite = Monster.MonsterIcon;

        Image[] Images = Button.GetComponentsInChildren<Image>();
        for(int i = 0; i < Images.Length; i++)
        {
            if (Images[i].gameObject.name == "CoolDown")
            {
                _image = Images[i];
                break;
            }
        }
        _image.fillAmount = 0;

        _player = FindAnyObjectByType<Player>();

        objectPoolTest.CreatePool(10);        
    }

    // Update is called once per frame
    void Update()
    {
        if (_bPushed)
        {
            _timer += Time.deltaTime;
            _image.fillAmount += Time.deltaTime / Monster.MonsterCoolDown;
            if (_timer > Monster.MonsterCoolDown)
            {
                Button.interactable = true;
                _image.fillAmount = 0;
                _bPushed = false;
            }
        }
        else
        {
            if (_player.Money <= Monster.MonsterCost)
            {
                Button.interactable = false;
            }
            else
            {
                Button.interactable = true;
            }
        }        
    }

    public void OnButtonDown_Spawn()
    {
        //Monsterをスポーンさせる
        Vector3 position = Vector3.zero;
        objectPoolTest.GetObj(position);

        Button.interactable = false;
        _timer = 0.0f;
        _player.Money -= Monster.MonsterCost;

        _bPushed = true;
    }
}
