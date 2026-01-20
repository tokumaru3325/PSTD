using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.CompilerServices;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Unity.VisualScripting;
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
    [SerializeField]
    private BuffTypeData _attackRangeData;
    [SerializeField]
    private BuffTypeData _moveSpeedData;

    //private List<BuffType> _currentBuff = new List<BuffType>();
    // 2026.01.17 ウー start
    /// <summary>
    /// プレイヤー1の効果中のバフ
    /// </summary>
    private List<C_Buff> _player1Buffs;

    /// <summary>
    /// プレイヤー2の効果中のバフ
    /// </summary>
    private List<C_Buff> _player2Buffs;

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
        _player1Buffs = new List<C_Buff>();
        _player2Buffs = new List<C_Buff>();
        SlotSceneManager.OnSlotBuffResult += ReceiveSlotResult;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            AddBuff(BuffType.AttackPower, "Player1").Forget();
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            AddBuff(BuffType.AttackRange, "Player1").Forget();
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            AddBuff(BuffType.AttackSpeed, "Player1").Forget();
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            AddBuff(BuffType.MoveSpeed, "Player1").Forget();
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
        BuffDataBase target = GetPlayerBuffDataByTag(playerTag);
        List<C_Buff> playerBuffs = GetPlayerBuffsByTag(playerTag);

        if (!buffData || !target || playerBuffs == null)
            return;

        // バフを作る
        C_Buff buff = CreateBuff(buffType, buffData, playerTag);
        playerBuffs.Add(buff);
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
    /// タグによって目標のバフデータをゲット
    /// </summary>
    /// <param name="playerTag">プレイヤのタグ</param>
    /// <returns>バフデータ</returns>
    public BuffDataBase GetPlayerBuffDataByTag(string playerTag)
    {
        if (playerTag.Equals("Player1"))
            return _buffData1;
        else if (playerTag.Equals("Player2"))
            return _buffData2;
        return null;
    }

    /// <summary>
    /// タグによってプレイヤのすべてのバフをゲット
    /// </summary>
    /// <param name="tag">タグ</param>
    /// <returns>バフ</returns>
    public List<C_Buff> GetPlayerBuffsByTag(string tag)
    {
        if (tag.Equals("Player1"))
            return _player1Buffs;
        else if (tag.Equals("Player2"))
            return _player2Buffs;
        return null;
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
                addBuff = _attackRangeData;
                break;

            case BuffType.MoveSpeed:
                addBuff = _moveSpeedData;
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
        List<C_Buff> playerBuffs = GetPlayerBuffsByTag(buff.TargetTag);
        if (playerBuffs == null || !playerBuffs.Contains(buff))
            return;

        buff.StopCount();
        BuffDataBase target = GetPlayerBuffDataByTag(buff.TargetTag);
        ReducePlayerBuff(buff, target);
        playerBuffs.Remove(buff);
        OnRemoveBuff?.Invoke(buff);
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
