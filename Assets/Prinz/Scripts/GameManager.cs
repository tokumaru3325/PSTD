using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool IsGameFinished {  get; private set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        IsGameFinished = false;
        M_Tower.OnPlayerDeath += OnPlayerDeathNotify;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnPlayerDeathNotify(string deadplayertag)
    {


        IsGameFinished = true;
    }
}
