using UnityEngine;

public class ArcherModel : UnitModel
{
    private float attackTimer;
    private ArcherData ArcherData => (ArcherData)Data;
    public ArcherModel(ArcherData data) : base(data) { }

    public override void Tick(UnitPresenter presenter)
    {
      //  attackTimer -= Time.deltaTime;

       // presenter.Move(this.MoveSpeed, this.MoveDirection);


/*        if (presenter.IsEnemyInRange(Data.AttackRange) && attackTimer <= 0f)
        {
            presenter.SpawnProjectile(
                ArcherData.ProjectilePrefab,
                ArcherData.ProjectileSpeed,
                Data.AttackDamage
            );
            attackTimer = Data.AttackCooldown;
        }*/
    }
}
