using UnityEngine;

public class KnightModel : UnitModel
{
    private UnitStateMachine _usm;
    private float attackTimer;

    public KnightModel(KnightData data) : base(data)
    {
    //    _usm = 
    }

    public override void Tick(UnitPresenter presenter)
    {
        attackTimer -= Time.deltaTime;

        presenter.Move(this.MoveSpeed, this.MoveDirection);

/*        if (presenter.IsEnemyInRange(Data.AttackRange) && attackTimer <= 0f)
        {
            presenter.PerformMeleeAttack(Data.AttackDamage);
            attackTimer = Data.AttackCooldown;
        }*/
    }
}
