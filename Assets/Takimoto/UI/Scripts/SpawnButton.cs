using Newtonsoft.Json.Bson;
using System;
using System.Threading;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SpawnButton : MonoBehaviour
{
    //2026.02.02 得丸陽生 start
    public Action spawn;
    public Action ripiter;
    //2026.02.02 得丸陽生 end

    private UnitData _unit; //ScriptableObjectをインスペクターに設定する //[2025/12/21] プリンス：ObjectPoolTestに移した
    public Image SpawnButtonPrefab;

    private Image _imageComp;
    private Button _buttonComp;
    private TextMeshProUGUI _textDecreaseMoney;
    private TextMeshProUGUI _textCost;
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

    // 2026.01.25 ウー start
    /// <summary>
    /// 戦術決定者
    /// </summary>
    [SerializeField]
    private V_StrategySelector _policyMaker;
    // 2026.01.25 ウー end

    const string SPAWN_TAG = "SpawnPos";

    //[2026/01/13] START プリンス
    // 2026.01.28 ウー start 子ども(MinerSpawnButton)にも使わせたい
    //private bool _isGameEnding = false;
    protected bool _isGameEnding = false;
    // 2026.01.28 ウー end
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
    protected virtual void Start()
    {
        GameManager.GameEnding += OnGameEndingNotify;  //[2026/01/13] プリンス 追加

        //ボタンの画像をモンスターのアイコンに変える
        //    Button.image.sprite = Monster.MonsterIcon;

        //2026/01/23 滝本海大　start GameManagerのStart()に移すよ
        //unitObjectPool.CreatePool(5);
        //2026/01/23 滝本海大　end

        // 2026.01.27 ウー start
        // switch (_unitType)
        // {
        //     case UnitID.Knight:
        //         _unit = UnitObjectPool.Instance.KnightData; //[2025/12/21] プリンス：ObjectPoolTestから参照を取得する
        //         break;

        //     case UnitID.Archer:
        //         _unit = UnitObjectPool.Instance.ArcherData; //[2025/12/21] プリンス：ObjectPoolTestから参照を取得する
        //         break;

        //     case UnitID.Mage:
        //         _unit = UnitObjectPool.Instance.MageData; //[2025/12/21] プリンス：ObjectPoolTestから参照を取得する
        //         break;
        // }
        UnitInfo info = UnitObjectPool.Instance.GetUnitInfo(_unitType);
        if (info == null)
        {
            Debug.LogError($"{_unitType}'s data is not attached");
            return;
        }
        _unit = info.Data;
        // 2026.01.27 ウー end


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
            if (Texts[i].gameObject.name == "Text_DecreaseMoney")
            {
                _textDecreaseMoney = Texts[i];
                _textDecreaseMoney.gameObject.SetActive(false);
                _textCompStartPosition = _textDecreaseMoney.rectTransform.anchoredPosition;
                _textDecreaseMoney.SetText("-" + _unit.BaseUnitCost);
            }
            else if (Texts[i].gameObject.name == "Text_Cost")
            {
                _textCost = Texts[i];
                _textCost.SetText("" + _unit.BaseUnitCost);
            }
        }


        //_player = FindAnyObjectByType<Player>();
        _player = GameObject.FindGameObjectWithTag(_playerTag).GetComponent<Player>();
        // 2026.01.28 ウー start 敵には必ずPlayerを持っているに限らない
        //_enemy = GameObject.FindGameObjectWithTag(_enemyTag).GetComponent<Player>();
        _enemy = GameObject.FindGameObjectWithTag(_enemyTag)?.GetComponent<Player>();
        // 2026.01.28 ウー end
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
                _textDecreaseMoney.gameObject.SetActive(false);
            }
            else
            {
                Debug.Log("Text_DecreaseMoney is moving");
                _textDecreaseMoney.rectTransform.anchoredPosition += new Vector2(0.0f, 50.0f) * Time.fixedDeltaTime;
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

    // 2026.01.27 ウー start
    //public void OnButtonDown_Spawn()
    public virtual void OnButtonDown_Spawn()
    // 2026.01.27 ウー end
    {
        if (_isGameEnding) //[2026/01/13] プリンス 追加
            return;

        //Monsterをスポーンさせる

        // 2026.01.28 ウー start
        //Vector3 mySpawnPos = GetSpawnPos(_player.gameObject);
        //Vector3 enemyPos = GetSpawnPos(_enemy.gameObject);
        // 2026.01.23 ウー start
        //unitObjectPool.GetObj(_unitType, mySpawnPos, _unit, enemyPos, _playerTag); //[2025/11/20]　プリンス　: 「, Unit」を追加した -> 適切のデータをユニットに与える
        // 2026.01.25 ウー start
        //unitObjectPool.GetObj(_unitType, mySpawnPos, _unit, enemyPos, _playerTag, _enemyTag);
        //PathStrategy strategy = _policyMaker ? _policyMaker.CurrentStrategy : PathStrategy.Shortest;
        // 2026.01.27 ウー start
        //UnitObjectPool.Instance.GetObj(_unitType, mySpawnPos, _unit, enemyPos, _playerTag, _enemyTag, strategy); //[2025/11/20]　プリンス　: 「, Unit」を追加した -> 適切のデータをユニットに与える
        //UnitObjectPool.Instance.GetObj(_unitType, mySpawnPos, enemyPos, _playerTag, _enemyTag, strategy); //[2025/11/20]　プリンス　: 「, Unit」を追加した -> 適切のデータをユニットに与える
        SpawnUnit();
        // 2026.01.27 ウー end
        // 2026.01.25 ウー end
        // 2026.01.23 ウー end
        // 2026.01.28 ウー end

        //2026.02.02 得丸陽生 start
        spawn?.Invoke();
        //2026.02.02 得丸陽生 end
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

    // 2026.01.28 ウー start
    /// <summary>
    /// ユニットを生成
    /// </summary>
    /// <param name="target">目標</param>
    protected void SpawnUnit(GameObject target = null)
    {
        Vector3 mySpawnPos = GetSpawnPos(_player.gameObject);
        Vector3 enemyPos = target ? target.transform.position : GetSpawnPos(_enemy.gameObject);
        PathStrategy strategy = _policyMaker ? _policyMaker.CurrentStrategy : PathStrategy.Shortest;

        UnitPresenter ps = UnitObjectPool.Instance.GetObj(_unitType, mySpawnPos, enemyPos, _playerTag, _enemyTag, strategy);//[2025/11/20]　プリンス　: 「, Unit」を追加した -> 適切のデータをユニットに与える

        //20260202得丸陽生　start
        //ps.dead += Chukei;
        //20260202得丸陽生　end

        _buttonComp.interactable = false;
        _timer = 0.0f;
        _player.Money -= _unit.BaseUnitCost;

        _textDecreaseMoney.gameObject.SetActive(true);
        _textDecreaseMoney.rectTransform.anchoredPosition = _textCompStartPosition;

        _bPushed = true;
    }
    // 2026.01.28 ウー end

    //2026.02.02 得丸陽生 start
    public void GetSpawn()
    {
        spawn?.Invoke();
    }

    public void Chukei()
    {
        ripiter?.Invoke();
    }
    //2026.02.02 得丸陽生 end
}
