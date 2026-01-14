using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static bool IsGameFinished {  get; private set; }
    public int unitCount {  get; private set; } //serialNumber

    Coroutine Ending;

    public static event Action<bool, string> GameEnding;

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

    private void OnDisable()
    {
    //    Time.timeScale = 1.0f;
        M_Tower.OnPlayerDeath -= OnPlayerDeathNotify;
    }

    private void OnPlayerDeathNotify(string deadplayertag)
    {
        IsGameFinished = true;
     //   Time.timeScale = 0;
        GameEnding?.Invoke(IsGameFinished, deadplayertag);
        Ending = StartCoroutine(EndingScene());
    }

    public int OnUnitSpawn()
    {
        unitCount++;
        return unitCount;
    }

    private IEnumerator EndingScene()
    {
        //do something like wait before loading ending scene and stuff

        yield break;
    }
}
