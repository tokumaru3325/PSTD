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
    private InputAction _debugAttackRangeDisplay;
    private InputAction _debugPathDisplay;
    private bool _isAttackRangeVisible = false;
    private bool _isPathVisible = false;

    [SerializeField]
    private C_MapManager _mapManager;

    public List<GameObject> allAttackRanges = new List<GameObject>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _debugAttackRangeDisplay = InputSystem.actions.FindAction("ToggleAttackRangeVisibility");
        _debugPathDisplay = InputSystem.actions.FindAction("ToggleDisplayPath");

        GetAllAttackRanges();
    }

    // Update is called once per frame
    void Update()
    {
        if(_debugAttackRangeDisplay.WasPressedThisFrame())
        {
            ToggleAttackRangeVisibility();
        }

        if( _debugPathDisplay.WasPressedThisFrame())
        {
            ToggleDisplayPath();
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

    public void OnUnitSpawn(UnitPresenter owner)
    {
        Debug.LogWarning($"OnUnitSpawn called in debug manager with visibility = {_isAttackRangeVisible}");
        Transform AttackRange;
        AttackRange = owner.transform.Find("AttackRange");
        if (AttackRange == null)
        {
            Debug.LogError("OnUnitSpawn did not find any AttackRange");
            return;
        }
        allAttackRanges.Add(AttackRange.gameObject);
        ApplyVisibility();
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
        _isAttackRangeVisible = !_isAttackRangeVisible;
        GetAllAttackRanges();
        ApplyVisibility();
    }

    private void ApplyVisibility()
    {
        foreach (GameObject AttackRange in allAttackRanges)
        {
            if (AttackRange != enabled)
            {
                continue;
            }
            EnableAttackRangeSprite(AttackRange);
        }
    }

    private void EnableAttackRangeSprite(GameObject AttackRange)
    {
        SpriteRenderer sr = AttackRange.GetComponent<SpriteRenderer>();
        sr.enabled = _isAttackRangeVisible;
    }

    private void ToggleDisplayPath()
    {
        _isPathVisible = !_isPathVisible;
        _mapManager.SetPathVisibility(_isPathVisible);
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
