using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class MoveState : IUnitState
{
    private readonly UnitModel _model;
    private readonly UnitPresenter _presenter;

    public MoveState(UnitModel model, UnitPresenter presenter)
    {
        _model = model;
        _presenter = presenter;
    }

    public void OnEnter() 
    {
        _presenter?.View?.PlayMove();
    }
    public void OnExit()
    {
        _presenter?.View?.StopMove();
    }

    public IUnitState OnUpdate(float dt)
    {
        Move(_model.MoveSpeed, _model.MoveDirection, dt);

//        if (_model.HasEnemyInRange())
//        {
//            return new AttackState(_model, _presenter);
//        }

        return null;
    }

    public void Move(float movespeed, Vector3 direction, float dt)
    {
        if (direction.x < 0) // TODO : call it when change direction not everyframe
            _presenter.FaceLeft(_presenter.transform);
        else _presenter.FaceRight(_presenter.transform);

        _presenter.transform.Translate(direction * movespeed * dt);

        _presenter.View?.PlayMove();
    }
}