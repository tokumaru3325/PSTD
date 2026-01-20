using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private float _speed;
    private Vector3 _target;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    public void Initialize(Vector3 position, Vector3 enemyposition)
    {
        _target = enemyposition;
    }

    private void Move()
    {
        float step = _speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, _target, step);
    }
}
