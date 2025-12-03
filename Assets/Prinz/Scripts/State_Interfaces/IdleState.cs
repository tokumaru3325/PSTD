using UnityEngine;

public class IdleState : IUnitState
{
    private readonly UnitModel _model;
    private readonly UnitPresenter _presenter;
    private float _idleTime;

    public IdleState(UnitModel model, UnitPresenter presenter)
    {
        _model = model;
        _presenter = presenter;
    }

    public void OnEnter()
    { 
        Debug.LogWarning("Enter IdleState");
        _idleTime = 0f;
        _presenter.OnEnterState();
    }
    public void OnExit() { }

    public IUnitState OnUpdate(float dt)
    {
        if(_idleTime < 1.0f)
        {
            _idleTime += dt;
            return null;
        }

        if(_model.HasTargetInRange() == true)
        {
            return new AttackState(_model, _presenter);
        }

        if(_model.HasTargetInRange() == false)
        {
            return new MoveState(_model, _presenter);
        }

        //_idleTime = 0f;

        return null;
    }

    public IUnitState OnFixedUpdate(float fdt)
    {
        return null;
    }
}
