using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class MageModel : UnitModel
{
    private float attackTimer;
    private MageData MageData => (MageData)Data;

    public MageModel(MageData data) : base(data) { }

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
                if(t.Model?.PlayerSide == PlayerSide)
                {
                    if (t.Model.IsBadlyWounded)
                    {
                        Owner.Log("Why do you even return this", LogType.Error);
                        return t;
                    }
                }
                if(t.Model?.PlayerSide != PlayerSide)
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
            float damage = -AttackPower;
            target.TakeDamage(damage);
        }
    }
}
