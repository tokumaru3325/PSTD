using UnityEngine;

public class MageModel : UnitModel
{
    private float attackTimer;
    private MageData MageData => (MageData)Data;

    public MageModel(MageData data) : base(data) { }

    public override void Tick(UnitPresenter presenter)
    {
     //   attackTimer -= Time.deltaTime;

     //   presenter.Move(this.MoveSpeed, this.MoveDirection);

     //   if (attackTimer > 0f)
     //       return;

       /* // Priority 1: Heal nearby ally
        if (presenter.TryGetLowHpAlly(out UnitPresenter ally))
        {
            ally.ReceiveHeal(MageData.HealAmount);
            presenter.PlayHealVFX();
            attackTimer = Data.AttackCooldown;
            return;
        }

        // Priority 2: Attack enemy
        if (presenter.IsEnemyInRange(Data.AttackRange))
        {
            presenter.PerformMagicAttack(Data.AttackDamage);
            attackTimer = Data.AttackCooldown;
        }*/
    }
}
