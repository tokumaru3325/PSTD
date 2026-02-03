using System;
//using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public delegate void MoneySlotResult(int i);
public delegate void MonsterSlotResult(UnitID _unitType, string playertag);
public delegate void BuffSlotResult(BuffType type);



public class SlotSceneManager : MonoBehaviour
{
    private static MoneySlotResult moneySlotResult = (int i) => { };
    private static MonsterSlotResult monsterSlotResult = (UnitID _unitTyp, string playertag) => { };
    private static BuffSlotResult buffSlotResult = (BuffType type) => { };
    private static bool openMoney = false;
    private static bool openMonster = false;
    private static bool openBuff = false;
    public static int slotType = 0;
    public static bool reelMoving = false;

    public static string playerTag;
    public static string enemyTag;

    //2025/12/23 滝本海大 start
    public static Action<BuffType, string, string> OnSlotBuffResult;
    //2025/12/23 滝本海大 end

    [SerializeField]
    private string _playerTag;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public static void ChangeSceneToMoney(string ptag, string etag)
    {
        if (reelMoving) return;
        if (!openMonster && !openBuff && !openMoney)
        {
            SceneManager.LoadScene("Slot", LoadSceneMode.Additive);
            playerTag = ptag;
            enemyTag = etag;
            openMoney = true;
            slotType = 0;
            Debug.Log("openMoney");
        }
        else
        {
            if (openMoney)
            {
                SceneManager.UnloadSceneAsync("Slot");
                openMoney = false;
                openBuff = false;
                openMonster = false;
                Debug.Log("closeMoney");
            }
            else
            {
                SceneManager.UnloadSceneAsync("Slot");
                openMoney = false;
                openBuff = false;
                openMonster = false;

                SceneManager.LoadScene("Slot", LoadSceneMode.Additive);
                playerTag = ptag;
                enemyTag = etag;
                openMoney = true;
                slotType = 0;
                Debug.Log("openMoney");
            }
        }
        //if (!openMonster && !openBuff)
        //{
        //    if (openMoney == false)
        //    {
        //        SceneManager.LoadScene("Slot", LoadSceneMode.Additive);
        //        playerTag = ptag;
        //        enemyTag = etag;
        //        openMoney = true;
        //        slotType = 0;
        //        Debug.Log("openMoney");
        //    }
        //    else
        //    {
        //        SceneManager.UnloadSceneAsync("Slot");
        //        openMoney = false;
        //        Debug.Log("closeMoney");
        //    }
        //}
    }

    public static void ChangeSceneToMonster(string ptag, string etag)
    {
        if (reelMoving) return;
        if (!openMonster && !openBuff && !openMoney)
        {
            SceneManager.LoadScene("Slot", LoadSceneMode.Additive);
            playerTag = ptag;
            enemyTag = etag;
            openMonster = true;
            slotType = 1;
            Debug.Log("openMonster");
        }
        else
        {
            if (openMonster)
            {
                SceneManager.UnloadSceneAsync("Slot");
                openMoney = false;
                openBuff = false;
                openMonster = false;
                Debug.Log("closeMoney");
            }
            else
            {
                SceneManager.UnloadSceneAsync("Slot");
                openMoney = false;
                openBuff = false;
                openMonster = false;

                SceneManager.LoadScene("Slot", LoadSceneMode.Additive);
                playerTag = ptag;
                enemyTag = etag;
                openMonster = true;
                slotType = 1;
                Debug.Log("openMonster");
            }
        }
        //if (!openMoney && !openBuff)
        //{
        //    if (openMonster == false)
        //    {
        //        SceneManager.LoadScene("Slot", LoadSceneMode.Additive);
        //        playerTag = ptag;
        //        enemyTag = etag;
        //        openMonster = true;
        //        slotType = 1;
        //        Debug.Log("openMonster");
        //    }
        //    else
        //    {
        //        SceneManager.UnloadSceneAsync("Slot");
        //        openMonster = false;
        //        Debug.Log("closeMonster");
        //    }
        //}
    }

    public static void ChangeSceneToBuff(string ptag, string etag)
    {
        if (reelMoving) return;
        if (!openMonster && !openBuff && !openMoney)
        {
            SceneManager.LoadScene("Slot", LoadSceneMode.Additive);
            playerTag = ptag;
            enemyTag = etag;
            openBuff = true;
            slotType = 2;
            Debug.Log("openMonster");
        }
        else
        {
            if (openBuff)
            {
                SceneManager.UnloadSceneAsync("Slot");
                openMoney = false;
                openBuff = false;
                openMonster = false;
                Debug.Log("closeMoney");
            }
            else
            {
                SceneManager.UnloadSceneAsync("Slot");
                openMoney = false;
                openBuff = false;
                openMonster = false;

                SceneManager.LoadScene("Slot", LoadSceneMode.Additive);
                playerTag = ptag;
                enemyTag = etag;
                openBuff = true;
                slotType = 2;
                Debug.Log("openBuff");
            }
        }
        //if (!openMoney && !openMonster)
        //{
        //    if (openBuff == false)
        //    {
        //        SceneManager.LoadScene("Slot", LoadSceneMode.Additive);
        //        playerTag = ptag;
        //        enemyTag = etag;
        //        openBuff = true;
        //        slotType = 2;
        //        Debug.Log("openBuff");
        //    }
        //    else
        //    {
        //        SceneManager.UnloadSceneAsync("Slot");
        //        openBuff = false;
        //        Debug.Log("closeBuff");
        //    }
        //}
    }

    /// <summary>
    /// 引数int型1つ
    /// </summary>
    /// <param name="sr"></param>
    public static void AddFuncToMoneySlot(MoneySlotResult sr)
    {
        moneySlotResult += sr;
    }

    /// <summary>
    /// 引数int型3つ
    /// </summary>
    /// <param name="sr"></param>
    public static void AddFuncToMonsterSlot(MonsterSlotResult sr)
    {
        monsterSlotResult += sr;
    }

    /// <summary>
    /// 引数int型3つ
    /// </summary>
    /// <param name="sr"></param>
    public static void AddFuncToBuffSlot(BuffSlotResult sr)
    {
        buffSlotResult += sr;

        //2025/12/23 滝本海大 start
        //OnSlotBuffResult?.Invoke(BuffType.None);
        //2025/12/23 滝本海大 end
    }

    public static void BroadcastMoneySlotResult(int i)
    {
        moneySlotResult(i);
    }

    public static void BroadcastMonsterSlotResult(UnitID _unitType)
    {
        monsterSlotResult(_unitType, playerTag);
    }

    public static void BroadcastBuffSlotResult(BuffType type,string playertag,string enemytag)
    {
        buffSlotResult(type);
        //2025/12/23 滝本海大 start
        OnSlotBuffResult?.Invoke(type, playertag, enemytag);
        //2025/12/23 滝本海大 end
    }
}
