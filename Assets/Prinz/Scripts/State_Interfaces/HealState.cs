using UnityEngine;

public class HealState : IUnitState
{
    private readonly UnitPresenter _me;
    private UnitPresenter _target;

    public HealState(UnitPresenter presenter, UnitPresenter target)
    {
        _me = presenter;
        _target = target;
    }

    public void OnEnter()
    {
        _me.Log($"Enter HealState {_me.GetPlayerSide()}", LogType.Warning);

        _me.OnEnterState(this);
    //    _target = _mymodel?.GetPrimaryTarget();
    }
    public void OnExit()
    {
       _me.StopAttack();
    }

    public IUnitState OnUpdate(float dt)
    {
        // Target is too far → go back to idle
        if (_me.IsValidTargetExist() == false)
        {
            //    Debug.Log("Target is too far");
            return new IdleState(_me);
        }
        ////
        //    _target = _mymodel?.GetPrimaryTarget();
        if (_target == null)
        {
            //    Debug.LogError("Target is null or dead");
            return new IdleState(_me);
        }

        _me.PerformHealSpell(_target, dt);

        return null;
    }

    public IUnitState OnFixedUpdate(float fdt)
    {
        return null;
    }
}
