using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class MoveState : IUnitState
{
    private readonly UnitModel _mymodel;
    private readonly UnitPresenter _me;

    private C_MapManager _mapManager;
//    private int _currentRouteIndex = 0; //絶対ダメ
    private Vector3 target;

    public MoveState(UnitModel model, UnitPresenter presenter)
    {
        _mymodel = model;
        _me = presenter;
        _mapManager = presenter.MapManager;
    }

    public void OnEnter() 
    {
        _me.Log($"Enter MoveState {_mymodel.PlayerSide}", LogType.Warning);
        _me.OnEnterState(this);
        _me.View.PlayMove(true);
    }
    public void OnExit()
    {
     //   Debug.LogWarning("Exit MoveState");
        _me?.View?.PlayMove(false);
    }

    public IUnitState OnUpdate(float dt)
    {

        return null;
    }

    public IUnitState OnFixedUpdate(float fdt)
    {
        DebugShowRoute();

        Move(_mymodel.TotalMoveSpeed, _mymodel.MoveDirection, fdt);

/*        if (_mymodel.HasTargetInRange() ) //ターゲットがいる場合
        {
            var target = _mymodel.GetPrimaryTarget();
            if (_me.IsSameTeamAs(target))
            {
                return new HealState(_mymodel, _me, target);
            }
            else
            {
                return new AttackState(_mymodel, _me, target);
            }
        }*/

        if(_me.IsValidTargetExist())
        {
            return new IdleState(_mymodel, _me);
        }

        return null;
    }

    private void DebugShowRoute()
    {
        for (int index = 0; index < _mymodel.Route.Count; index++)
        {
            Color txtColor = Color.blue;
            if (index == 0 || index == _mymodel.Route.Count - 1)
                txtColor = Color.red;
            Vector3 pos = _mapManager.ConvertToUnityPos(_mymodel.Route[index]);
            Debug.DrawLine(pos + Vector3.left, pos - Vector3.left, txtColor, 100f, false);
        }
    }

    public void Move(float movespeed, Vector3 direction, float dt)
    {
        //移動
        float step = movespeed * dt;

        GetDistanceToTarget();

        _me.transform.position = Vector3.MoveTowards(_me.transform.position, target, step);    

        _me.View.PlayMove(true);
    }

    private void GetDistanceToTarget()
    {
        int cri = _mymodel.CurrentRouteIndex;
        if (cri < _mymodel.Route.Count - 1)
        {
            //M_MapPosition currentMP = _mapManager.ConvertToMapPos(_presenter.transform.position);
            Vector3 NextTargetPos = _mapManager.ConvertToUnityPos(_mymodel.Route[cri + 1]);
                float Distance = Vector3.Distance(_me.transform.position, NextTargetPos);
            //Debug.Log($"current route: {_presenter.transform.position}, target: {NextTargetPos}, distance: {Distance}");

            if (Distance <= 0.001f)
            {
            //    Debug.Log("next route");
                _mymodel.SetCurrentRouteIndex(cri + 1);
            }

            Vector3 current = _mapManager.ConvertToUnityPos(_mymodel.Route[cri]);
            target = current;
            if (cri + 1 < _mymodel.Route.Count)
                target = _mapManager.ConvertToUnityPos(_mymodel.Route[cri + 1]);
            _mymodel.SetMoveDirection(target - current);
        }
    }
}