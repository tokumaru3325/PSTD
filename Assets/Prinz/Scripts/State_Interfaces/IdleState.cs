using UnityEngine;

public class IdleState : IUnitState
{
    private readonly UnitModel _model;
    private readonly UnitPresenter _presenter;
    private float _idleTime;

    public IdleState(UnitModel model, UnitPresenter presenter)
    {
        _model = model;
        _presenter = presenter;
    }

    public void OnEnter()
    { 
    //    Debug.LogWarning($"Enter IdleState {_model.PlayerSide}");
        _presenter.OnEnterState(this);
        _idleTime = 0f;
    }
    public void OnExit() { }

    public IUnitState OnUpdate(float dt)
    {
        if(_idleTime < 0.5f)
        {
            if (_presenter.IsValidTargetExist())
            {
                return new AttackState(_model, _presenter);
            }
            _idleTime += dt;
            return null;
        }


        if(_presenter.IsValidTargetExist() == false)
        {
            _presenter.Model.ClearTargets();
            return new MoveState(_model, _presenter);
        }

        //_idleTime = 0f;

        return null;
    }

    public IUnitState OnFixedUpdate(float fdt)
    {
        return null;
    }
}
