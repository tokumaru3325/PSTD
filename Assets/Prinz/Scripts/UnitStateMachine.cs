using UnityEngine;

public class UnitStateMachine
{
    public IUnitState Current { get; private set; }

    public UnitStateMachine() { }

    public void Initialize(IUnitState startState)
    {
        Debug.Log("Initializing State Machine");
        Current = startState;
        Current?.OnEnter();
    }

    public void Tick(float deltaTime) //これで更新の頻度が調整できる（処理が重くなる場合）
    {
        var next = Current?.OnUpdate(deltaTime);
        if (next != null)
        {
            Current.OnExit();
            Current = null;
            Current = next;
            Current.OnEnter();
        }
    }

    public void FixedTick(float fixeddeltaTime)
    {
        var next = Current?.OnFixedUpdate(fixeddeltaTime);
        if (next != null)
        {
            Current.OnExit();
            Current = null;
            Current = next;
            Current.OnEnter();
        }
    }

    public void TrySetState(IUnitState newState)
    {
        if (Current == newState) return;

        Debug.LogWarning("TrySetState() called");
        Current.OnExit();
        Current = null;
        Current = newState;
        Current.OnEnter();
    
    }
}
