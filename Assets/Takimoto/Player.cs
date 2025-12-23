using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Player : MonoBehaviour
{
    public float Money;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Money = 0.0f;
        //得丸陽生　2025/12/16 start
        SlotSceneManager.AddFuncToMoneySlot(UpdateStateBySlotResult);
        //得丸陽生 end
    }

    // Update is called once per frame
    void Update()
    {
        Money += Time.deltaTime * 3;
    }

    //得丸陽生　2025/12/16 start
    void UpdateStateBySlotResult(int i)
    {
        //ここに効果を書く
        Money += i * 100;
    }
    //得丸陽生 end
}
