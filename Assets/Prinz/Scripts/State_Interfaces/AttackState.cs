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
        Debug.LogWarning("Enter AttackState");
     //   _presenter.OnEnterState();
        _attackTimer = 0f;
    }
    public void OnExit()
    {
        _presenter.View.StopAttack();
    }

    public IUnitState OnUpdate(float dt)
    {
    //    if(_model.IsDead) return new DeadState(_model, _presenter);

        // Enemy is too far → go back to walking
        if (_model.HasTargetInRange() == false)
        {
            Debug.Log("Target is too far");
            return new IdleState(_model, _presenter);
        }

        ////
        var target = _model?.GetPrimaryTarget();
        if (target == null || target.Model.IsDead)
        {
            Debug.LogError("Target is null or dead");
            return new IdleState(_model, _presenter);
        }

        ////
        _attackTimer += dt;

        if (_attackTimer >= 1f / _model.AttackSpeed)
        {
            _attackTimer = 0f;
            _presenter.PerformMeleeAttack(target);
        //    _presenter?.View?.PlayAttack();
        //    target.TakeDamage(_model.AttackPower);
        }


        return null;
    }

    public IUnitState OnFixedUpdate(float fdt)
    {
        return null;
    }
}
