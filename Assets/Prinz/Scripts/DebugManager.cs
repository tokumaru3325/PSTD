using Newtonsoft.Json.Bson;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class DebugManager : MonoBehaviour
{

    public TextMeshProUGUI debugText;
    private string logBuffer = "";
    private int maxLogLines = 20; // Adjust as needed
    private InputAction _debugAction;
    private bool _isAttackRangeVisible = true;

    public List<GameObject> allAttackRanges = new List<GameObject>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _debugAction = InputSystem.actions.FindAction("ToggleAttackRangeVisibility");

        GetAllAttackRanges();
    }

    // Update is called once per frame
    void Update()
    {
        if(_debugAction.WasPressedThisFrame())
        {
            ToggleAttackRangeVisibility();
        }
    }

    private void OnEnable()
    {
    //    Application.logMessageReceived += HandleLog;
    }

    private void OnDisable()
    {
        allAttackRanges.Clear();
     //   Application.logMessageReceived -= HandleLog;
    }

    private void GetAllAttackRanges()
    {
        allAttackRanges.Clear();

        GameObject[] AttackRanges = GameObject.FindGameObjectsWithTag("AttackRange");
        int cnt = 0;
        Debug.Log("GameObjects with 'AttackRange' tag:");
        foreach (GameObject AttackRange in AttackRanges)
        {
            cnt++;
            allAttackRanges.Add(AttackRange);
            Debug.Log($"{AttackRange.name} number: {cnt}");
        }
    }

    private void ToggleAttackRangeVisibility()
    {
        GetAllAttackRanges();
        foreach (GameObject AttackRange in allAttackRanges)
        {
            if (AttackRange == enabled)
            {
                SpriteRenderer sr = AttackRange.GetComponent<SpriteRenderer>();
                sr.enabled = _isAttackRangeVisible;
            }
        }
        _isAttackRangeVisible = !_isAttackRangeVisible;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        // Optionally filter by LogType
        if (type == LogType.Error || type == LogType.Exception || type == LogType.Warning || type == LogType.Log)
        {
            LogMessage(logString);
        }
    }

    public void LogMessage(string message)
    {
        logBuffer += message + "\n";
        // Simple line limit
        string[] lines = logBuffer.Split('\n');
        if (lines.Length > maxLogLines)
        {
            System.Array.Copy(lines, lines.Length - maxLogLines, lines, 0, maxLogLines);
            logBuffer = string.Join("\n", lines);
        }
        debugText.text = logBuffer;
    }
}
