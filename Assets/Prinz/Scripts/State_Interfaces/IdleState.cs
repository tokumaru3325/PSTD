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

    public void OnEnter() { }
    public void OnExit() { }

    public IUnitState OnUpdate(float dt)
    {
        return null;
    }
}
