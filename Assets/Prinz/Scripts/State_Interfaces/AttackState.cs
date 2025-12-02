using UnityEngine;

public class AttackState : IUnitState
{
    private readonly UnitModel _model;
    private readonly UnitPresenter _presenter;
    private float _attackTimer;

    public AttackState(UnitModel model, UnitPresenter presenter)
    {
        _model = model;
        _presenter = presenter;
    }

    public void OnEnter() 
    {
        _attackTimer = 0f;
        _presenter.View.PlayAttack();
    }
    public void OnExit()
    {
        _presenter.View.StopAttack();
    }

    public IUnitState OnUpdate(float dt)
    {

    //    if (_model.TargetEnemy == null)
     //   {
     //       return new MoveState(_model, _presenter);
     //   }

        // Enemy is too far → go back to walking
        if (!_model.HasEnemyInRange())
        {
            return new MoveState(_model, _presenter);
        }

/*        var target = _model.GetPrimaryTarget();

        if (target == null || target.IsDead)
            return new IdleState();

        PerformAttack(target);
        return this;*/

        _attackTimer += dt;

        if (_attackTimer >= 1f / _model.AttackSpeed)
        {
            _attackTimer = 0f;
         //   _model.TargetEnemy.TakeDamage(_model.AttackPower);
        }

        return null;
    }
}
