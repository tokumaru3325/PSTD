using UnityEngine;

public class MinerPresenter : UnitPresenter
{
    protected override bool IsUntargetable(GameObject obj)
    {
        //敵の城でもUnitでもないモノを無視する
        return !obj.CompareTag("Obstacle");
    }

    protected override bool HandleTarget(Collider2D other)
    {
        if (other.gameObject.CompareTag("Obstacle"))
        {
            bool flowcontrol = HandleObstacleTarget(other);
            if (!flowcontrol)
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// 障害物の処理
    /// </summary>
    /// <param name="other">障害物</param>
    /// <returns></returns>
    private bool HandleObstacleTarget(Collider2D other)
    {
        if (!other.TryGetComponent<C_ObstacleStone>(out var obstacle)) { return false; }
        if (obstacle.IsDead()) { return false; }
        obstacle.OnDead += CompleteTask;
        Model.SetPlayerInRange(true);
        Model.BindTargetObstacle(obstacle);
        return true;
    }

    /// <summary>
    /// 障害物を破壊したら
    /// </summary>
    /// <param name="pos"></param>
    private void CompleteTask(Vector3 pos)
    {
        Model.Obstacle.OnDead -= CompleteTask;
        Release();
    }
}
