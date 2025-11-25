using UnityEngine;

public class MoveState : IUnitState
{
    private UnitModel _model;

    public MoveState(UnitModel model)
    {
        _model = model;
    }

    public void OnEnter() 
    {
        
    }
    public void OnExit() { }

    public IUnitState OnUpdate(float dt)
    {
        return null;
    }
}