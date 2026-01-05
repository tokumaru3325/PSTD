using UnityEngine;


public class AttackPlayerState : IUnitState
{
    private readonly UnitModel _mymodel;
    private readonly UnitPresenter _me;

    public AttackPlayerState(UnitModel model, UnitPresenter presenter)
    {
        _mymodel = model;
        _me = presenter;
    }

    public void OnEnter()
    {
        _me.Log($"Enter AttackPlayerState {_mymodel.PlayerSide}", LogType.Warning);
        _me.OnEnterState(this);
    }
    public void OnExit()
    {
        _me.View?.StopAttack();
    }

    public IUnitState OnUpdate(float dt)
    {
        // Enemy is too far → go back to idle
        if (_mymodel.IsPlayerInRange == false || _mymodel.HasTargetInRange())
        {
            //    Debug.Log("Target is too far");
            return new IdleState(_mymodel, _me);
        }

        _me.PerformPlayerAttack(dt);

        return null;
    }

    public IUnitState OnFixedUpdate(float fdt)
    {
        return null;
    }
}
