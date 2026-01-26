using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class MageModel : UnitModel
{
    private float attackTimer;
    private float timerGoal;

    private bool isPlayAttack = false;
    private bool isStopAttack = false;
    public float HealPower {  get; private set; }
    public float HealSpeed { get; private set; }
    private MageData MageData => (MageData)Data;

    public MageModel(MageData data) : base(data) 
    { 
        HealPower = data.BaseHealPower;
        HealSpeed = data.BaseHealSpeed;
    }

    public override void Tick(UnitPresenter presenter)
    {

    }

    public override UnitPresenter GetPrimaryTarget()
    {
        if (targets.Count == 0) return null;
        foreach (var t in targets)
        {
            if (t != null && t.IsDead() == false)
            {
                if(Owner.IsSameTeamAs(t))
                {
                    if (t.IsBadlyWounded())
                    {
                    //    Owner.Log("Why do you even return this", LogType.Error);
                        return t;
                    }
                }
                if(false == Owner.IsSameTeamAs(t))
                {
                    return t;
                }
            }
        }
        //敵がいなかったら、味方を回復する
        foreach (var t in targets)
        {
            if (t.IsWounded() == true)
            {
                return t;
            }
            else
            {
            //    Owner.Log($"Mage removes {t} from targets", LogType.Warning);
            //    t.Model?.RemoveTarget(t);
            }
        }

        return null;

    //    return base.GetPrimaryTarget();
    }

    public override void BasicAttack(UnitPresenter target, float dt)
    {
        timerGoal = 1f / TotalAttackSpeed;

        if (attackTimer < timerGoal * 0.9f)
        {
            if (!isPlayAttack)
            {
                Owner.PlayAttack();
                isPlayAttack = true;
            }
        }
        else if (!isStopAttack)
        {
            Owner.StopAttack();
            Owner.Log($"{Owner.name} basic attack", LogType.Error);
            isStopAttack = true;
        }

        attackTimer += dt;

        if (attackTimer >= timerGoal)
        {
            Owner.TriggerIdle();
            isPlayAttack = false;
            isStopAttack = false;
            attackTimer = 0f;
            float damage = TotalAttackPower;
            target.TakeDamage(damage);
        }
    }

    public override void PlayerAttack(float dt)
    {
        timerGoal = 1f / TotalAttackSpeed;

        if (attackTimer < timerGoal * 0.9f)
        {
            if (!isPlayAttack)
            {
                Owner.PlayAttack();
                isPlayAttack = true;
            }
        }
        else if (!isStopAttack)
        {
            Owner.StopAttack();
            Owner.Log($"{Owner.name} basic attack", LogType.Error);
            isStopAttack = true;
        }

        attackTimer += dt;

        if (attackTimer >= timerGoal)
        {
            Owner.TriggerIdle();
            isPlayAttack = false;
            isStopAttack = false;
            attackTimer = 0f;
            float damage = TotalAttackPower;
            EnemyPlayer?.DecreaseHP(damage);
        }
    }

    public override void Heal(UnitPresenter target, float dt)
    {
        //    Owner.Log("Heal performed in MageModel", LogType.Warning);

        timerGoal = 1f / HealSpeed;

        if (HealSpeed < timerGoal * 0.9f)
        {
            if (!isPlayAttack)
            {
                Owner.PlayAttack(); //回復アニメーションがまだない
                isPlayAttack = true;
            }
        }
        else if (!isStopAttack)
        {
            Owner.StopAttack();
            Owner.Log($"{Owner.name} basic attack", LogType.Error);
            isStopAttack = true;
        }

        HealSpeed += dt;

        if (HealSpeed >= timerGoal)
        {
            Owner.TriggerIdle();
            isPlayAttack = false;
            isStopAttack = false;
            HealSpeed = 0f;
            float heal = HealPower;
            target?.ReceiveHeal(heal);
        }
    }
}
