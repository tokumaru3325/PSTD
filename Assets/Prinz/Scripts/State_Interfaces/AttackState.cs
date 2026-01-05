using System;
using System.Threading;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class AttackState : IUnitState
{
    private readonly UnitModel _mymodel;
    private readonly UnitPresenter _me;
    private UnitPresenter _target;

    public AttackState(UnitModel model, UnitPresenter presenter, UnitPresenter target)
    {
        _mymodel = model;
        _me = presenter;
        _target = target;
    }

    public void OnEnter() 
    {
        _me.Log($"Enter AttackState {_mymodel.PlayerSide}", LogType.Warning);
        _me.OnEnterState(this);
    //    _target = _mymodel?.GetPrimaryTarget();
    }
    public void OnExit()
    {
        _me.View?.StopAttack();
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
        //    _target = _mymodel?.GetPrimaryTarget();
        if (_target == null)
        {
            //    Debug.LogError("Target is null or dead");
            return new IdleState(_mymodel, _me);
        }

        _me.PerformBasicAttack(_target, dt);
        //    _me.Log("PerformBasicAttack() called in AttackState", LogType.Warning);
        //    Debug.LogWarning($"attack timer now : {_attackTimer} | attack timer when fired : {tmpAT}");


        return null;
    }

    public IUnitState OnFixedUpdate(float fdt)
    {
        return null;
    }
}
