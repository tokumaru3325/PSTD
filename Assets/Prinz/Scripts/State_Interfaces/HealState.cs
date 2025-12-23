using UnityEngine;

public class HealState : IUnitState
{
    private readonly UnitModel _mymodel;
    private readonly UnitPresenter _me;
    private UnitPresenter _target;

    public HealState(UnitModel model, UnitPresenter presenter)
    {
        _mymodel = model;
        _me = presenter;
    }

    public void OnEnter()
    {
        _me.Log($"Enter AttackState {_mymodel.PlayerSide}", LogType.Warning);

        _me.OnEnterState(this);
        _target = _mymodel?.GetPrimaryTarget();
    }
    public void OnExit()
    {
    }

    public IUnitState OnUpdate(float dt)
    {
        // Target is too far → go back to idle
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
            if (_me.IsSameTeamAs(_target))
            {
                _me.PerformBasicAttack(_target, dt);
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
