using UnityEngine;

/// <summary>
/// パス構造体
/// </summary>
enum PathStructure : int
{
    // 一般道
    Road = 0,
    // プレイヤ位置(右向き)
    PlayerR = 1,
    // プレイヤ位置(左向き)
    PlayerL = 2,
    // 障害物がある道
    Obstacle = 20,
    // 通れない道
    Blocked = 999,
}