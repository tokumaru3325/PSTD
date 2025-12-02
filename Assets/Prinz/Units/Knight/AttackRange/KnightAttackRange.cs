using UnityEngine;

public class KnightAttackRange : MonoBehaviour
{
    public UnitView View { get; private set; }

    private void Awake()
    {
        View = GetComponentInParent<UnitView>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

/*    private void OnCollisionEnter(Collider2D other)
    {
        Debug.LogWarning("Collision ! in range script");
    }*/

/*    private void OnCollisionEnter2D(Collision2D other)
    {
        Debug.LogWarning("Collision ! in range script");
    }*/

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Unit")
        {
            View.OnEnterRange(other);
            Debug.LogWarning($"KNIGHTATTACKRANGE.cs : Collision with {other.gameObject.name}");
        }
    }
}
