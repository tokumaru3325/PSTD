using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public delegate void MoneySlotResult(int i);
public delegate void MonsterSlotResult(int some, int strength, int hatena);
public delegate void BuffSlotResult(int some, int target, int strength);



public class SlotSceneManager : MonoBehaviour
{
    private static MoneySlotResult moneySlotResult = (int i) => { };
    private static MonsterSlotResult monsterSlotResult = (int some, int target, int strength) => { };
    private static BuffSlotResult buffSlotResult = (int some, int target, int strength) => { };
    private static bool openMoney = false;
    private static bool openMonster = false;
    private static bool openBuff = false;
    public static int slotType = 0;
    public static bool reelMoving = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public static void ChangeSceneToMoney()
    {
        if (reelMoving) return;
        if (!openMonster && !openBuff)
        {
            if (openMoney == false)
            {
                SceneManager.LoadScene("Slot", LoadSceneMode.Additive);
                openMoney = true;
                slotType = 0;
                Debug.Log("openMoney");
            }
            else
            {
                SceneManager.UnloadSceneAsync("Slot");
                openMoney = false;
                Debug.Log("closeMoney");
            }
        }
    }

    public static void ChangeSceneToMonster()
    {
        if (reelMoving) return;
        if (!openMoney && !openBuff)
        {
            if (openMonster == false)
            {
                SceneManager.LoadScene("Slot", LoadSceneMode.Additive);
                openMonster = true;
                slotType = 1;
                Debug.Log("openMonster");
            }
            else
            {
                SceneManager.UnloadSceneAsync("Slot");
                openMonster = false;
                Debug.Log("closeMonster");
            }
        }
    }

    public static void ChangeSceneToBuff()
    {
        if (reelMoving) return;
        if (!openMoney && !openMonster)
        {
            if (openBuff == false)
            {
                SceneManager.LoadScene("Slot", LoadSceneMode.Additive);
                openBuff = true;
                slotType = 2;
                Debug.Log("openBuff");
            }
            else
            {
                SceneManager.UnloadSceneAsync("Slot");
                openBuff = false;
                Debug.Log("closeBuff");
            }
        }
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
    }

    public static void BroadcastMoneySlotResult(int i)
    {
        moneySlotResult(i);
    }

    public static void BroadcastMonsterSlotResult(int some, int strength, int hatena)
    {
        monsterSlotResult(some, strength, hatena);
    }

    public static void BroadcastBuffSlotResult(int some, int target, int strength)
    {
        buffSlotResult(some, target, strength);
    }
}
