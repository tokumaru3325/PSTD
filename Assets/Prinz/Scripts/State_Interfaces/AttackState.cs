using UnityEditor.Experimental.GraphView;
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
     //   Debug.LogWarning($"Enter AttackState {_model.PlayerSide}");
        _presenter.OnEnterState();
        _attackTimer = 0f;
    }
    public void OnExit()
    {
     //   _presenter.View.StopAttack();
    }

    public IUnitState OnUpdate(float dt)
    {
        // Enemy is too far → go back to idle
        if (_presenter.IsValidTargetExist() == false)
        {
            //    Debug.Log("Target is too far");
            return new IdleState(_model, _presenter);
        }
        ////
        var target = _model?.GetPrimaryTarget();
        if (target == null)
        {
/*            if (target != null)
            {
                _model.Targets.Remove(target);
            }*/
            if (_model.IsPlayerInRange == false)
            {
            //    Debug.LogError("Target is null or dead");
                return new IdleState(_model, _presenter);
            }
        }

        ////
        _attackTimer += dt;

        if (_attackTimer >= 1f / _model.AttackSpeed)
        {
            float tmpAT = _attackTimer;
            _attackTimer = 0f;
            if (_model.IsPlayerInRange == true && _model.HasTargetInRange() == false)
            {
                _presenter.PerformPlayerAttack();
            }
            else
            {
                _presenter.PerformBasicAttack(target);
            //    Debug.LogWarning($"attack timer now : {_attackTimer} | attack timer when fired : {tmpAT}");
            }
        }


        return null;
    }

    public IUnitState OnFixedUpdate(float fdt)
    {
        return null;
    }
}
