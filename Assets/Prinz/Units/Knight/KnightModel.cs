using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class KnightModel : UnitModel
{
    private float attackTimer;

    public KnightModel(KnightData data) : base(data)
    {

    }

    public override void Tick(UnitPresenter presenter)
    {

    }

    public override void BasicAttack(UnitPresenter target, float dt)
    {
        attackTimer += dt;

        if (attackTimer >= 1f / TotalAttackSpeed)
        {
            attackTimer = 0f;
            Owner.View?.StopAttack();
            float damage = TotalAttackPower;
            target.TakeDamage(damage);
            Owner.View?.PlayAttack();
        }
    }

    public override void PlayerAttack(float dt)
    {
        attackTimer += dt;

        if (attackTimer >= 1f / TotalAttackSpeed)
        {
            attackTimer = 0f;
            Owner.View?.StopAttack();
            float damage = TotalAttackPower;
            EnemyPlayer.DecreaseHP(damage);
            Owner.View?.PlayAttack();
        }
    }
}
