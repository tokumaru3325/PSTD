using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
using Unity.Properties;

public class DebugManager : MonoBehaviour 
{
/*    #region Singleton Implementation

    private static readonly Lazy<DebugManager> instance =
        new Lazy<DebugManager>(() => new DebugManager());

    public static DebugManager Instance => instance.Value;

    private DebugManager()
    {
    }

    #endregion*/

    [Serializable]
    public class DebugLogSettings
    {
        [SerializeField] public bool debugLogEnabled;
        [SerializeField] public bool debugLogWarningEnabled;
        [SerializeField] public bool debugLogErrorEnabled;
        [SerializeField] public bool RuntimeLogEnabled;
        [SerializeField] public bool debugLogColliders;
    }
    [SerializeField]
    public DebugLogSettings _debugLogVisibility;

    public TextMeshProUGUI debugText;
//    private string logBuffer = "";
//    private int maxLogLines = 20; // Adjust as needed
    private InputAction _debugAttackRangeDisplay;
    private InputAction _debugPathDisplay;
    private InputAction _debugKillPlayer2;
    private bool _isAttackRangeVisible = false;
    private bool _isPathVisible = false;

    [SerializeField]
    private C_MapManager _mapManager;

    [SerializeField]
    private C_PlayerTowerController _PlayerTowerController;

    public List<GameObject> allAttackRanges = new List<GameObject>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _debugAttackRangeDisplay = InputSystem.actions.FindAction("ToggleAttackRangeVisibility");
        _debugPathDisplay = InputSystem.actions.FindAction("ToggleDisplayPath");
        _debugKillPlayer2 = InputSystem.actions.FindAction("Spacebar");

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

        if(_debugKillPlayer2.WasPressedThisFrame())
        {
            KillPlayer2();
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

    private void KillPlayer2()
    {
        GameObject playerobject = GameObject.FindGameObjectWithTag("Player2");
        C_PlayerTowerController player2 = playerobject.GetComponent<C_PlayerTowerController>();

        player2.DecreaseHP(9999);
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

    /*    void HandleRuntimeLog(string logString, LogType type)
        {
            if (false == _debugLogVisibility.RuntimeLogEnabled) return;

            LogMessage(logString);
        }*/

    /// <summary>
    /// この関数を使って、DebugLogの表示をDebugManagerのInspector上で設定することが出来る
    /// 使い方：Log("「○○メッセージ」", 「LogType.Log、LogType.Warning、LogType.Errorのいずれを選ぶ」);
    /// </summary>
    /// <param name="message"></param>
    /// <param name="type"></param>
    public void Log(string logString, LogType type)
    {
        if (_debugLogVisibility.RuntimeLogEnabled)
        {
          //  HandleRuntimeLog(logString, type); //未実装
        }

        HandleEditorLog(logString, type);
    }

    void HandleEditorLog(string logString, LogType type)
    {
        if (type == LogType.Log && _debugLogVisibility.debugLogEnabled)
        {
            Debug.Log(logString);
        }
        if(type == LogType.Warning && _debugLogVisibility.debugLogWarningEnabled)
        {
            Debug.LogWarning(logString);
        }
        if(type == LogType.Error && _debugLogVisibility.debugLogErrorEnabled)
        {
            Debug.LogError(logString);
        }
    }

/*    private void LogMessage(string message)
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
    }*/
}
