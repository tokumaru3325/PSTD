using System.Collections.Generic;
using Microsoft.Win32.SafeHandles;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public class ReelController : MonoBehaviour
{

    public AudioClip betSE;
    public AudioClip leberOnSE;
    public AudioClip stopSE;

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

    bool reelLStoped = true;
    bool reelCStoped = true;
    bool reelRStoped = true;

    [SerializeField] ReelMover reelMover;

    [SerializeField] AudioSource audioSource;

    Dictionary<int, float> reelLeftZugaraNum = new Dictionary<int, float>();
    Dictionary<int, float> reelCenterZugaraNum = new Dictionary<int, float>();
    Dictionary<int, float> reelRightZugaraNum = new Dictionary<int, float>();

    int koyakuNum = 0;
    bool koyaku = false;

    bool hazureChange = false;
    int hazureNumKeeper = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        firstPositionL = reelL.transform.position.y;
        firstPositionC = reelC.transform.position.y;
        firstPositionR = reelR.transform.position.y;
        ReelZugaraPositionInit();
    }

    // Update is called once per frame
    void Update()
    {
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
                    reelL.transform.position = new Vector3(reelL.transform.position.x, firstPositionL, oya.transform.position.z);
                }
            }
            else
            {
                if (!reelLStoped)
                {

                    ReelStopper(koyakuNum, koyaku);

                }
            }

            if (canMoveC)
            {
                reelC.transform.Translate(0, ySpeed * deltaTIme, 0);
                if (reelC.transform.position.y <= 1.0f + oya.transform.position.y)
                {
                    reelC.transform.position = new Vector3(reelC.transform.position.x, firstPositionC, oya.transform.position.z);

                }
            }
            else
            {
                if (!reelCStoped)
                {

                    ReelStopper(koyakuNum, koyaku);

                }
            }

            if (canMoveR)
            {
                reelR.transform.Translate(0, ySpeed * deltaTIme, 0);
                if (reelR.transform.position.y <= 1.0f + oya.transform.position.y)
                {
                    reelR.transform.position = new Vector3(reelR.transform.position.x, firstPositionR, oya.transform.position.z);

                }
            }
            else
            {
                if (!reelRStoped)
                {

                    ReelStopper(koyakuNum, koyaku);

                }
            }
        }
    }

    public void InputSlot()
    {
        //if (Keyboard.current.spaceKey.wasPressedThisFrame)
        //{
            switch (reelMover.state)
            {
                case ReelMover.State.nextBet:
                    if (reelLStoped && reelCStoped && reelRStoped)
                    {
                        reelMover.StateChange();
                        reelLStoped = false;
                        reelCStoped = false;
                        reelRStoped = false;
                        audioSource.PlayOneShot(betSE);
                    }

                    break;
                case ReelMover.State.nextLeber:
                    reelMover.StateChange();
                    SetZugara();
                    audioSource.PlayOneShot(leberOnSE);
                    canMoveL = true;
                    canMoveC = true;
                    canMoveR = true;
                    break;
                case ReelMover.State.nextreelL:
                    audioSource.PlayOneShot(stopSE);
                    reelMover.StateChange();
                    canMoveL = false;
                    break;
                case ReelMover.State.nextreelC:
                    if (!reelLStoped) break;
                    audioSource.PlayOneShot(stopSE);
                    reelMover.StateChange();
                    canMoveC = false;
                    break;
                case ReelMover.State.nextreelR:
                    if (!reelCStoped) break;
                    audioSource.PlayOneShot(stopSE);
                    reelMover.StateChange();
                    canMoveR = false;
                    break;
            }
        //}
    }

    bool ReelStopper(int num, bool koyakuok)
    {
        bool no = false;
        switch (reelMover.state)
        {

            case ReelMover.State.nextreelC:
                if (reelLStoped) break;
                if (koyakuok)
                {
                    if (reelLeftZugaraNum[num] + oya.transform.position.y + 0.1f >= reelL.transform.position.y && reelL.transform.position.y >= reelLeftZugaraNum[num] + oya.transform.position.y - 0.1f)
                    {
                        reelL.transform.position = new Vector3(reelL.transform.position.x, reelLeftZugaraNum[num] + oya.transform.position.y, oya.transform.position.z);
                        no = true;
                        reelLStoped = true;
                       
                       // Debug.Log("reelLstop");
                    }
                    else
                    {
                        reelL.transform.Translate(0, ySpeed * Time.deltaTime, 0);
                        if (reelL.transform.position.y <= 1.0f + oya.transform.position.y)
                        {
                            reelL.transform.position = new Vector3(reelL.transform.position.x, firstPositionL, oya.transform.position.z);

                        }
                    }
                }
                else
                {
                    if (!hazureChange)
                    {
                        koyakuNum = (int)Random.Range(0.0f, 5.0f);
                        hazureNumKeeper = koyakuNum;
                        hazureChange = true;
                    }
                    if (reelLeftZugaraNum[koyakuNum] + oya.transform.position.y + 0.1f >= reelL.transform.position.y && reelL.transform.position.y >= reelLeftZugaraNum[koyakuNum] + oya.transform.position.y - 0.1f)
                    {
                        reelL.transform.position = new Vector3(reelL.transform.position.x, reelLeftZugaraNum[koyakuNum] + oya.transform.position.y, oya.transform.position.z);
                        no = true;
                        reelLStoped = true;
                        hazureChange = false;
                       
                        //Debug.Log("reelLstop");
                    }
                    else
                    {
                        reelL.transform.Translate(0, ySpeed * Time.deltaTime, 0);
                        if (reelL.transform.position.y <= 1.0f + oya.transform.position.y)
                        {
                            reelL.transform.position = new Vector3(reelL.transform.position.x, firstPositionL, oya.transform.position.z);

                        }
                    }
                }
                    break;
            case ReelMover.State.nextreelR:
                if (reelCStoped) break;
                if (koyaku)
                {
                    if (reelCenterZugaraNum[num] + oya.transform.position.y + 0.1f >= reelC.transform.position.y && reelC.transform.position.y >= reelCenterZugaraNum[num] + oya.transform.position.y - 0.1f)
                    {
                        reelC.transform.position = new Vector3(reelC.transform.position.x, reelCenterZugaraNum[num] + oya.transform.position.y, oya.transform.position.z);
                        no = true;
                        reelCStoped = true;
                        
                        // Debug.Log("reelCstop");
                    }
                    else
                    {
                        reelC.transform.Translate(0, ySpeed * Time.deltaTime, 0);
                        if (reelC.transform.position.y <= 1.0f + oya.transform.position.y)
                        {
                            reelC.transform.position = new Vector3(reelC.transform.position.x, firstPositionC, oya.transform.position.z);

                        }
                    }
                }
                else
                {
                    if (!hazureChange)
                    {
                        do
                        {
                            koyakuNum = (int)Random.Range(0.0f, 5.0f);
                        } while (hazureNumKeeper == koyakuNum);
                        hazureNumKeeper = koyakuNum;
                        hazureChange = true;
                    }
                    if (reelCenterZugaraNum[koyakuNum] + oya.transform.position.y + 0.1f >= reelC.transform.position.y && reelC.transform.position.y >= reelCenterZugaraNum[koyakuNum] + oya.transform.position.y - 0.1f)
                    {
                        reelC.transform.position = new Vector3(reelC.transform.position.x, reelCenterZugaraNum[koyakuNum] + oya.transform.position.y, oya.transform.position.z);
                        no = true;
                        reelCStoped = true;
                        hazureChange = false;
                        
                        // Debug.Log("reelCstop");
                    }
                    else
                    {
                        reelC.transform.Translate(0, ySpeed * Time.deltaTime, 0);
                        if (reelC.transform.position.y <= 1.0f + oya.transform.position.y)
                        {
                            reelC.transform.position = new Vector3(reelC.transform.position.x, firstPositionC, oya.transform.position.z);

                        }
                    }
                }
                    break;
            case ReelMover.State.nextBet:
                if (reelRStoped) break;
                if (koyaku)
                {
                    if (reelRightZugaraNum[num] + oya.transform.position.y + 0.1f >= reelR.transform.position.y && reelR.transform.position.y >= reelRightZugaraNum[num] + oya.transform.position.y - 0.1f)
                    {
                        reelR.transform.position = new Vector3(reelR.transform.position.x, reelRightZugaraNum[num] + oya.transform.position.y, oya.transform.position.z);
                        no = true;
                        reelRStoped = true;
                        
                        // Debug.Log("reelRstop");
                    }
                    else
                    {
                        reelR.transform.Translate(0, ySpeed * Time.deltaTime, 0);
                        if (reelR.transform.position.y <= 1.0f + oya.transform.position.y)
                        {
                            reelR.transform.position = new Vector3(reelR.transform.position.x, firstPositionR, oya.transform.position.z);

                        }
                    }
                }
                else
                {
                    if (!hazureChange)
                    {
                        do
                        {
                            koyakuNum = (int)Random.Range(0.0f, 5.0f);
                        } while (hazureNumKeeper == koyakuNum);
                        hazureNumKeeper = koyakuNum;
                        hazureChange = true;
                    }
                    if (reelRightZugaraNum[koyakuNum] + oya.transform.position.y + 0.1f >= reelR.transform.position.y && reelR.transform.position.y >= reelRightZugaraNum[koyakuNum] + oya.transform.position.y - 0.1f)
                    {
                        reelR.transform.position = new Vector3(reelR.transform.position.x, reelRightZugaraNum[koyakuNum] + oya.transform.position.y, oya.transform.position.z);
                        no = true;
                        reelRStoped = true;
                        hazureChange = false;
                       
                        //Debug.Log("reelRstop");
                    }
                    else
                    {
                        reelR.transform.Translate(0, ySpeed * Time.deltaTime, 0);
                        if (reelR.transform.position.y <= 1.0f + oya.transform.position.y)
                        {
                            reelR.transform.position = new Vector3(reelR.transform.position.x, firstPositionR, oya.transform.position.z);

                        }
                    }
                }
                    break;
            default:
                break;
        }
        return no;
    }

    void SetZugara()
    {
        int random = (int)Random.Range(0.0f, 100.0f);
        if (random >= 0 && random <= 19)
        {
            koyakuNum = 0;
            koyaku = true;
        }
        else if (random >= 20 && random <= 39)
        {
            koyakuNum = 1;
            koyaku = true;
        }
        else if (random >= 40 && random <= 49)
        {
            koyakuNum = 2;
            koyaku = true;
        }
        else if (random >= 50 && random <= 59)
        {
            koyakuNum = 3;
            koyaku = true;
        }
        else if (random >= 60 && random <= 64)
        {
            koyakuNum = 4;
            koyaku = true;
        }
        else
        {
            koyakuNum = 100;
            koyaku = false;
        }

        Debug.Log(koyakuNum);
    }

    void ReelZugaraPositionInit()
    {

        reelLeftZugaraNum.Add(0, 4.1f);
        reelLeftZugaraNum.Add(1, 2.9f);
        reelLeftZugaraNum.Add(2, 6.3f);
        reelLeftZugaraNum.Add(3, 5.2f);
        reelLeftZugaraNum.Add(4, 1.5f);

        reelCenterZugaraNum.Add(0, 5.15f);
        reelCenterZugaraNum.Add(1, 4.0f);
        reelCenterZugaraNum.Add(2, 1.44f);
        reelCenterZugaraNum.Add(3, 2.85f);
        reelCenterZugaraNum.Add(4, 6.3f);

        reelRightZugaraNum.Add(0, 2.9f);
        reelRightZugaraNum.Add(1, 6.25f);
        reelRightZugaraNum.Add(2, 5.1f);
        reelRightZugaraNum.Add(3, 1.4f);
        reelRightZugaraNum.Add(4, 4.0f);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("enter");
    }
}
