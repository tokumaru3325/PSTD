using UnityEngine;

public class Player : MonoBehaviour
{
    public float Money;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Money = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        Money += Time.deltaTime * 3;
    }
}
