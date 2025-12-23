using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class MageModel : UnitModel
{
    private float attackTimer;
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
            if (t != null && t.Model?.IsDead == false)
            {
                if(t.IsSameTeamAs(Owner))
                {
                    if (t.Model.IsBadlyWounded)
                    {
                        Owner.Log("Why do you even return this", LogType.Error);
                        return t;
                    }
                }
                if(false == t.IsSameTeamAs(Owner))
                {
                    return t;
                }
            }
        }
        //敵がいなかったら、味方を回復する
        foreach (var t in targets)
        {
            if (t.Model?.IsWounded == true)
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
        float timergoal = 1f / AttackSpeed;

        if (attackTimer < timergoal * 0.9f)
            Owner.View?.PlayAttack();
        else
            Owner.View?.StopAttack();

        attackTimer += dt;

        if (attackTimer >= timergoal)
        {
            Owner.View?.StopAttack();
            attackTimer = 0f;
            float damage = AttackPower;
            target.TakeDamage(damage);
        }
    }

    public override void PlayerAttack(float dt)
    {
        float timergoal = 1f / AttackSpeed;

        if (attackTimer < timergoal * 0.9f)
            Owner.View?.PlayAttack();
        else
            Owner.View?.StopAttack();

        attackTimer += dt;

        if (attackTimer >= timergoal)
        {
            Owner.View?.StopAttack();
            attackTimer = 0f;
            float damage = AttackPower;
            EnemyPlayer.DecreaseHP(damage);
        }
    }

    public void Heal(UnitPresenter target, float dt)
    {
        float timergoal = 1f / HealSpeed;

        if (HealSpeed < timergoal * 0.9f)
            Owner.View?.PlayAttack(); //回復アニメーションがまだない
        else
            Owner.View?.StopAttack(); //回復アニメーションがまだない

        HealSpeed += dt;

        if (HealSpeed >= timergoal)
        {
            Owner.View?.StopAttack();
            HealSpeed = 0f;
            float heal = HealPower;
            target.ReceiveHeal(heal);
        }
    }
}
