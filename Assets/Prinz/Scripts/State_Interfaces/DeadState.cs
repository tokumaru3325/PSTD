using Unity.VisualScripting;
using UnityEngine;

public class DeadState : IUnitState
{
    private readonly UnitModel _model;
    private readonly UnitPresenter _presenter;

    public DeadState(UnitModel model, UnitPresenter presenter)
    {
        _model = model;
        _presenter = presenter;
    }

    public void OnEnter() 
    {
     //   Debug.LogWarning($"Enter DeadState {_model.PlayerSide}");
        _presenter.OnEnterState();
        _presenter.PerformDeath();
     
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
