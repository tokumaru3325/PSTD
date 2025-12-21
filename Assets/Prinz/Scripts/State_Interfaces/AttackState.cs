using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class AttackState : IUnitState
{
    private readonly UnitModel _model;
    private readonly UnitPresenter _presenter;
    private float _attackTimer;
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
        _attackTimer = 0f;
    }
    public void OnExit()
    {
        //   _presenter.View.StopAttack();
    //    Debug.LogError($"Unit {_model.PlayerSide} died with {_attackTimer} remaining on attack timer");
        if (_attackTimer < 1f && _target != null) //bad but quick fix to avoid the last attack from being ignored
        {
            if (_model.IsPlayerInRange == true && _model.HasTargetInRange() == false)
            {
                _presenter.PerformPlayerAttack(999);
            }
            else
            {
                _presenter.PerformBasicAttack(_target, 999);
            }
        }
    }

    public IUnitState OnUpdate(float dt)
    {
        // Enemy is too far → go back to idle
        if (_presenter.IsValidTargetExist() == false)
        {
        //    _model.AllowAttack(false);
            //    Debug.Log("Target is too far");
            return new IdleState(_model, _presenter);
        }
        ////
        /*var */_target = _model?.GetPrimaryTarget();
        if (_target == null)
        {
            if (_model.IsPlayerInRange == false)
            {
                //    Debug.LogError("Target is null or dead");
            //    _model.AllowAttack(false);
                return new IdleState(_model, _presenter);
            }
        }

    //    _model.AllowAttack(true);

        ////
        _attackTimer += dt;


        if (_model.IsPlayerInRange == true && _model.HasTargetInRange() == false)
        {
            _presenter.PerformPlayerAttack(dt);
        }
        else
        {
            _presenter.PerformBasicAttack(_target, dt);
            //    Debug.LogWarning($"attack timer now : {_attackTimer} | attack timer when fired : {tmpAT}");
        }


       /* if (_attackTimer >= 1f / _model.AttackSpeed)
        {
    //        float tmpAT = _attackTimer;
            _attackTimer = 0f;
            if (_model.IsPlayerInRange == true && _model.HasTargetInRange() == false)
            {
                _presenter.PerformPlayerAttack();
            }
            else
            {
                _presenter.PerformBasicAttack(_target, dt);
            //    Debug.LogWarning($"attack timer now : {_attackTimer} | attack timer when fired : {tmpAT}");
            }
        }*/


        return null;
    }

    public IUnitState OnFixedUpdate(float fdt)
    {
        return null;
    }
}
