using System;
using System.Collections;
using UnityEngine;

public class VictoryState : IUnitState
{
    private readonly UnitModel _mymodel;
    private readonly UnitPresenter _me;

    public VictoryState(UnitModel model, UnitPresenter presenter)
    {
        _mymodel = model;
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