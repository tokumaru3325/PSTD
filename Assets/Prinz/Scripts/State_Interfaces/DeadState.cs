using UnityEngine;

public class DeadState : IUnitState
{
    private readonly UnitModel _model;
    private readonly UnitPresenter _presenter;

    public DeadState(UnitModel model, UnitPresenter presenter)
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
