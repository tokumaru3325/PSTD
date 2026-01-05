using UnityEngine;

public class DefeatState : IUnitState
{
    private readonly UnitModel _mymodel;
    private readonly UnitPresenter _me;

    public DefeatState(UnitModel model, UnitPresenter presenter)
    {
        _mymodel = model;
        _me = presenter;
    }

    public void OnEnter()
    {
        _me.OnEnterState(this);
        _me.PerformDefeatAnimation();
    }
    public void OnExit()
    {

    }

    public IUnitState OnUpdate(float dt)
    {
        return null;
    }

    public IUnitState OnFixedUpdate(float fdt)
    {
        return null;
    }
}