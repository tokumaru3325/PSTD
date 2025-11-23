using UnityEngine;

public class DeadState : IUnitState
{
    private UnitModel _model;

    public DeadState(UnitModel model)
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
