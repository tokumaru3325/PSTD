using Newtonsoft.Json.Bson;
using System.Threading;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SpawnButton : MonoBehaviour
{
    private UnitData _unit; //ScriptableObjectをインスペクターに設定する //[2025/12/21] プリンス：ObjectPoolTestに移した
    public Image SpawnButtonPrefab;

    private Image _imageComp;
    private Button _buttonComp;
    private TextMeshProUGUI _textComp;
    private Vector2 _textCompStartPosition;

    private float _timer = 10.0f;
    private Player _player;
    private bool _bPushed = false;

    //2026/01/23 滝本海大 start UnitObjectPoolをシングルトンにしたからいらない
    //[SerializeField] UnitObjectPool unitObjectPool;
    //2026/01/23 滝本海大 end

    [SerializeField] Vector3 spawnPosition;

    [SerializeField]
    private string _playerTag;

    [SerializeField]
    private string _enemyTag;

    [SerializeField]
    private UnitID _unitType;

    private Player _enemy;


    const string SPAWN_TAG = "SpawnPos";

    //[2026/01/13] START プリンス
    private bool _isGameEnding = false;
    private void OnGameEndingNotify(bool gameending, string deadplayertag)
    {
        _isGameEnding = gameending;
    }

    private void OnDisable()
    {
        GameManager.GameEnding -= OnGameEndingNotify;
    }
    //[2026/01/13] END プリンス

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.GameEnding += OnGameEndingNotify;  //[2026/01/13] プリンス 追加

        //ボタンの画像をモンスターのアイコンに変える
        //    Button.image.sprite = Monster.MonsterIcon;

        //2026/01/23 滝本海大　start GameManagerのStart()に移すよ
        //unitObjectPool.CreatePool(5);
        //2026/01/23 滝本海大　end

        switch (_unitType)
        {
            case UnitID.Knight:
                _unit = UnitObjectPool.Instance.KnightData; //[2025/12/21] プリンス：ObjectPoolTestから参照を取得する
                break;

            case UnitID.Archer:
                _unit = UnitObjectPool.Instance.ArcherData; //[2025/12/21] プリンス：ObjectPoolTestから参照を取得する
                break;

            case UnitID.Mage:
                _unit = UnitObjectPool.Instance.MageData; //[2025/12/21] プリンス：ObjectPoolTestから参照を取得する
                break;
        }
        

        Image[] Images = SpawnButtonPrefab.GetComponentsInChildren<Image>();
        for (int i = 0; i < Images.Length; i++)
        {
            if (Images[i].gameObject.name == "CoolDown")
            {
                _imageComp = Images[i];
                break;
            }
        }
        _imageComp.fillAmount = 0;

        Button[] Buttons = SpawnButtonPrefab.GetComponentsInChildren<Button>();
        for (int i = 0; i < Buttons.Length; i++)
        {
            if (Buttons[i].gameObject.name == "Button")
            {
                _buttonComp = Buttons[i];
                break;
            }
        }

        TextMeshProUGUI[] Texts = SpawnButtonPrefab.GetComponentsInChildren<TextMeshProUGUI>();
        for (int i = 0; i < Texts.Length; i++)
        {
            if (Texts[i].gameObject.name == "Text_Cost")
            {
                _textComp = Texts[i];
                _textComp.gameObject.SetActive(false);
                _textCompStartPosition = _textComp.rectTransform.anchoredPosition;
                _textComp.SetText("-" + _unit.BaseUnitCost);
                break;
            }
        }


        //_player = FindAnyObjectByType<Player>();
        _player = GameObject.FindGameObjectWithTag(_playerTag).GetComponent<Player>();
        _enemy = GameObject.FindGameObjectWithTag(_enemyTag).GetComponent<Player>();
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

            _imageComp.fillAmount += Time.fixedDeltaTime / _unit.BaseUnitCoolDown;
            if (_timer > _unit.BaseUnitCoolDown)
            {
                _buttonComp.interactable = true;
                _imageComp.fillAmount = 0;
                _bPushed = false;
            }
        }
        else
        {
            // 2026.01.13 ウー start
            if (!_player)
                return;
            // 2026.01.13 ウー end
            //Debug.Log($"_player:{_player}, Unit:{Unit}, cost:{Unit.UnitCost}");
            if (_player.Money <= _unit.BaseUnitCost)
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
        if(_isGameEnding) //[2026/01/13] プリンス 追加
            return;

        //Monsterをスポーンさせる

        Vector3 mySpawnPos = GetSpawnPos(_player.gameObject);
        Vector3 enemyPos = GetSpawnPos(_enemy.gameObject);

        UnitObjectPool.Instance.GetObj(_unitType, mySpawnPos, _unit, enemyPos, _playerTag); //[2025/11/20]　プリンス　: 「, Unit」を追加した -> 適切のデータをユニットに与える

        _buttonComp.interactable = false;
        _timer = 0.0f;
        _player.Money -= _unit.BaseUnitCost;

        _textComp.gameObject.SetActive(true);
        _textComp.rectTransform.anchoredPosition = _textCompStartPosition;

        _bPushed = true;
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
