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
        _attackTimer = 0f;
        _presenter.View.PlayAttack();
    }
    public void OnExit()
    {
        _presenter.View.StopAttack();
    }

    public IUnitState OnUpdate(float dt)
    {
        // Enemy is too far → go back to walking
        if (_model.HasTargetInRange() == false)
        {
            Debug.Log("Target is too far");
            return new MoveState(_model, _presenter);
        }

        ////
        var target = _model.GetPrimaryTarget();
        if (target == null || target.Model.IsDead)
        {
            Debug.LogError("Target is null or dead");
            return new IdleState(_model, _presenter);
        }

/*        //PerformAttack(target);
        return this;*/
     

        ////
        _attackTimer += dt;

        if (_attackTimer >= 1f / _model.AttackSpeed)
        {
            _attackTimer = 0f;
            target.TakeDamage(_model.AttackPower);
            //   _model.TargetEnemy.TakeDamage(_model.AttackPower);
        }


        return null;
    }

    public IUnitState OnFixedUpdate(float fdt)
    {
        return null;
    }
}
