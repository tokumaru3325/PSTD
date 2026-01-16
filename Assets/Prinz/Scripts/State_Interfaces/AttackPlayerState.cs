using UnityEngine;


public class AttackPlayerState : IUnitState
{
    private readonly UnitPresenter _me;

    public AttackPlayerState(UnitPresenter presenter)
    {
        _me = presenter;
    }

    public void OnEnter()
    {
        _me.Log($"Enter AttackPlayerState {_me.GetPlayerSide()}", LogType.Warning);
        _me.OnEnterState(this);
    }
    public void OnExit()
    {
        _me.StopAttack();
    }

    public IUnitState OnUpdate(float dt)
    {
        // Enemy is too far → go back to idle
        if (_me.IsPlayerInRange() == false || _me.HasTargetInRange())
        {
            //    Debug.Log("Target is too far");
            return new IdleState(_me);
        }

        _me.PerformPlayerAttack(dt);

        return null;
    }

    public IUnitState OnFixedUpdate(float fdt)
    {
        return null;
    }
}
