using UnityEngine;

public interface IUnitState
{
    void OnEnter();
    void OnExit();
    IUnitState OnUpdate(float dt);
}
