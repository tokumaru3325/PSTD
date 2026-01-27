using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ArcherModel : UnitModel
{
    private float attackTimer;
    private float timerGoal;
    private bool isPlayAttack = false;
    private bool isStopAttack = false;
    private ArcherData ArcherData => (ArcherData)Data;
    public ArcherModel(ArcherData data) : base(data) { }

    public override void Tick(UnitPresenter presenter)
    {

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
            //Owner.Log($"{Owner.name} basic attack", LogType.Error);
            isStopAttack = true;
        }

        attackTimer += dt;

        if (attackTimer >= timerGoal)
        {
            Owner.TriggerIdle();
            isPlayAttack = false;
            isStopAttack= false;
            attackTimer = 0f;
            float damage = TotalAttackPower;
            target.TakeDamage(damage);

            Owner.ShootArrow(target.transform.position);
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
            isStopAttack = true;
        }

        attackTimer += dt;

        if (attackTimer >= timerGoal)
        {
            Owner.TriggerIdle();
            isPlayAttack = false;
            isStopAttack= false;
            attackTimer = 0f;
            float damage = TotalAttackPower;
            EnemyPlayer.DecreaseHP(damage);

            Owner.ShootArrow(EnemyPlayer.transform.position);
        }
    }
}
