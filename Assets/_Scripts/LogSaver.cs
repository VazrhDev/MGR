using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using PlayFab;
using PlayFab.ClientModels;
using Newtonsoft.Json;

public class LogSaver : MonoBehaviour
{

    public static LogSaver instance;

    public bool canStartLogging = false;
    bool canSaveLogs = true;
    [SerializeField] int counter = 0;
    [SerializeField] int lastSavedLength = 0;
    [SerializeField] float logsUpdateFrequency = 2;
    [SerializeField] List<LogClass> logClass = new List<LogClass>();

    private void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        
    }

    private void OnEnable()
    {
        Application.logMessageReceived += Log;
    }

    private void OnDisable()
    {
        Application.logMessageReceived -= Log;
    }

    public void Log(string _logString, string _stackTrace, LogType _type)
    {
        LogClass temp = new LogClass();
        temp.log = _logString;
        temp.logType = _type.ToString();
        temp.logTime = DateTime.Now.ToString();
        temp.trace = _stackTrace;
        logClass.Add(temp);
        if (canStartLogging)
        {
            if (canSaveLogs || _type == LogType.Error)
            {
                lastSavedLength = logClass.Count;
                SaveLogsInDB();
                canSaveLogs = false;
                if (IsInvoking(nameof(ResetCanSaveLogs))) CancelInvoke(nameof(ResetCanSaveLogs));
                Invoke(nameof(ResetCanSaveLogs), logsUpdateFrequency);
            }
        }
    }

    void ResetCanSaveLogs()
    {
        canSaveLogs = true;
        if (logClass.Count > lastSavedLength)
        {
            lastSavedLength = logClass.Count;
            SaveLogsInDB();
        }
    }

    public void SaveLogsInDB()
    {
        counter++;
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                { "RecentLogs", JsonConvert.SerializeObject(logClass) }
            }
        };

        PlayFabClientAPI.UpdateUserData(request, OnDataSend, OnError);
    }

    private void OnDataSend(UpdateUserDataResult result)
    {
        //Master.ColoredLog("green", "Data Sent Successfully");
    }

    void OnError(PlayFabError error)
    {
        //Master.ColoredLog("red", error.GenerateErrorReport());
    }
}

[Serializable]
public class LogClass
{
    public string log;
    public string logType;
    public string logTime;
    public string trace;
}
