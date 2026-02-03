using UnityEngine;

public class IdleState : IUnitState
{
    private readonly UnitPresenter _me;
    private float _idleTime;
    private float _idleWaitTime = 0;

    public IdleState(UnitPresenter presenter)
    {
        _me = presenter;
    }

    public void OnEnter()
    {
        _me.Log($"Enter IdleState {_me.GetPlayerSide()}", LogType.Warning);
        _me.OnEnterState(this);
        _me.TriggerIdle();
        _idleTime = 0f;
    }
    public void OnExit() { }

    public IUnitState OnUpdate(float dt)
    {
        if(_idleTime <= _idleWaitTime) //ディレイ
        {
            if (_me.IsValidTargetExist())
            {
                var target = _me.GetPrimaryTarget();
                if (target != null)
                {
                    if (_me.IsSameTeamAs(target))
                    {
                        return new HealState(_me, target); //味方だったら回復する
                    }
                    if (false == _me.IsSameTeamAs(target)) //敵だったら攻撃する
                    {
                        return new AttackState(_me, target);
                    }
                }
                if (_me.IsPlayerInRange())
                {
                    return new AttackPlayerState(_me);
                }
            }
            _idleTime += dt;
            return null;
        }


        if(_me.IsValidTargetExist() == false)
        {
        //    _me.Model.ClearTargets();
            return new MoveState(_me);
        }

        return null;
    }

    public IUnitState OnFixedUpdate(float fdt)
    {
        return null;
    }
}
