using System;
using System.Threading;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class AttackState : IUnitState
{
    private readonly UnitModel _mymodel;
    private readonly UnitPresenter _me;
//    private float _attackTimer;
    private UnitPresenter _target;

    public AttackState(UnitModel model, UnitPresenter presenter)
    {
        _mymodel = model;
        _me = presenter;
    }

    public void OnEnter() 
    {
        //   Debug.LogWarning($"Enter AttackState {_model.PlayerSide}");
        _me.OnEnterState(this);
    //    _attackTimer = 0f;
        _target = _mymodel?.GetPrimaryTarget();
    }
    public void OnExit()
    {
        //   _presenter.View.StopAttack();
    }

    public IUnitState OnUpdate(float dt)
    {
        // Enemy is too far → go back to idle
        if (_me.IsValidTargetExist() == false)
        {
            //    Debug.Log("Target is too far");
            return new IdleState(_mymodel, _me);
        }
        ////
        _target = _mymodel?.GetPrimaryTarget();
        if (_target == null)
        {
            if (_mymodel.IsPlayerInRange == false)
            {
                //    Debug.LogError("Target is null or dead");
                return new IdleState(_mymodel, _me);
            }
        }

        if (_mymodel.IsPlayerInRange == true && _mymodel.HasTargetInRange() == false)
        {
            _me.PerformPlayerAttack(dt);
        }
        else
        {
            if (false == _me.IsSameTeamAs(_target))
            {
                _me.PerformBasicAttack(_target, dt);
            //    _me.Log("PerformBasicAttack() called in AttackState", LogType.Warning);
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
