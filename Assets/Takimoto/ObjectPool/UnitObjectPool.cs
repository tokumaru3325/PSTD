using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class UnitObjectPool : MonoBehaviour
{
    [SerializeField]
    private GameObject _knightPrefab;

    [SerializeField]
    private GameObject _archerPrefab;

    [SerializeField]
    private GameObject _magePrefab;

    [SerializeField]
    public UnitData KnightData; //ScriptableObjectをインスペクターに設定する //[2025/12/21] プリンス：SpawnButtonから移した

    [SerializeField]
    public UnitData ArcherData;

    [SerializeField]
    public UnitData MageData;

    [SerializeField]
    public BuffDataBase BuffData;

    List<UnitPresenter> KnightPool;

    List<UnitPresenter> ArcherPool;

    List<UnitPresenter> MagePool;

    public static UnitObjectPool Instance { get; private set; }

    // 2026.01.18 ウー start
    /// <summary>
    /// バフマネージャー
    /// </summary>
    [SerializeField]
    private BuffManager _buffManager;

    void Awake()
    {
        if (!_buffManager)
            _buffManager = FindFirstObjectByType<BuffManager>();


        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }    
    }

    void Start()
    {
        if (_buffManager)
        {
            _buffManager.OnAddBuff += HandleUpdateBuff;
            _buffManager.OnRemoveBuff += HandleUpdateBuff;
        }
    }
    // 2026.01.18 ウー end

    //最初にいくつプールに貯めておくか
    public void CreatePool(int maxCount)
    {
        KnightPool = new List<UnitPresenter>();
        ArcherPool = new List<UnitPresenter>();
        MagePool = new List<UnitPresenter>();

        for (int i = 0; i < maxCount; i++)
        {
            //ナイトのオブジェクトプールを貯める
            GameObject gameObject = Instantiate(_knightPrefab);
            UnitPresenter obj = gameObject.GetComponent<UnitPresenter>();
            obj.BindPool(this);
            obj.gameObject.SetActive(false);
            KnightPool.Add(obj);

            //アーチャーのオブジェクトプールを貯める
            gameObject = Instantiate(_archerPrefab);
            obj = gameObject.GetComponent<UnitPresenter>();
            obj.BindPool(this);
            obj.gameObject.SetActive(false);
            ArcherPool.Add(obj);

            //メイジのオブジェクトプールを貯める
            gameObject = Instantiate(_magePrefab);
            obj = gameObject.GetComponent<UnitPresenter>();
            obj.BindPool(this);
            obj.gameObject.SetActive(false);
            MagePool.Add(obj);
        }
    }

    //使う時に場所を指定して表示する
    public UnitPresenter GetObj(UnitID unitType, Vector3 position, UnitData data, Vector3 enemyPos, string playerTag, string enemyTag, PathStrategy strategy) //[2025/12/02] プリンス : "playerTag"追加 // 2026.01.23 ウー: enemyTag追加 // 2026.01.25 ウー: strategyを追加
    {
        UnitPresenter Unit = null;
        switch (unitType)
        {
            case UnitID.Knight:
                Unit = GetObjectPool(KnightPool.Count, KnightPool);
                break;

            case UnitID.Archer:
                Unit = GetObjectPool(ArcherPool.Count, ArcherPool);
                break;

            case UnitID.Mage:
                Unit = GetObjectPool(MagePool.Count, MagePool);
                break;
        }

        if (Unit)
        {
            // 2026.01.18 ウー start バフの追加
            List<C_Buff> buffs = _buffManager.GetPlayerBuffsByTag(playerTag);
            List<C_Buff> needShowBuffs = GetShowBuff(buffs);
            // 2026.01.23 ウー start 敵ユニットを追加
            List<UnitPresenter> enemies = GetEnemysUnit(enemyTag);
            //[2025/12/02] プリンス : "playerTag"追加
            //Unit.Initialize(data, BuffData, position, enemyPos, playerTag, _gameManager, _debugManager);
            //Unit.Initialize(data, BuffData, position, enemyPos, playerTag, needShowBuffs);
            // 2026.01.25 ウー start
            //Unit.Initialize(data, BuffData, position, enemyPos, playerTag, needShowBuffs, enemies);
            Unit.Initialize(data, BuffData, position, enemyPos, playerTag, needShowBuffs, enemies, strategy);
            // 2026.01.25 ウー end
            // 2026.01.18 ウー end
            // 2026.01.23 ウー end
            Unit.gameObject.SetActive(true);
            return Unit; //オブジェクトプールに使ってないのがあったらそれを返す
        }

        //全部使っていたら
        GameObject newObj = null;
        switch (unitType)
        {
            case UnitID.Knight:
                newObj = Instantiate(_knightPrefab, position, Quaternion.identity);
                break;

            case UnitID.Archer:
                newObj = Instantiate(_archerPrefab, position, Quaternion.identity);
                break;

            case UnitID.Mage:
                newObj = Instantiate(_magePrefab, position, Quaternion.identity);
                break;
        }

        if (newObj)
        {
            UnitPresenter newUnit = newObj.GetComponent<UnitPresenter>();
            // 2026.01.18 ウー start バフの追加
            List<C_Buff> buffs = _buffManager.GetPlayerBuffsByTag(playerTag);
            List<C_Buff> needShowBuffs = GetShowBuff(buffs);
            //[2025/11/18]　プリンス　Start
            // 2026.01.23 ウー start 敵ユニットを追加
            List<UnitPresenter> enemies = GetEnemysUnit(enemyTag);
            //newUnit.Initialize(data, BuffData, position, enemyPos, playerTag, needShowBuffs);
            // 2026.01.25 ウー start
            //newUnit.Initialize(data, BuffData, position, enemyPos, playerTag, needShowBuffs, enemies);
            newUnit.Initialize(data, BuffData, position, enemyPos, playerTag, needShowBuffs, enemies, strategy); //[2025/12/02] プリンス : "playerTag"追加 | [2026/01/13]: ", _gameManager, _debugManager" 追加
            // 2026.01.25 ウー end
            // 2026.01.18 ウー end
            // 2026.01.23 ウー end
            newUnit.BindPool(this);
            //[2025/11/18]　プリンス　End
            newUnit.gameObject.SetActive(true);
            //   newMonster.ElapsedTime = 0;

            switch (unitType)
            {
                case UnitID.Knight:
                    KnightPool.Add(newUnit);
                    break;

                case UnitID.Archer:
                    ArcherPool.Add(newUnit);
                    break;

                case UnitID.Mage:
                    MagePool.Add(newUnit);
                    break;
            }

            return newUnit;
        }

        return null;
    }

    //[2025/11/20]　プリンス　Start
    public void Release(UnitPresenter unit)
    {
        unit.gameObject.SetActive(false);
    }
    //[2025/11/20]　プリンス　End

    //オブジェクトプールの中から、アクティブじゃないものを探す
    private UnitPresenter GetObjectPool(int count, List<UnitPresenter> ObjectPool)
    {
        for (int i = 0; i < count; i++)
        {
            if (ObjectPool[i].gameObject.activeSelf == false)
            {
                UnitPresenter Unit = ObjectPool[i];
                return Unit;
            }
        }

        return null;
    }

    // 2026.01.18 ウー start
    /// <summary>
    /// バフの更新処理
    /// </summary>
    /// <param name="buff">バフ</param>
    private void HandleUpdateBuff(C_Buff buff)
    {
        if (!_buffManager)
            return;

        List<C_Buff> buffs = _buffManager.GetPlayerBuffsByTag(buff.TargetTag);
        List<C_Buff> needShowBuffs = GetShowBuff(buffs);
        Debug.Log($"need show Buff: {needShowBuffs.Count}, player : {buff.TargetTag}");
        UpdateActivedUnitBuffEffect(needShowBuffs, buff.TargetTag);
    }

    /// <summary>
    /// 表示するバフのタイプを探す
    /// </summary>
    /// <param name="buffs">バフリスト</param>
    /// <returns>表示するバフのタイプ</returns>
    private List<C_Buff> GetShowBuff(List<C_Buff> buffs)
    {
        List<C_Buff> needShow = new List<C_Buff>();
        int maxCount = 4;
        int buffCount = buffs.Count;
        // バフリストから表示するバフを探す
        for (int index = 0; index < buffCount; index++)
        {
            // 表示できるバフのエフェクトは最大４つ
            if (needShow.Count >= maxCount)
                break;

            // 表示するバフの中に既に同じタイプのバフがある場合、表示しない
            if (!needShow.Any(buff => buff.Type == buffs[index].Type))
            {
                needShow.Add(buffs[index]);
            }
        }
        return needShow;
    }

    /// <summary>
    /// 自陣の既に出したユニットのバフエフェクトを更新
    /// </summary>
    /// <param name="needShowBuffs">バフ</param>
    /// <param name="playerTag">タグ</param>
    private void UpdateActivedUnitBuffEffect(List<C_Buff> needShowBuffs, string playerTag)
    {
        // 更新するユニットをゲット
        List<UnitPresenter> knights = FindMyActivedUnit(KnightPool, playerTag);
        List<UnitPresenter> archers = FindMyActivedUnit(ArcherPool, playerTag);
        List<UnitPresenter> mages = FindMyActivedUnit(MagePool, playerTag);
        Debug.Log($"knights: {knights.Count}, archers: {archers.Count}, mages: {mages.Count}");
        // 更新
        knights.ForEach(knight => knight.UpdateBuffEffect(needShowBuffs));
        archers.ForEach(archer => archer.UpdateBuffEffect(needShowBuffs));
        mages.ForEach(mage => mage.UpdateBuffEffect(needShowBuffs));
    }

    /// <summary>
    /// プールから自陣のアクティブユニットを探す
    /// </summary>
    /// <param name="pool">プール</param>
    /// <returns>自陣のアクティブユニット</returns>
    private List<UnitPresenter> FindMyActivedUnit(List<UnitPresenter> pool, string playerTag)
    {
        return pool.Where(obj => obj.gameObject.activeSelf && obj.CompareWithTag(playerTag)).ToList();
    }
    // 2026.01.18 ウー end

    // 2026.01.23 ウー start
    /// <summary>
    /// 敵陣のユニットをゲット
    /// </summary>
    /// <param name="enemyTag">敵陣のタグ</param>
    /// <returns>ユニット</returns>
    private List<UnitPresenter> GetEnemysUnit(string enemyTag)
    {
        // 敵のユニットをゲット
        List<UnitPresenter> knights = FindMyActivedUnit(KnightPool, enemyTag);
        List<UnitPresenter> archers = FindMyActivedUnit(ArcherPool, enemyTag);
        List<UnitPresenter> mages = FindMyActivedUnit(MagePool, enemyTag);

        List<UnitPresenter> enemys = new List<UnitPresenter>();
        enemys.AddRange(knights);
        enemys.AddRange(archers);
        enemys.AddRange(mages);

        return enemys;
    }
    // 2026.01.23 ウー end
}
