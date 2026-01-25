using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ArcherModel : UnitModel
{
    private float attackTimer;
    private ArcherData ArcherData => (ArcherData)Data;
    public ArcherModel(ArcherData data) : base(data) { }

    public override void Tick(UnitPresenter presenter)
    {

    }

    public override void BasicAttack(UnitPresenter target, float dt)
    {
        float timergoal = 1f / TotalAttackSpeed;

        //2026/01/20 滝本海大 start
        if (attackTimer < timergoal * 0.9f)
        {
            Owner.PlayAttack();
        }
        else
        {
            Owner.StopAttack();
        }

        attackTimer += dt;

        if (attackTimer >= timergoal)
        {
            Owner.StopAttack();
            attackTimer = 0f;
            float damage = TotalAttackPower;
            target.TakeDamage(damage);
            Owner.ShootArrow(target.transform.position);
           /* ArrowPool.Instance.GetObj(Owner.transform.position, target.transform.position);*/
        }
    }

    public override void PlayerAttack(float dt)
    {
        float timergoal = 1f / TotalAttackSpeed;

        if (attackTimer < timergoal * 0.9f)
            Owner.PlayAttack();
        else
            Owner.StopAttack();

        attackTimer += dt;

        if (attackTimer >= timergoal)
        {
            Owner.StopAttack();
            attackTimer = 0f;
            float damage = TotalAttackPower;
            EnemyPlayer.DecreaseHP(damage);
            Owner.ShootArrow(EnemyPlayer.transform.position);
        }
    }
}
