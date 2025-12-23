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
    private BuffData _buffData;

    [SerializeField]
    private BuffAttackPowerData _attackPowerData;
    [SerializeField]
    private BuffAttackSpeedData _attackSpeedData;

    //private List<BuffType> _currentBuff = new List<BuffType>();

    private void Start()
    {
        SlotSceneManager.OnSlotBuffResult += ReceiveSlotResult;
    }

    private void ReceiveSlotResult(BuffType buffType)
    {
        AddBuff(buffType);
    }

    public async UniTask AddBuff(BuffType buffType)
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
            _buffData.AttackPower += addBuff.BuffValue;
            await UniTask.Delay(addBuff.BuffTime);
            _buffData.AttackPower -= addBuff.BuffValue;
        }        
    }

    public void TestButtonDown()
    {
        AddBuff(BuffType.AttackPower);
    }
}
