using UnityEngine;

/// <summary>
/// パス構造体
/// </summary>
enum PathStructure : int
{
    // 一般道
    Road = 0,
    // 障害物がある道
    Obstacle = 1,
    // 通れない道
    Blocked = 999,
}