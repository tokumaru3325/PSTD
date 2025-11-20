using System;
using UnityEngine;

public class TestMonsterParent : MonoBehaviour
{
    public Sprite MonsterIcon;
    public int MonsterCost;
    public float MonsterCoolDown;

    public float ElapsedTime = 0.0f;

    public float speed = -50.0f;

    public void Initialize(Action onDisable)
    {
        //_onDisable = onDisable;
        //_elapsedTime = 0.0f;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ElapsedTime = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        ElapsedTime += Time.deltaTime;

        transform.position += new Vector3(0.0f, 5.0f, 0.0f) * Time.deltaTime;

        if(ElapsedTime >= 2.0f)
        {
            //_onDisable?.Invoke();
            gameObject.SetActive(false);
        }
    }
}
