using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public class ReelController : MonoBehaviour
{
    [SerializeField] private GameObject oya;
    [SerializeField] private GameObject reelL;
    [SerializeField] private GameObject reelC;
    [SerializeField] private GameObject reelR;
    [SerializeField] private float ySpeed = 0.0f;

    private float firstPositionL;
    private float firstPositionC;
    private float firstPositionR;

    bool canMoveL = false;
    bool canMoveC = false;
    bool canMoveR = false;

    [SerializeField] ReelMover reelMover;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        firstPositionL = reelL.transform.position.y;
        firstPositionC = reelC.transform.position.y;
        firstPositionR = reelR.transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        InputSlot();
        ReelMove(Time.deltaTime);
    }

    public void ReelMove(float deltaTIme)
    {
        if (reelL && reelC && reelR)
        {
            if (canMoveL)
            {
                reelL.transform.Translate(0, ySpeed * deltaTIme, 0);
                if (reelL.transform.position.y <= 1.0f + oya.transform.position.y)
                {
                    reelL.transform.position = new Vector3(reelL.transform.position.x, firstPositionL, 0);
                }
            }
            if (canMoveC)
            {
                reelC.transform.Translate(0, ySpeed * deltaTIme, 0);
                if (reelC.transform.position.y <= 1.0f + oya.transform.position.y)
                {
                    reelC.transform.position = new Vector3(reelC.transform.position.x, firstPositionC, 0);

                }
            }
            if (canMoveR)
            {
                reelR.transform.Translate(0, ySpeed * deltaTIme, 0);
                if (reelR.transform.position.y <= 1.0f + oya.transform.position.y)
                {
                    reelR.transform.position = new Vector3(reelR.transform.position.x, firstPositionR, 0);

                }
            }
        }
    }

    void InputSlot()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            switch (reelMover.state)
            {
                case ReelMover.State.nextBet:
                    reelMover.StateChange();
                    break;
                case ReelMover.State.nextLeber:
                    reelMover.StateChange();
                    canMoveL = true;
                    canMoveC = true;
                    canMoveR = true;
                    break;
                case ReelMover.State.nextreelL:
                    reelMover.StateChange();
                    canMoveL = false;
                    break;
                case ReelMover.State.nextreelC:
                    reelMover.StateChange();
                    canMoveC = false;
                    break;
                case ReelMover.State.nextreelR:
                    reelMover.StateChange();
                    canMoveR = false;
                    break;
            }
        }
    }
}
