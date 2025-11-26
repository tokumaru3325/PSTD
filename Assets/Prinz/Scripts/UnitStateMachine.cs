using UnityEngine;

public class UnitStateMachine
{
    private IUnitState _current;

    public UnitStateMachine() { }

    public void Initialize(IUnitState startState)
    {
        _current = startState;
        _current?.OnEnter();
    }

    public void Tick(float deltaTime) //これで更新の頻度が調整できる（処理が重くなる場合）
    {
        var next = _current?.OnUpdate(deltaTime);
        if (next != null)
        {
            _current.OnExit();
            _current = next;
            _current.OnEnter();
        }
    }
}
