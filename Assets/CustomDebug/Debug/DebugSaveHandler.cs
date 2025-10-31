using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class DebugSaveHandler
{
    private readonly string path;
    private readonly StringBuilder sb;
    private readonly bool isFileSave;

    public DebugSaveHandler(string path, bool isFileSave)
    {
        this.path = path;
        this.isFileSave = isFileSave;
        sb = new StringBuilder();
    }

    public void Append(string message)
    {
        string timestampedMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] {message}";
        sb.AppendLine(timestampedMessage);
    }

    public void Save()
    {
        if (!isFileSave)
            return;

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        string fileName = $"DebugLog_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
        string fullPath = Path.Combine(path, fileName);

        try
        {
            File.WriteAllText(fullPath, sb.ToString(), Encoding.UTF8);

            sb.Clear();

            Debug.Log($"Log Save Complete: {fullPath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Log Save Error: {e.Message}");
        }

#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif
    }    
}
