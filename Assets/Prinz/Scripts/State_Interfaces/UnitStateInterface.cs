using UnityEngine;

public interface IUnitState
{
    void OnEnter();
    void OnExit();
    IUnitState OnUpdate(float dt);

    IUnitState OnFixedUpdate(float fdt); //移動に使う
}
