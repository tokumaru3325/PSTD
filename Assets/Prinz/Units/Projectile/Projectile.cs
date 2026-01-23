using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] protected float _speed;
    protected Vector3 _target;

    // Update is called once per frame
    virtual protected void Update()
    {
        Move();
    }

    protected void Move()
    {
        //投射物を敵の方向に移動させる
        float step = _speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, _target, step);

        //投射物の向きを敵の方向に向かせる
        Vector3 direction = _target - transform.position;
        transform.right = direction;
    }

    public void Initialize(Vector3 position, Vector3 enemyposition)
    {
        transform.position = position;
        _target = enemyposition;
    }
}
