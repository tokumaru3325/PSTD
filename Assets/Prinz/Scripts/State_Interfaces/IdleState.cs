using UnityEngine;

public class IdleState : IUnitState
{
    private readonly UnitModel _mymodel;
    private readonly UnitPresenter _me;
    private float _idleTime;

    public IdleState(UnitModel model, UnitPresenter presenter)
    {
        _mymodel = model;
        _me = presenter;
    }

    public void OnEnter()
    { 
    //    Debug.LogWarning($"Enter IdleState {_model.PlayerSide}");
        _me.OnEnterState(this);
        _idleTime = 0f;
    }
    public void OnExit() { }

    public IUnitState OnUpdate(float dt)
    {
        if(_idleTime < 0.5f)
        {
            if (_me.IsValidTargetExist())
            {
                var target = _mymodel.GetPrimaryTarget();
                if (_me.IsSameTeamAs(target))
                {
                    return new HealState(_mymodel, _me);
                }
                else if (false == _me.IsSameTeamAs(target))
                {
                    return new AttackState(_mymodel, _me);
                }
            }
            _idleTime += dt;
            return null;
        }


        if(_me.IsValidTargetExist() == false)
        {
            _me.Model.ClearTargets();
            return new MoveState(_mymodel, _me);
        }

        return null;
    }

    public IUnitState OnFixedUpdate(float fdt)
    {
        return null;
    }
}
