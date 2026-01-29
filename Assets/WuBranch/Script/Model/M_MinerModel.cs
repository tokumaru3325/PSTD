using UnityEngine;

public class M_MinerModel : UnitModel
{

    private float _attackTimer;

    public M_MinerModel(M_MinerData data) : base(data)
    {

    }

    public override void BasicAttack(UnitPresenter presenter, float dt)
    {
        _attackTimer += dt;

        if (_attackTimer >= 1f / TotalAttackSpeed)
        {
            _attackTimer = 0f;
            Owner.StopAttack();
            float damage = TotalAttackPower;
            presenter.TakeDamage(damage);
            Owner.PlayAttack();
        }
    }

    public override void PlayerAttack(float dt)
    {
        _attackTimer += dt;

        if (_attackTimer >= 1f / TotalAttackSpeed)
        {
            _attackTimer = 0f;
            Owner.StopAttack();
            float damage = TotalAttackPower;
            if (Obstacle)
                Obstacle.DecreaseHP(damage);
            Owner.PlayAttack();
        }
    }

    public override void Tick(UnitPresenter presenter)
    {

    }
}
