using UnityEngine;

public class MageModel : UnitModel
{
    private float attackTimer;
    private MageData MageData => (MageData)Data;

    public MageModel(MageData data) : base(data) { }

    public override void Tick(UnitPresenter presenter)
    {

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
}
