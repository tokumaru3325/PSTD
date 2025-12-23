using UnityEngine;

public class IdleState : IUnitState
{
    private readonly UnitModel _mymodel;
    private readonly UnitPresenter _me;
    private float _idleTime;
    private float _idleWaitTime = 0;

    public IdleState(UnitModel model, UnitPresenter presenter)
    {
        _mymodel = model;
        _me = presenter;
    }

    public void OnEnter()
    {
        _me.Log($"Enter IdleState {_mymodel.PlayerSide}", LogType.Warning);
        _me.OnEnterState(this);
        _idleTime = 0f;
    }
    public void OnExit() { }

    public IUnitState OnUpdate(float dt)
    {
        if(_idleTime <= _idleWaitTime) //ディレイ
        {
            if (_me.IsValidTargetExist())
            {
                var target = _mymodel.GetPrimaryTarget();
                if (target != null)
                {
                    if (_me.IsSameTeamAs(target))
                    {
                        return new HealState(_mymodel, _me, target); //味方だったら回復する
                    }
                    if (false == _me.IsSameTeamAs(target)) //敵だったら攻撃する
                    {
                        return new AttackState(_mymodel, _me, target);
                    }
                }
                if (_mymodel.IsPlayerInRange)
                {
                    return new AttackPlayerState(_mymodel, _me);
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
