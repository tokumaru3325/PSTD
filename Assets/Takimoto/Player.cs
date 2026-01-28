using UnityEngine;
//using static UnityEditor.Experimental.GraphView.GraphView;

public class Player : MonoBehaviour
{
    public float Money;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Money = 0.0f;
        SlotSceneManager.AddFuncToMoneySlot(AddMoneyBySlot);
    }

    // Update is called once per frame
    void Update()
    {
        Money += Time.deltaTime * 3;
    }
    
    public void UseMoney(int some)
    {
        Money -= some;
    }

    void AddMoneyBySlot(int num)
    {
        switch (num)
        {
            case 0:
                Money += 0;
                break;
            case 1:
                Money += 15;
                break;
            case 2:
                Money += 30;
                break;
            case 3:
                Money += 150;
                break;
            case 4:
                Money += 300;
                break;

        }
    }
}
