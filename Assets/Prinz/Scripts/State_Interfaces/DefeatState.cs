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
        //   Debug.LogWarning($"Enter DeadState {_model.PlayerSide}");
        _me.OnEnterState(this);
        _me.PerformDefeatAnimation();
    }
    public void OnExit()
    {
        //    Debug.LogWarning("!!!!!!!!!!!!!!!!EXIT DeadState");
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