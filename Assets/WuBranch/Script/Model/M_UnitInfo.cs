using System;
using UnityEngine;

[Serializable]
public class UnitInfo
{
    [Tooltip("ユニットID")]
    public UnitID ID;

    [Tooltip("ユニットプレハブ")]
    public GameObject Prefab;

    [Tooltip("ユニットデータ")]
    public UnitData Data;
}
