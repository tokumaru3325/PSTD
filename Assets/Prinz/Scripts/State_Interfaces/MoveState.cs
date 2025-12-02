using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class MoveState : IUnitState
{
    private readonly UnitModel _model;
    private readonly UnitPresenter _presenter;

    private C_MapManager _mapManager;
//    private int _currentRouteIndex = 0; //絶対ダメ
    private Vector3 target;

    public MoveState(UnitModel model, UnitPresenter presenter)
    {
        _model = model;
        _presenter = presenter;
        _mapManager = presenter.MapManager;
    }

    public void OnEnter() 
    {
        Debug.LogWarning("Enter MoveState");
        _presenter.OnEnterState();
        _presenter.View.PlayMove();
    }
    public void OnExit()
    {
        _presenter?.View?.StopMove();
        
    }

    public IUnitState OnUpdate(float dt)
    {

        return null;
    }

    public IUnitState OnFixedUpdate(float fdt)
    {
        DebugShowRoute();

        Move(_model.MoveSpeed, _model.MoveDirection, fdt);

        if (_model.HasTargetInRange() ) //ターゲットがいる場合
        {
            return new AttackState(_model, _presenter);
        }

        return null;
    }

    private void DebugShowRoute()
    {
        for (int index = 0; index < _model.Route.Count; index++)
        {
            Color txtColor = Color.blue;
            if (index == 0 || index == _model.Route.Count - 1)
                txtColor = Color.red;
            Vector3 pos = _mapManager.ConvertToUnityPos(_model.Route[index]);
            Debug.DrawLine(pos + Vector3.left, pos - Vector3.left, txtColor, 100f, false);
        }
    }

    public void Move(float movespeed, Vector3 direction, float dt)
    {
        //スプライトの向きを設定する
        if (direction.x < 0) // TODO : call it when change direction not everyframe
            _presenter.FaceLeft(_presenter.transform);
        else _presenter.FaceRight(_presenter.transform);


        //移動
        float step = movespeed * dt;

        GetDistanceToTarget();

        _presenter.transform.position = Vector3.MoveTowards(_presenter.transform.position, target, step);    

    //    _presenter.transform.Translate(direction * movespeed * step);

    //    _presenter.View.PlayMove();
    }

    private void GetDistanceToTarget()
    {
        int cri = _model.CurrentRouteIndex;
        if (cri < _model.Route.Count - 1)
        {
            //M_MapPosition currentMP = _mapManager.ConvertToMapPos(_presenter.transform.position);
            Vector3 NextTargetPos = _mapManager.ConvertToUnityPos(_model.Route[cri + 1]);
                float Distance = Vector3.Distance(_presenter.transform.position, NextTargetPos);
            //Debug.Log($"current route: {_presenter.transform.position}, target: {NextTargetPos}, distance: {Distance}");

            if (Distance <= 0.001f)
            {
                Debug.Log("next route");
                _model.SetCurrentRouteIndex(cri + 1);
            }

            Vector3 current = _mapManager.ConvertToUnityPos(_model.Route[cri]);
            target = current;
            if (cri + 1 < _model.Route.Count)
                target = _mapManager.ConvertToUnityPos(_model.Route[cri + 1]);
            _model.SetMoveDirection(target - current);
        }
    }
}