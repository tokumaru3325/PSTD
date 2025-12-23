using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class DebugManager : MonoBehaviour 
{
    #region Singleton Implementation

    private static readonly Lazy<DebugManager> instance =
        new Lazy<DebugManager>(() => new DebugManager());

    public static DebugManager Instance => instance.Value;

    private DebugManager()
    {
    }

    #endregion

    [Serializable]
    public struct DebugLogSettings
    {
        [SerializeField] public bool debugLogEnabled;
        [SerializeField] public bool debugLogWarningEnabled;
        [SerializeField] public bool debugLogErrorEnabled;
        [SerializeField] public bool RuntimeLogEnabled;

    }

    public TextMeshProUGUI debugText;
    private string logBuffer = "";
    private int maxLogLines = 20; // Adjust as needed
    private InputAction _debugAttackRangeDisplay;
    private InputAction _debugPathDisplay;
    private bool _isAttackRangeVisible = false;
    private bool _isPathVisible = false;
    public DebugLogSettings _debugLogVisibility;

    [SerializeField]
    private C_MapManager _mapManager;

    public List<GameObject> allAttackRanges = new List<GameObject>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _debugAttackRangeDisplay = InputSystem.actions.FindAction("ToggleAttackRangeVisibility");
        _debugPathDisplay = InputSystem.actions.FindAction("ToggleDisplayPath");
        _debugLogVisibility = new  DebugLogSettings();

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
        Log($"OnUnitSpawn called in debug manager with visibility = {_isAttackRangeVisible}", LogType.Warning);
        Transform AttackRange;
        AttackRange = owner.transform.Find("AttackRange");
        if (AttackRange == null)
        {
            Log("OnUnitSpawn did not find any AttackRange", LogType.Error);
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
        Log("GameObjects with 'AttackRange' tag:", LogType.Log);
        foreach (GameObject AttackRange in AttackRanges)
        {
            cnt++;
            allAttackRanges.Add(AttackRange);
            Log($"{AttackRange.name} number: {cnt}", LogType.Log);
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

    void HandleRuntimeLog(string logString, LogType type)
    {
        if (false == _debugLogVisibility.RuntimeLogEnabled) return;

        LogMessage(logString);
    }

    public void Log(string logString, LogType type)
    {
        if (_debugLogVisibility.RuntimeLogEnabled)
        {
            HandleRuntimeLog(logString, type);
        }

        HandleEditorLog(logString, type);
    }

    void HandleEditorLog(string logString, LogType type)
    {
        if (type == LogType.Log)
        {
            Debug.Log(logString);
        }
        if(type == LogType.Warning)
        {
            Debug.LogWarning(logString);
        }
        if(type == LogType.Error)
        {
            Debug.LogError(logString);
        }
    }

    private void LogMessage(string message)
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
