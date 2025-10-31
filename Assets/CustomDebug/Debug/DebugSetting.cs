using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Debug_Setting")]
public class DebugSetting : ScriptableObject
{
    public bool isFileSave = false;
    public string fileName = string.Empty;
    [SerializeField] private Color logColor, warningColor, errorColor, exceptionColor;

    public Color GetLogColor(LogType logType)
    {
        return logType switch
        {
            LogType.Log => logColor,
            LogType.Warning => warningColor,
            LogType.Error => errorColor,
            LogType.Exception => exceptionColor,
            _ => Color.white,
        };
    }
}
