using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public delegate void MoneySlotResult(int i);
public delegate void MonsterSlotResult(int some,int strength,int hatena);
public delegate void BuffSlotResult(int some, int target, int strength);

public class SlotSceneManager : MonoBehaviour
{
    private static MoneySlotResult moneySlotResult = (int i) => { };
    private static MonsterSlotResult monsterSlotResult = (int some,int target,int strength) => { };
    private static BuffSlotResult buffSlotResult = (int some,int target,int strength) => { };
    private static bool open = false;
    private static int slotType = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public static void ChangeScene()
    {
        if(open == false)
        {
            SceneManager.LoadScene("Slot", LoadSceneMode.Additive);
            open = true;
        }
        else
        {
            SceneManager.UnloadSceneAsync("Slot");
            open = false;
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
}
