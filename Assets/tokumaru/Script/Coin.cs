using System;
using UnityEngine;

public class Coin : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    float currentY = 0.0f;
    float currentX = 0.0f;

    float life = 0.0f;

    public event Action<Coin> release;

    bool active = true;
    public void Create(float powerX, float powerY, float Life)
    {
        currentX = powerX;
        currentY = powerY;
        life = Life;
        active = true;
    }

    public void Initialize(Vector3 position)
    {
        this.gameObject.transform.position = position;
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!active) return;
        currentY -= 1000.0f * Time.deltaTime;




        Vector3 nowPos = this.transform.position;
        nowPos.y += currentY * Time.deltaTime;
        nowPos.x += currentX * Time.deltaTime;
        this.transform.position = nowPos;

        life -= 1.0f * Time.deltaTime;
        if (life <= 0.0f)
        {
            //Destroy(this.gameObject);
            release.Invoke(this);
            active = false;
        }

    }
}
