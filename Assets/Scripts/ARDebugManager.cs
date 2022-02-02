using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ARDebugManager : MonoBehaviour
{
    public Text debugText;
    public static ARDebugManager Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }

    public void LogInfo(string logText)
    {
        Debug.Log(logText);
        debugText.text = logText;
    }
}
