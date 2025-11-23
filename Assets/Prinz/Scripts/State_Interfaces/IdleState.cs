using UnityEngine;

public class IdleState : IUnitState
{
    private UnitModel _model;

    public IdleState(UnitModel model)
    {
        _model = model;
    }

    public void OnEnter() { }
    public void OnExit() { }

    public IUnitState OnUpdate(float dt)
    {
        return null;
    }
}
