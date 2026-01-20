using UnityEngine;

public class DefeatState : IUnitState
{
    private readonly UnitPresenter _me;

    public DefeatState(UnitPresenter presenter)
    {
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