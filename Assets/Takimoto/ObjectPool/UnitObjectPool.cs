using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UnitObjectPool : MonoBehaviour
{
    //[2026/01/13] START プリンス：Unitに継承を伝えるため
    [SerializeField]
    private GameManager _gameManager;

    [SerializeField]
    private DebugManager _debugManager;
    //[2026/01/13] END プリンス

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
            obj.SetPool(this);
            obj.gameObject.SetActive(false);
            KnightPool.Add(obj);

            //アーチャーのオブジェクトプールを貯める
            gameObject = Instantiate(_archerPrefab);
            obj = gameObject.GetComponent<UnitPresenter>();
            obj.SetPool(this);
            obj.gameObject.SetActive(false);
            ArcherPool.Add(obj);

            //メイジのオブジェクトプールを貯める
            gameObject = Instantiate(_magePrefab);
            obj = gameObject.GetComponent<UnitPresenter>();
            obj.SetPool(this);
            obj.gameObject.SetActive(false);
            MagePool.Add(obj);
        }
    }

    //使う時に場所を指定して表示する
    public UnitPresenter GetObj(UnitID unitType, Vector3 position, UnitData data, Vector3 enemyPos, string playerTag) //[2025/12/02] プリンス : "playerTag"追加
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
            Unit.Initialize(data, BuffData, position, enemyPos, playerTag, _gameManager, _debugManager); //[2025/12/02] プリンス : "playerTag"追加 | [2026/01/13]: ", _gameManager, _debugManager" 追加
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
            //[2025/11/18]　プリンス　Start
            newUnit.Initialize(data, BuffData, position, enemyPos, playerTag, _gameManager, _debugManager); //[2025/12/02] プリンス : "playerTag"追加 | [2026/01/13]: ", _gameManager, _debugManager" 追加
            newUnit.SetPool(this);
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
}
