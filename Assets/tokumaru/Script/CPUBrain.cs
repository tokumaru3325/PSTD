using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D.IK;


public struct GameState
{
    public int playerUnitCount;          // プレイヤーのモンスター数
    public int playerMoney;              // プレイヤーの所持金

    public int cpuUnitCount;             // CPUのモンスター数
}

public enum CPUAction
{
    SummonKnight,
    SummonArcher,
    SummonMage,
    UseBuffSlot,
    UseMoneySlot,
    UnitSearchWayChange,
    SafeMoney,
    wantToMoneySlot

}

public class ActionWeights
{
    public Dictionary<CPUAction, float> Weights = new()
    {
        { CPUAction.SummonKnight, 1f },
        { CPUAction.SummonArcher, 1f },
        { CPUAction.SummonMage, 1f },
        { CPUAction.UseBuffSlot, 1f },
        { CPUAction.UseMoneySlot, 1f },
        //{ CPUAction.UnitSearchWayChange, 1.0f },
        {CPUAction.SafeMoney,0.5f },
        {CPUAction.wantToMoneySlot,1f },
    };
}



public class CPUBrain : MonoBehaviour
{
    [SerializeField] SpawnButton solverB;
    [SerializeField] SpawnButton archerB;
    [SerializeField] SpawnButton meigeB;


    [SerializeField] CPUBuffList cpuBuffList;
    [SerializeField] V_BuffList playerBuffList;

    [SerializeField] int playerUnit;
    [SerializeField] int CPUUnit;

    GameState state = new GameState();

    private CPUFileLoader executor;
    private CancellationTokenSource cts;

    private int readLineNumber = 0;

    private int fileLineCount = 0;

    [SerializeField] private bool ignoreTimeScale;

    string[][] loadedFile;

    [SerializeField] private Player _CPU;
    [SerializeField] private Player _soloPlayer;

    private Vector3 _CPUSpawnPos;
    private Vector3 _soloPlayerSpawnPos;

    [SerializeField]
    private string _CPUTag = "Player2";
    [SerializeField]
    private string _soloPlayerTag = "Player1";

    const string SPAWN_TAG = "SpawnPos";

    PathStrategy way = PathStrategy.Safe;

    [SerializeField]
    public UnitObjectPool UnitObjectPool;

    /// <summary>
    /// 読み込み間隔（秒）
    /// </summary>
    [SerializeField] private float _interval;

    /// <summary>
    /// 次に読み込む時間
    /// </summary>
    [SerializeField] private float _nextTime;

    /// <summary>
    /// 経過時間トラッカー
    /// </summary>
    private ElapsedTimeCounter _tracker;

    [SerializeField] bool useBrain = false;

    float knightCooldown = 1.0f;
    float archerCooldown = 1.0f;
    float mageCooldown = 1.0f;

    float currentKnightCooldown = 0.0f;
    float currentArcherCooldown = 0.0f;
    float currentMageCooldown = 0.0f;

    [SerializeField] float playBuffSlotMoney = 0.0f;
    [SerializeField] float playMonetSlotMoney = 0.0f;

    [SerializeField] UnitData _Knight;
    [SerializeField] UnitData _Archer;
    [SerializeField] UnitData _Mage;

    ActionWeights actionWeights = new ActionWeights();
    ActionWeights currentWeights = new ActionWeights();

    float waitCount = 0.0f;

    bool wantToSlot = false;
    private void Start()
    {
        solverB.spawn += AddPlayerUnitCount;
        archerB.spawn += AddPlayerUnitCount;
        meigeB.spawn += AddPlayerUnitCount;

        solverB.ripiter += SubPlayerUnitCount;
        archerB.ripiter += SubPlayerUnitCount;
        meigeB.ripiter += SubPlayerUnitCount;

        _CPUSpawnPos = GetSpawnPos(_CPU.gameObject);
        _soloPlayerSpawnPos = GetSpawnPos(_soloPlayer.gameObject);

        if (!useBrain)
        {
            cts = new CancellationTokenSource();
            executor = new CPUFileLoader(
                Path.Combine(Application.streamingAssetsPath, "CPU/data.csv"),
                intervalSeconds: 10f
            );

            _tracker = new ElapsedTimeCounter();

            executor.OnFileLoaded += data =>
            {
                loadedFile = data;
                fileLineCount = data.Length;
            };

            // 経過時間トラッカー開始
            _tracker.OnTick += OnTick;
            _tracker.StartTracking(cts.Token, ignoreTimeScale);

            executor.Start(cts.Token);
        }

    }

    void OnTick(float time)
    {
        if (time >= _nextTime)
        {
            string command = loadedFile[readLineNumber % fileLineCount][0];
            float coolDown = float.Parse(loadedFile[readLineNumber % fileLineCount][1]);

            switch (command)
            {
                case "SpawnEnemy":
                    switch (UnityEngine.Random.Range(0, 3))
                    {
                        case 0:
                            CPUSpawnKnight();
                            break;
                        case 1:
                            CPUSpawnArcher();
                            break;
                        case 2:
                            CPUSpawnMage();
                            break;
                    }
                    break;
                case "Buff":
                    SlotSceneManager.BroadcastBuffSlotResult((BuffType)RondomGetter(), "Player2", "Player1");
                    break;

            }
            readLineNumber++;
            _nextTime += coolDown;
        }
    }

    private void OnDestroy()
    {
        executor?.Dispose();
        cts.Cancel();
    }


    private void CPUSpawnKnight()
    {
        if (currentKnightCooldown > 0.0f) return;
        UnitPresenter ps = UnitObjectPool.GetObj(UnitID.Knight, _CPUSpawnPos, _soloPlayerSpawnPos, _CPUTag, _soloPlayerTag, way);
        state.cpuUnitCount++;
        ps.dead += SubCPUUnitCount;
        currentKnightCooldown = knightCooldown;
        _CPU.Money -= _Knight.BaseUnitCost;
    }
    private void CPUSpawnArcher()
    {
        if (currentArcherCooldown > 0.0f) return;
        UnitPresenter ps = UnitObjectPool.GetObj(UnitID.Archer, _CPUSpawnPos, _soloPlayerSpawnPos, _CPUTag, _soloPlayerTag, way);
        state.cpuUnitCount++;
        ps.dead += SubCPUUnitCount;
        currentArcherCooldown = archerCooldown;
        _CPU.Money -= _Archer.BaseUnitCost;
    }
    private void CPUSpawnMage()
    {
        if (currentMageCooldown > 0.0f) return;
        UnitPresenter ps = UnitObjectPool.GetObj(UnitID.Mage, _CPUSpawnPos, _soloPlayerSpawnPos, _CPUTag, _soloPlayerTag, way);
        state.cpuUnitCount++;
        ps.dead += SubCPUUnitCount;
        currentMageCooldown = mageCooldown;
        _CPU.Money -= _Mage.BaseUnitCost;
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

    int RondomGetter()
    {
        int random = UnityEngine.Random.Range(0, 65);
        if (random >= 0 && random <= 19)
        {
            return 0;
        }
        else if (random >= 20 && random <= 39)
        {
            return 1;
        }
        else if (random >= 40 && random <= 49)
        {
            return 2;
        }
        else if (random >= 50 && random <= 59)
        {
            return 3;
        }
        else if (random >= 60 && random <= 64)
        {
            return 4;
        }
        return 0;
    }


    void AddPlayerUnitCount()
    {
        state.playerUnitCount++;
    }

    void SubPlayerUnitCount()
    {
        state.playerUnitCount--;
    }

    void SubCPUUnitCount()
    {
        state.cpuUnitCount--;
    }

    private CPUAction ApplyCombinedConditions(GameState state)
    {
        var w = new Dictionary<CPUAction, float>(actionWeights.Weights);
        // 1. お金がある × 敵ユニットがいる
        if (_CPU.Money >= 50 && state.playerUnitCount > 0)
        {
            //w[CPUAction.SummonKnight] += 1.5f;
            //w[CPUAction.SummonArcher] += 1f;

            w[CPUAction.SummonKnight] += 0.5f;
        }

        // 2. お金がある × 敵ユニットが多い
        if (_CPU.Money >= 50 && state.playerUnitCount >= 4)
        {
            //w[CPUAction.SummonMage] += 2f;
            w[CPUAction.SummonMage] += 0.5f;
            w[CPUAction.SummonKnight] += 0.5f;
        }

        // 3. お金がある × 敵ユニットがいない
        if (_CPU.Money >= 50 && state.playerUnitCount == 0)
        {
            //w[CPUAction.SummonArcher] += 1.5f;
            //w[CPUAction.SummonMage] += 1f;
            w[CPUAction.wantToMoneySlot] += 0.75f;
            w[CPUAction.SummonArcher] += 0.2f;
            w[CPUAction.SummonMage] += 0.2f;
        }

        // 3. お金がある × 敵ユニットがいない
        if (_CPU.Money >= 50 && state.playerUnitCount <= 2)
        {
            w[CPUAction.wantToMoneySlot] += 0.5f;
            //w[CPUAction.SummonArcher] += 1.5f;
            //w[CPUAction.SummonMage] += 1f;
        }

        // 4. お金がない × 敵ユニットが多い
        if (_CPU.Money < 30 && state.playerUnitCount >= 4)
        {
            //w[CPUAction.UseMoneySlot] += 2f;
            w[CPUAction.UseMoneySlot] += 0.5f;
        }



        // 5. CPU ユニットが少ない × ナイトクールダウン中
        if (state.cpuUnitCount == 1 && knightCooldown <= 0.0f)
        {
            //w[CPUAction.UseMoneySlot] += 1.5f;
            //w[CPUAction.UseBuffSlot] += 1f;
            w[CPUAction.UseMoneySlot] += 0.5f;
            w[CPUAction.UseBuffSlot] += 0.5f;
        }

        // 6. 敵バフあり × CPU バフなし × お金あり
        if (playerBuffList.GetListLength() != 0 && cpuBuffList.GetListLength() == 0 && _CPU.Money >= playBuffSlotMoney)
        {
            //w[CPUAction.UseBuffSlot] += 2f;
            w[CPUAction.UseBuffSlot] += 0.5f;
        }

        // 7. CPU が劣勢 × バフあり
        if (state.playerUnitCount > state.cpuUnitCount && cpuBuffList.GetListLength() != 0)
        {
            //w[CPUAction.SummonKnight] += 1.5f;
            //w[CPUAction.UnitSearchWayChange] += 1f;
            w[CPUAction.SummonKnight] += 0.5f;
            //w[CPUAction.UnitSearchWayChange] += 0.5f;
        }

        // 8. CPU が優勢 × バフなし
        if (state.cpuUnitCount > state.playerUnitCount && cpuBuffList.GetListLength() == 0)
        {
            //w[CPUAction.UseBuffSlot] += 0.5f;
            w[CPUAction.SummonKnight] += 0.5f;
            w[CPUAction.UseBuffSlot] += 0.5f;
        }

        // 9. プレイヤーのお金が多い
        if (state.playerMoney >= 50)
        {
            //w[CPUAction.SummonKnight] += 1f;
            w[CPUAction.SummonArcher] += 1f;
            w[CPUAction.SummonKnight] += 0.5f;
        }

        // 10. プレイヤーのお金が少ない
        if (state.playerMoney < 30)
        {
            //w[CPUAction.wantToMoneySlot] += 0.75f;
            w[CPUAction.SummonKnight] += 0.5f;
        }
        //w[CPUAction.SafeMoney] += 1f;

        // 11. 敵ユニットが多い × CPU バフあり
        if (state.playerUnitCount >= 3 && cpuBuffList.GetListLength() != 0)
        {
            //w[CPUAction.SummonKnight] += 1.5f;
            w[CPUAction.SummonKnight] += 0.5f;
        }

        // 12. 敵ユニットが少ない × CPU バフあり
        if (state.playerUnitCount == 0 && cpuBuffList.GetListLength() != 0)
        {
            w[CPUAction.SummonMage] += 0.2f;
            w[CPUAction.wantToMoneySlot] += 0.75f;
        }

        // 13. CPU ユニットが多い × 敵ユニットが少ない
        if (state.cpuUnitCount >= 3 && state.playerUnitCount == 0)
        {
            // w[CPUAction.SummonArcher] += 1f;
            w[CPUAction.SummonArcher] += 0.5f;
            w[CPUAction.SummonKnight] += 0.5f;
            w[CPUAction.SummonMage] += 0.5f;
        }

        // 14. 敵バフあり × CPU バフあり
        if (playerBuffList.GetListLength() != 0 && cpuBuffList.GetListLength() != 0)
        {
            //w[CPUAction.SummonMage] += 1f;
            w[CPUAction.SummonArcher] += 0.5f;
            w[CPUAction.SummonKnight] += 0.5f;
            w[CPUAction.SummonMage] += 0.5f;
        }

        // 15. CPU が劣勢 × お金がある
        if (state.playerUnitCount > state.cpuUnitCount && _CPU.Money >= _Knight.BaseUnitCost)
        {
            // w[CPUAction.SummonKnight] += 1.5f;
            w[CPUAction.SummonKnight] += 0.5f;
            w[CPUAction.SummonArcher] += 0.5f;
        }

        // 16. CPU が優勢 × お金がある
        if (state.cpuUnitCount > state.playerUnitCount && _CPU.Money >= _Archer.BaseUnitCost)
        {
            //w[CPUAction.SummonArcher] += 1f;
            w[CPUAction.SummonArcher] += 0.5f;
            w[CPUAction.SummonKnight] += 0.5f;
        }

        // 17. 敵ユニットが多い × プレイヤーのお金が多い
        if (state.playerUnitCount >= 4 && state.playerMoney >= 50)
        {
            //w[CPUAction.SummonKnight] += 1f;
            w[CPUAction.SummonKnight] += 0.5f;
            w[CPUAction.SummonArcher] += 0.5f;
            w[CPUAction.SummonMage] += 0.2f;
        }

        // 18. 敵ユニットが少ない × プレイヤーのお金が多い
        if (state.playerUnitCount == 2 && state.playerMoney >= 50)
        {
            //w[CPUAction.SummonMage] += 1f;
            w[CPUAction.SummonMage] += 0.2f;
            w[CPUAction.SummonArcher] += 0.5f;
        }

        // 19. CPU ユニットが多い × CPU バフなし
        if (state.cpuUnitCount >= 3 && cpuBuffList.GetListLength() == 0)
        {
            //w[CPUAction.UseBuffSlot] += 1f;
            w[CPUAction.UseBuffSlot] += 1f;
            w[CPUAction.wantToMoneySlot] += 2.0f;
        }

        // 20. CPU ユニットが少ない × CPU バフあり
        if (state.cpuUnitCount == 0 && cpuBuffList.GetListLength() != 0)
        {
            //w[CPUAction.SummonKnight] += 1f;
            w[CPUAction.SummonKnight] += 0.5f;
        }

        if (_CPU.Money >= 15.0f)
        {
            w[CPUAction.SummonArcher] += 1.0f;
            w[CPUAction.SummonMage] += 1.0f;
            w[CPUAction.SummonKnight] += 0.1f;
        }

        if(state.cpuUnitCount >= 3)
        {
            w[CPUAction.SafeMoney] += 2.5f;
        }

        if (_CPU.Money >= playMonetSlotMoney && state.cpuUnitCount >= 2)
            w[CPUAction.UseMoneySlot] += 2.5f;

        if (_CPU.Money >= playBuffSlotMoney && cpuBuffList.GetListLength() == 0)
            return CPUAction.UseBuffSlot;

        if (currentKnightCooldown > 0.0f || _CPU.Money < _Knight.BaseUnitCost)
            w[CPUAction.SummonKnight] = 0f;

        if (currentArcherCooldown > 0.0f || _CPU.Money < _Archer.BaseUnitCost)
            w[CPUAction.SummonArcher] = 0f;

        if (currentMageCooldown > 0.0f || _CPU.Money < _Mage.BaseUnitCost)
            w[CPUAction.SummonMage] = 0f;

        if (_CPU.Money < playBuffSlotMoney)
            w[CPUAction.UseBuffSlot] = 0f;

        if (_CPU.Money < playMonetSlotMoney)
            w[CPUAction.UseMoneySlot] = 0f;

        if (state.cpuUnitCount >= 4)
        {
            w[CPUAction.wantToMoneySlot] += 0.5f;
        }
        else
        {
            w[CPUAction.wantToMoneySlot] = 0;
        }
        return WeightedRandom(w);




    }


    private CPUAction WeightedRandom(Dictionary<CPUAction, float> w)
    {
        float total = w.Values.Sum();
        float r = UnityEngine.Random.Range(0, total);

        foreach (var pair in w)
        {
            r -= pair.Value;
            if (r <= 0)
                return pair.Key;
        }

        // 念のため
        return CPUAction.SummonKnight;
    }

    public void ExecuteAction(CPUAction action)
    {
        switch (action)
        {
            case CPUAction.SummonKnight:
                CPUSpawnKnight();
                break;

            case CPUAction.SummonArcher:
                CPUSpawnArcher();
                break;

            case CPUAction.SummonMage:
                CPUSpawnMage();
                break;

            case CPUAction.UseBuffSlot:
                SlotSceneManager.BroadcastBuffSlotResult((BuffType)RondomGetter(), "Player2", "Player1");
                _CPU.Money -= playBuffSlotMoney;
                break;

            case CPUAction.UseMoneySlot:

                switch (RondomGetter())
                {
                    case 0:
                        _CPU.Money += 0;
                        break;
                    case 1:
                        _CPU.Money += 15;
                        break;
                    case 2:
                        _CPU.Money += 30;
                        break;
                    case 3:
                        _CPU.Money += 150;
                        break;
                    case 4:
                        _CPU.Money += 300;
                        break;

                }
                _CPU.Money -= playMonetSlotMoney;
                break;

            case CPUAction.UnitSearchWayChange:
                switch (RondomGetter())
                {
                    case 0:
                        way = PathStrategy.Aggressive;
                        break;
                    case 1:
                        way = PathStrategy.Safe;
                        break;
                    case 2:
                        way = PathStrategy.Shortest;
                        break;
                    case 3:
                        way = PathStrategy.Aggressive;
                        break;
                    case 4:
                        way = PathStrategy.Safe;
                        break;

                }
                waitCount = 0.5f;
                break;
            case CPUAction.SafeMoney:
                _CPU.Money += 0.3f;
                waitCount = 2.0f;
                break;
            case CPUAction.wantToMoneySlot:
                wantToSlot = true;
                break;
        }

        foreach (var key in currentWeights.Weights.Where(x => x.Value == 0f).Select(x => x.Key).ToList())
        {
            currentWeights.Weights[key] = 1f;
        }



    }


    private void Update()
    {
        //playerUnit = state.playerUnitCount;
        //CPUUnit = state.cpuUnitCount;
        //Debug.Log($"今のぷれいや所持金{_soloPlayer.Money}今の自分の所持金{_CPU.Money}");
        //Debug.Log($"今のプレイヤーのバフ{playerBuffList.GetListLength()}");
        //Debug.Log($"今のcpuのバフ{cpuBuffList.GetListLength()}");

        if (currentArcherCooldown > 0.0f) currentArcherCooldown -= 1.0f * Time.deltaTime;
        if (currentKnightCooldown > 0.0f) currentKnightCooldown -= 1.0f * Time.deltaTime;
        if (currentMageCooldown > 0.0f) currentMageCooldown -= 1.0f * Time.deltaTime;

        if (useBrain)
        {
            if (waitCount > 0.0f)
            {
                waitCount -= 1.0f * Time.deltaTime;
            }
            else
            {
                if (wantToSlot)
                {
                    if (_CPU.Money >= playMonetSlotMoney)
                    {
                        wantToSlot = false;
                    }
                }
                else
                {
                    CPUAction action = ApplyCombinedConditions(state);
                    ExecuteAction(action);
                    Debug.Log($"今の状態{action}");
                }
            }
        }
    }

}
