using System.Collections.Generic;
using Microsoft.Win32.SafeHandles;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
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
    [SerializeField] private float resetY = 0.0f;

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

    [SerializeField] SlotSceneManager slotSceneManager;

    [SerializeField] ResultTextManager resultTextManager;

    [SerializeField] AudioSource audioSource;

    Dictionary<int, float> reelLeftZugaraNum = new Dictionary<int, float>();
    Dictionary<int, float> reelCenterZugaraNum = new Dictionary<int, float>();
    Dictionary<int, float> reelRightZugaraNum = new Dictionary<int, float>();

    int koyakuNum = 0;
    bool koyaku = false;

    bool hazureChange = false;
    int hazureNumKeeper = 0;

    float initialazeYkakeru = 0.0f;

    int slotResultL = 0;
    int slotResultC = 0;
    int slotResultR = 0;

    [SerializeField] RectTransform rt;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        firstPositionL = reelL.transform.position.y;
        firstPositionC = reelC.transform.position.y;
        firstPositionR = reelR.transform.position.y;
        ReelZugaraPositionInit();
        initialazeYkakeru = (rt.sizeDelta.y * rt.localScale.y) / rt.sizeDelta.y;
        Debug.Log($"rt.sizeDelta.x: {initialazeYkakeru}");

        ySpeed = -300 * rt.localScale.y;

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
                if (reelL.transform.position.y <= resetY * initialazeYkakeru + oya.transform.position.y)
                {
                    reelL.transform.position = new Vector3(reelL.transform.position.x, firstPositionL, oya.transform.position.z);
                }
            }
            else
            {
                if (!reelLStoped)
                {

                    ReelStopper(koyakuNum, SlotSceneManager.slotType);

                }
            }

            if (canMoveC)
            {
                reelC.transform.Translate(0, ySpeed * deltaTIme, 0);
                if (reelC.transform.position.y <= resetY * initialazeYkakeru + oya.transform.position.y)
                {
                    reelC.transform.position = new Vector3(reelC.transform.position.x, firstPositionC, oya.transform.position.z);

                }
            }
            else
            {
                if (!reelCStoped)
                {

                    ReelStopper(koyakuNum, SlotSceneManager.slotType);

                }
            }

            if (canMoveR)
            {
                reelR.transform.Translate(0, ySpeed * deltaTIme, 0);
                if (reelR.transform.position.y <= resetY * initialazeYkakeru + oya.transform.position.y)
                {
                    reelR.transform.position = new Vector3(reelR.transform.position.x, firstPositionR, oya.transform.position.z);

                }
            }
            else
            {
                if (!reelRStoped)
                {

                    ReelStopper(koyakuNum, SlotSceneManager.slotType);

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
                resultTextManager.ResetText();
                reelMover.StateChange();
                SetZugara();
                audioSource.PlayOneShot(leberOnSE);
                canMoveL = true;
                canMoveC = true;
                canMoveR = true;
                SlotSceneManager.reelMoving = true;
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
                resultTextManager.TextChange((int)reelMover.someSlot, koyakuNum, koyaku);
                break;
        }
        //}
    }

    bool ReelStopper(int num, int slotType)
    {
        bool no = false;
        switch (reelMover.state)
        {

            case ReelMover.State.nextreelC:
                if (reelLStoped) break;
                if (koyaku)
                {
                    if (reelLeftZugaraNum[num] * initialazeYkakeru + oya.transform.position.y + 1.0f >= reelL.transform.position.y && reelL.transform.position.y >= reelLeftZugaraNum[num] + oya.transform.position.y - 1.0f)
                    {
                        reelL.transform.position = new Vector3(reelL.transform.position.x, reelLeftZugaraNum[num] * initialazeYkakeru + oya.transform.position.y, oya.transform.position.z);
                        no = true;
                        reelLStoped = true;

                        // Debug.Log("reelLstop");
                    }
                    else
                    {
                        reelL.transform.Translate(0, ySpeed * Time.deltaTime, 0);
                        if (reelL.transform.position.y <= resetY * initialazeYkakeru + oya.transform.position.y)
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
                        slotResultL = koyakuNum;
                        hazureNumKeeper = koyakuNum;
                        hazureChange = true;
                    }
                    if (reelLeftZugaraNum[koyakuNum] * initialazeYkakeru + oya.transform.position.y + 1.0f >= reelL.transform.position.y && reelL.transform.position.y >= reelLeftZugaraNum[koyakuNum] + oya.transform.position.y - 1.0f)
                    {
                        reelL.transform.position = new Vector3(reelL.transform.position.x, reelLeftZugaraNum[koyakuNum] * initialazeYkakeru + oya.transform.position.y, oya.transform.position.z);
                        no = true;
                        reelLStoped = true;
                        hazureChange = false;

                        //Debug.Log("reelLstop");
                    }
                    else
                    {
                        reelL.transform.Translate(0, ySpeed * Time.deltaTime, 0);
                        if (reelL.transform.position.y <= resetY * initialazeYkakeru + oya.transform.position.y)
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
                    if (reelCenterZugaraNum[num] * initialazeYkakeru + oya.transform.position.y + 1.0f >= reelC.transform.position.y && reelC.transform.position.y >= reelCenterZugaraNum[num] + oya.transform.position.y - 1.0f)
                    {
                        reelC.transform.position = new Vector3(reelC.transform.position.x, reelCenterZugaraNum[num] * initialazeYkakeru + oya.transform.position.y, oya.transform.position.z);
                        no = true;
                        reelCStoped = true;


                        // Debug.Log("reelCstop");
                    }
                    else
                    {
                        reelC.transform.Translate(0, ySpeed * Time.deltaTime, 0);
                        if (reelC.transform.position.y <= resetY * initialazeYkakeru + oya.transform.position.y)
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
                        slotResultC = koyakuNum;
                        hazureNumKeeper = koyakuNum;
                        hazureChange = true;
                    }
                    if (reelCenterZugaraNum[koyakuNum] * initialazeYkakeru + oya.transform.position.y + 1.0f >= reelC.transform.position.y && reelC.transform.position.y >= reelCenterZugaraNum[koyakuNum] + oya.transform.position.y - 1.0f)
                    {
                        reelC.transform.position = new Vector3(reelC.transform.position.x, reelCenterZugaraNum[koyakuNum] * initialazeYkakeru + oya.transform.position.y, oya.transform.position.z);
                        no = true;
                        reelCStoped = true;
                        hazureChange = false;

                        // Debug.Log("reelCstop");
                    }
                    else
                    {
                        reelC.transform.Translate(0, ySpeed * Time.deltaTime, 0);
                        if (reelC.transform.position.y <= resetY * initialazeYkakeru + oya.transform.position.y)
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
                    if (reelRightZugaraNum[num] * initialazeYkakeru + oya.transform.position.y + 1.0f >= reelR.transform.position.y && reelR.transform.position.y >= reelRightZugaraNum[num] + oya.transform.position.y - 1.0f)
                    {
                        reelR.transform.position = new Vector3(reelR.transform.position.x, reelRightZugaraNum[num] * initialazeYkakeru + oya.transform.position.y, oya.transform.position.z);
                        no = true;
                        reelRStoped = true;
                        switch (slotType)
                        {
                            case 0:
                                SlotSceneManager.BroadcastMoneySlotResult(koyakuNum);
                                break;
                            case 1:
                                if (koyakuNum >= 3) koyakuNum = 2;
                                SlotSceneManager.BroadcastMonsterSlotResult((UnitID)koyakuNum); //(UnitID)koyakuNum
                                break;
                            case 2:
                                SlotSceneManager.BroadcastBuffSlotResult((BuffType)koyakuNum);
                                break;

                        }
                        SlotSceneManager.reelMoving = false;
                        // Debug.Log("reelRstop");
                    }
                    else
                    {
                        reelR.transform.Translate(0, ySpeed * Time.deltaTime, 0);
                        if (reelR.transform.position.y <= resetY * initialazeYkakeru + oya.transform.position.y)
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
                        slotResultR = koyakuNum;
                        hazureNumKeeper = koyakuNum;
                        hazureChange = true;
                    }
                    if (reelRightZugaraNum[koyakuNum] * initialazeYkakeru + oya.transform.position.y + 1.0f >= reelR.transform.position.y && reelR.transform.position.y >= reelRightZugaraNum[koyakuNum] + oya.transform.position.y - 1.0f)
                    {
                        reelR.transform.position = new Vector3(reelR.transform.position.x, reelRightZugaraNum[koyakuNum] * initialazeYkakeru + oya.transform.position.y, oya.transform.position.z);
                        no = true;
                        reelRStoped = true;
                        hazureChange = false;
                        switch (slotType)
                        {
                            case 1:
                                SlotSceneManager.BroadcastMonsterSlotResult((UnitID)koyakuNum);
                                break;
                            case 2:
                                SlotSceneManager.BroadcastBuffSlotResult((BuffType)koyakuNum);
                                break;

                        }
                        SlotSceneManager.reelMoving = false;
                        //Debug.Log("reelRstop");
                    }
                    else
                    {
                        reelR.transform.Translate(0, ySpeed * Time.deltaTime, 0);
                        if (reelR.transform.position.y <= resetY * initialazeYkakeru + oya.transform.position.y)
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
        int random = Random.Range(0, 65);
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

        Debug.Log(koyakuNum);
    }

    void ReelZugaraPositionInit()
    {
        
        reelLeftZugaraNum.Add(0, 25.0f);
        reelLeftZugaraNum.Add(1, 45.0f);
        reelLeftZugaraNum.Add(2, 80.0f);
        reelLeftZugaraNum.Add(3, 63.0f);
        reelLeftZugaraNum.Add(4, 2.0f);

        reelCenterZugaraNum.Add(0, 43.0f);
        reelCenterZugaraNum.Add(1, 63.0f);
        reelCenterZugaraNum.Add(2, 0.0f);
        reelCenterZugaraNum.Add(3, 23.0f);
        reelCenterZugaraNum.Add(4, 80.0f);

        reelRightZugaraNum.Add(0, 80.0f);
        reelRightZugaraNum.Add(1, 26.0f);
        reelRightZugaraNum.Add(2, 62.0f);
        reelRightZugaraNum.Add(3, 0.8f);
        reelRightZugaraNum.Add(4, 44.0f);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("enter");
    }
}
