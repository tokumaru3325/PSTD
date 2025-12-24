using Cysharp.Threading.Tasks;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public enum BuffType
{
    None,
    AttackPower,
    AttackSpeed,
    AttackRange,
    MoveSpeed
}

public class BuffManager : MonoBehaviour
{
    [SerializeField]
    private BuffDataPlayer1 _buffData1;
    [SerializeField]
    private BuffDataPlayer2 _buffData2;

    [SerializeField]
    private BuffAttackPowerData _attackPowerData;
    [SerializeField]
    private BuffAttackSpeedData _attackSpeedData;

    //private List<BuffType> _currentBuff = new List<BuffType>();

    private void Start()
    {
        SlotSceneManager.OnSlotBuffResult += ReceiveSlotResult;
    }

    private void ReceiveSlotResult(BuffType buffType, string playertag, string enemytag)
    {
        AddBuff(buffType, playertag, enemytag);
    }

    public async UniTask AddBuff(BuffType buffType, string playertag, string enemytag)
    {
        BuffTypeData addBuff = null;
        switch (buffType)
        {
        case BuffType.AttackPower:
                addBuff = _attackPowerData;               
                break;

        case BuffType.AttackSpeed:
                addBuff = _attackSpeedData;
                break;

        case BuffType.AttackRange:
                break;

        case BuffType.MoveSpeed:
                break;
        }

        if (addBuff)
        {
            if (playertag == "Player1")
            {
                _buffData1.AttackPower += addBuff.BuffValue;
                await UniTask.Delay(addBuff.BuffTime);
                _buffData1.AttackPower -= addBuff.BuffValue;
            }
            else if (playertag == "Player2")
            {
                _buffData2.AttackPower += addBuff.BuffValue;
                await UniTask.Delay(addBuff.BuffTime);
                _buffData2.AttackPower -= addBuff.BuffValue;
            }
        }        
    }

    public void TestButtonDown()
    {
        AddBuff(BuffType.AttackPower, "Player1", "Player2");
    }
}
