using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.CompilerServices;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
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
    // 2026.01.17 ウー start
    /// <summary>
    /// 効果中のバフ
    /// </summary>
    private List<C_Buff> _buffs;

    /// <summary>
    /// バフを付与された時の動き
    /// </summary>
    public Action<C_Buff> OnAddBuff;

    /// <summary>
    /// バフを外された時の動き
    /// </summary>
    public Action<C_Buff> OnRemoveBuff;
    // 2026.01.17 ウー end

    private void Start()
    {
        _buffs = new List<C_Buff>();
        SlotSceneManager.OnSlotBuffResult += ReceiveSlotResult;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TestButtonDown();
        }
    }

    private void ReceiveSlotResult(BuffType buffType, string playertag, string enemytag)
    {
        // 2026.01.16 ウー start
        // AddBuff(buffType, playertag, enemytag);
        AddBuff(buffType, playertag).Forget();
        // 2026.01.16 ウー end
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
            C_Timer buffTimer = new();
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

    public async UniTaskVoid AddBuff(BuffType buffType, string playerTag)
    {
        // データ準備
        BuffTypeData buffData = GetBuffData(buffType);
        BuffDataBase target = GetTarget(playerTag);

        if (!buffData || !target)
            return;

        // バフを作る
        C_Buff buff = CreateBuff(buffType, buffData, playerTag);
        _buffs.Add(buff);
        // 通知
        OnAddBuff?.Invoke(buff);

        AddBuufToPlayer(buff, target);
        buff.StartCount(this.GetCancellationTokenOnDestroy());
    }

    public void TestButtonDown()
    {
        // 2026.01.16 ウー start
        // AddBuff(BuffType.AttackPower, "Player1", "Player2");
        AddBuff(BuffType.AttackPower, "Player1").Forget();
        // 2026.01.16 ウー end
    }

    // 2026.01.17 ウー start

    /// <summary>
    /// プレイヤにバフを付与する
    /// </summary>
    /// <param name="buffType">バフタイプ</param>
    /// <param name="buff">バフ</param>
    /// <param name="player">プレイヤー</param>
    private void AddBuufToPlayer(C_Buff buff, BuffDataBase player)
    {
        if (buff.Type == BuffType.AttackPower)
        {
            player.AttackPower += buff.GetBuffValue();
        }
        else if (buff.Type == BuffType.AttackSpeed)
        {
            player.AttackSpeed += buff.GetBuffValue();
        }
        else if (buff.Type == BuffType.AttackRange)
        {
            player.AttackRange += buff.GetBuffValue();
        }
        else if (buff.Type == BuffType.MoveSpeed)
        {
            player.MoveSpeed += buff.GetBuffValue();
        }
    }

    /// <summary>
    /// プレイヤのタグによって、目標のバフデータをゲット
    /// </summary>
    /// <param name="playerTag">プレイヤのタグ</param>
    /// <returns>バフデータ</returns>
    private BuffDataBase GetTarget(string playerTag)
    {
        BuffDataBase target = null;
        if (playerTag.Equals("Player1"))
            target = _buffData1;
        else if (playerTag.Equals("Player2"))
            target = _buffData2;
        return target;
    }

    /// <summary>
    /// バフのデータをゲット
    /// </summary>
    /// <param name="buffType">バフのタイプ</param>
    /// <returns>データ</returns>
    private BuffTypeData GetBuffData(BuffType buffType)
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

        return addBuff;
    }

    /// <summary>
    /// バフを生成
    /// </summary>
    /// <param name="type">バフのタイプ</param>
    /// <param name="buffInfo">バフのデータ</param>
    /// <param name="playerTag">対象のタグ</param>
    /// <returns>バフ</returns>
    private C_Buff CreateBuff(BuffType type, BuffTypeData buffInfo, string playerTag)
    {
        C_Buff buff = new C_Buff(type, playerTag, buffInfo);
        buff.BindTimeComplete(OnTimerFinished);
        return buff;
    }

    /// <summary>
    /// タイマー終了
    /// </summary>
    private void OnTimerFinished(C_Buff buff)
    {
        if (!_buffs.Contains(buff))
            return;

        buff.StopCount();
        OnRemoveBuff?.Invoke(buff);
        BuffDataBase target = GetTarget(buff.TargetTag);
        ReducePlayerBuff(buff, target);
        _buffs.Remove(buff);
    }

    /// <summary>
    /// プレイヤーからバフを外す
    /// </summary>
    /// <param name="buff">バフ</param>
    /// <param name="player">対象</param>
    private void ReducePlayerBuff(C_Buff buff, BuffDataBase player)
    {
        if (buff.Type == BuffType.AttackPower)
        {
            player.AttackPower -= buff.GetBuffValue();
        }
        else if (buff.Type == BuffType.AttackSpeed)
        {
            player.AttackSpeed -= buff.GetBuffValue();
        }
        else if (buff.Type == BuffType.AttackRange)
        {
            player.AttackRange -= buff.GetBuffValue();
        }
        else if (buff.Type == BuffType.MoveSpeed)
        {
            player.MoveSpeed -= buff.GetBuffValue();
        }
    }
    // 2026.01.17 ウー end
}
