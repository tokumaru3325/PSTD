using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class SpawnButton : MonoBehaviour
{
    public UnitData Unit; //ScriptableObjectをインスペクターに設定する
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
    //    Button.image.sprite = Monster.MonsterIcon;

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
            _image.fillAmount += Time.deltaTime / Unit.UnitCoolDown;
            if (_timer > Unit.UnitCoolDown) //HERE USE UNITDATA INSTEAD !!!!!
            {
                Button.interactable = true;
                _image.fillAmount = 0;
                _bPushed = false;
            }
        }
        else
        {
            //Debug.Log($"_player:{_player}, Unit:{Unit}, cost:{Unit.UnitCost}");
            if (_player.Money <= Unit.UnitCost)
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
        objectPoolTest.GetObj(position, Unit); //[2025/11/20]　プリンス　: 「, Unit」を追加した -> 適切のデータをユニットに与える

        Button.interactable = false;
        _timer = 0.0f;
        _player.Money -= Unit.UnitCost;

        _bPushed = true;
    }
}
