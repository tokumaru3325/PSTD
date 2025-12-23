using System;
using System.Threading;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class AttackState : IUnitState
{
    private readonly UnitModel _model;
    private readonly UnitPresenter _presenter;
//    private float _attackTimer;
    private UnitPresenter _target;

    public AttackState(UnitModel model, UnitPresenter presenter)
    {
        _model = model;
        _presenter = presenter;
    }

    public void OnEnter() 
    {
        //   Debug.LogWarning($"Enter AttackState {_model.PlayerSide}");
        _presenter.OnEnterState(this);
    //    _attackTimer = 0f;
        _target = _model?.GetPrimaryTarget();
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
    //    /*var */_target = _model?.GetPrimaryTarget();
        if (_target == null)
        {
            if (_model.IsPlayerInRange == false)
            {
                //    Debug.LogError("Target is null or dead");
                return new IdleState(_model, _presenter);
            }
        }

        if (_model.IsPlayerInRange == true && _model.HasTargetInRange() == false)
        {
            _presenter.PerformPlayerAttack(dt);
        }
        else
        {
            _presenter.PerformBasicAttack(_target, dt);
            //    Debug.LogWarning($"attack timer now : {_attackTimer} | attack timer when fired : {tmpAT}");
        }
        return null;
    }

    public IUnitState OnFixedUpdate(float fdt)
    {
        return null;
    }
}
