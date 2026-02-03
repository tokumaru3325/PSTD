using System;
using System.Collections;
using UnityEngine;

public class VictoryState : IUnitState
{
    private readonly UnitPresenter _me;

    public VictoryState(UnitPresenter presenter)
    {
        _me = presenter;
    }

    public void OnEnter()
    {
        _me.OnEnterState(this);

        _me.PerformVictoryAnimation();
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