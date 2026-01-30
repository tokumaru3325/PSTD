using UnityEngine;

public class LightningSpell : Projectile
{ 
    override protected void Update()
    {
        base.Update();

        //矢が目標に着いたら消す
        if (Vector3.Distance(transform.position, _target) < 0.1f)
        {
            ProjectileObjectPool.Instance.Release(this);
        }
    }
}
