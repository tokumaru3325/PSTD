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

    void AddMoneyBySlot(int num)
    {
        switch (num)
        {
            case 0:
                Money += 100;
                break;
            case 1:
                Money += 200;
                break;
            case 2:
                Money += 300;
                break;
            case 3:
                Money += 400;
                break;
            case 4:
                Money += 500;
                break;

        }
    }
}
