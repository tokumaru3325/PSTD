using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpawnButton : MonoBehaviour
{
    public UnitData Unit; //ScriptableObjectをインスペクターに設定する
    public Image SpawnButtonPrefab;

    private Image _imageComp;
    private Button _buttonComp;
    private TextMeshProUGUI _textComp;
    private Vector2 _textCompStartPosition;

    private float _timer = 10.0f;
    private Player _player;
    private bool _bPushed = false;

    [SerializeField] ObjectPoolTest objectPoolTest;

    [SerializeField] Vector3 spawnPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //ボタンの画像をモンスターのアイコンに変える
    //    Button.image.sprite = Monster.MonsterIcon;

        Image[] Images = SpawnButtonPrefab.GetComponentsInChildren<Image>();
        for(int i = 0; i < Images.Length; i++)
        {
            if (Images[i].gameObject.name == "CoolDown")
            {
                _imageComp = Images[i];    
                break;
            }
        }
        _imageComp.fillAmount = 0;

        Button[] Buttons = SpawnButtonPrefab.GetComponentsInChildren<Button>();
        for(int i = 0; i < Buttons.Length; i++)
        {
            if (Buttons[i].gameObject.name == "Button")
            {
                _buttonComp = Buttons[i];
                break;
            }
        }

        TextMeshProUGUI[] Texts = SpawnButtonPrefab.GetComponentsInChildren<TextMeshProUGUI>();
        for(int i = 0; i < Texts.Length; i++)
        {
            if (Texts[i].gameObject.name == "Text_Cost")
            {
                _textComp = Texts[i];
                _textComp.gameObject.SetActive(false);
                _textCompStartPosition = _textComp.rectTransform.anchoredPosition;
                _textComp.SetText("-" + Unit.UnitCost);
                break;
            }
        }

        _player = FindAnyObjectByType<Player>();

        objectPoolTest.CreatePool(10);        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_bPushed)
        {            
            _timer += Time.fixedDeltaTime;
            Debug.Log("_timer:" + _timer);

            if (_timer >= 0.5f)
            {
                _textComp.gameObject.SetActive(false);
            }
            else
            {
                Debug.Log("Text_Cost is moving");
                _textComp.rectTransform.anchoredPosition += new Vector2(0.0f, 50.0f) * Time.fixedDeltaTime;
            }

            _imageComp.fillAmount += Time.fixedDeltaTime / Unit.UnitCoolDown;
            if (_timer > Unit.UnitCoolDown) //HERE USE UNITDATA INSTEAD !!!!!
            {
                _buttonComp.interactable = true;
                _imageComp.fillAmount = 0;
                _bPushed = false;
            }
        }
        else
        {
            //Debug.Log($"_player:{_player}, Unit:{Unit}, cost:{Unit.UnitCost}");
            if (_player.Money <= Unit.UnitCost)
            {
                _buttonComp.interactable = false;
            }
            else
            {
                _buttonComp.interactable = true;
            }
        }        
    }

    public void OnButtonDown_Spawn()
    {
        //Monsterをスポーンさせる
        objectPoolTest.GetObj(spawnPosition, Unit); //[2025/11/20]　プリンス　: 「, Unit」を追加した -> 適切のデータをユニットに与える

        _buttonComp.interactable = false;
        _timer = 0.0f;
        _player.Money -= Unit.UnitCost;

        _textComp.gameObject.SetActive(true);
        _textComp.rectTransform.anchoredPosition = _textCompStartPosition;

        _bPushed = true;
    }
}
