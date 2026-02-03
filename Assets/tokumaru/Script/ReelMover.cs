using UnityEngine;




public class ReelMover : MonoBehaviour
{
    public enum State
    {
        nextBet,
        //nextLeber,
        nextreelL,
        nextreelC,
        nextreelR
    };
    public class enter<T> : MonoBehaviour
    {
        T num;
        public void input(T a)
        {
            num = a;
        }
    }

    public enum SomeSlot
    {
        moneySlot,
        monsterSlot,
        buffSlot
    };
    private int nowStateNum = 0;
    private int numMax = 0;

    public State state = State.nextBet;
    public SomeSlot someSlot = SomeSlot.moneySlot;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        someSlot = (SomeSlot)SlotSceneManager.slotType;
    }
    void Start()
    {
        state = State.nextBet;
        nowStateNum = (int)state;
        numMax = 4;
    }

    // Update is called once per frame
    void Update()
    {
    }
    public void StateChange()
    {
        nowStateNum++;
        nowStateNum = nowStateNum % numMax;
        state = (State)nowStateNum;
    }
}
