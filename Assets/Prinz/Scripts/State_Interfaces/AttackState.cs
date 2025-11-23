using UnityEngine;

public class AttackState : IUnitState
{
    private UnitModel _model;

    public AttackState(UnitModel model)
    {
        _model = model;
    }

    public void OnEnter() { }
    public void OnExit() { }

    public IUnitState OnUpdate(float dt)
    {
    //    if (_model.TargetInRange)
     //       return new AttackState(_model);

        return null;
    }
}
