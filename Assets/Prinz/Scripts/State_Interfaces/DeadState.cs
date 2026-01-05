using Unity.VisualScripting;
using UnityEngine;

public class DeadState : IUnitState
{
    private readonly UnitModel _mymodel;
    private readonly UnitPresenter _me;

    public DeadState(UnitModel model, UnitPresenter presenter)
    {
        _mymodel = model;
        _me = presenter;
    }

    public void OnEnter() 
    {
     //   Debug.LogWarning($"Enter DeadState {_model.PlayerSide}");
        _me.OnEnterState(this);
        _me.PerformDeath();
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
