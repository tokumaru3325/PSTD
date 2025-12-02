using UnityEngine;

public class IdleState : IUnitState
{
    private readonly UnitModel _model;
    private readonly UnitPresenter _presenter;

    public IdleState(UnitModel model, UnitPresenter presenter)
    {
        _model = model;
        _presenter = presenter;
    }

    public void OnEnter() 
    {
        Debug.LogWarning("Enter IdleState");
    }
    public void OnExit() { }

    public IUnitState OnUpdate(float dt)
    {
        if(_model.HasTargetInRange() == true)
        {
            return new AttackState(_model, _presenter);
        }

        if(_model.HasTargetInRange() == false)
        {
            return new MoveState(_model, _presenter);
        }

        return null;
    }
}
