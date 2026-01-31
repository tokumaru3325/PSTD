using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class MoveState : IUnitState
{
    private readonly UnitPresenter _me;

    private C_MapManager _mapManager;
    private Vector3 target;

    public MoveState(UnitPresenter presenter)
    {
        _me = presenter;
        _mapManager = presenter.MapManager;
    }

    public void OnEnter()
    {
        _me.Log($"Enter MoveState {_me.GetPlayerSide()}", LogType.Warning);
        _me.OnEnterState(this);
        _me.PlayMove(true);
    }
    public void OnExit()
    {
        //   Debug.LogWarning("Exit MoveState");
        _me.PlayMove(false);
    }

    public IUnitState OnUpdate(float dt)
    {
        return null;
    }

    public IUnitState OnFixedUpdate(float fdt)
    {
        Move(_me.GetTotalMoveSpeed(), _me.GetMoveDirection(), fdt);

        if (_me.IsValidTargetExist())
        {
            return new IdleState(_me);
        }

        return null;
    }

    //移動
    public void Move(float movespeed, Vector3 direction, float dt)
    {
        float step = movespeed * dt;

        GetDistanceToTarget();

        _me.transform.position = Vector3.MoveTowards(_me.transform.position, target, step);

        _me.PlayMove(true);
    }

    private void GetDistanceToTarget()
    {
        int cri = _me.GetCurrentRouteIndex();
        if (cri < _me.GetRouteCount() - 1)
        {
            //M_MapPosition currentMP = _mapManager.ConvertToMapPos(_presenter.transform.position);
            Vector3 NextTargetPos = _mapManager.ConvertToUnityPos(_me.GetRoutePosition(cri + 1));
            float Distance = Vector3.Distance(_me.transform.position, NextTargetPos);
            //Debug.Log($"current route: {_presenter.transform.position}, target: {NextTargetPos}, distance: {Distance}");

            if (Distance <= 0.001f)
            {
                //    Debug.Log("next route");
                _me.SetCurrentRouteIndex(cri + 1);
            }

            Vector3 current = _mapManager.ConvertToUnityPos(_me.GetRoutePosition(cri));
            target = current;
            if (cri + 1 < _me.GetRouteCount())
                target = _mapManager.ConvertToUnityPos(_me.GetRoutePosition(cri + 1));
            _me.SetMoveDirection(target - current);
        }
    }
}