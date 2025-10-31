using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;
using System.Diagnostics;
using UDebug = UnityEngine.Debug;

public static class Debug
{
    static DebugSetting _debugSetting;
    static DebugSetting debugSetting 
    { 
        get 
        { 
            if(_debugSetting == null) 
                _debugSetting = Resources.Load<DebugSetting>("DebugSetting"); 
            return _debugSetting; 
        }
        set => _debugSetting = value;
    }

    static DebugSaveHandler saveHandler;
    const string closeColorTag = "</color>";

#if UNITY_EDITOR
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    static void Init()
    {
        debugSetting = Resources.Load<DebugSetting>("DebugSetting");
        saveHandler = new DebugSaveHandler($"{Application.streamingAssetsPath}/{debugSetting.fileName}", debugSetting.isFileSave);        
    }
#endif

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void CreateSaver()
    {
        Object.Instantiate(Resources.Load<GameObject>("DebugSaver"));
    }

    #region Custom Log Methods with Context
    [HideInCallstack]
    public static void Log(object message,
        [CallerMemberName] string member = "",
        [CallerFilePath] string file = "",
        [CallerLineNumber] int line = 0) => ShowLog(message, LogType.Log, member, file, line);

    [HideInCallstack]
    public static void LogWarning(object message,
        [CallerMemberName] string member = "",
        [CallerFilePath] string file = "",
        [CallerLineNumber] int line = 0) => ShowLog(message, LogType.Warning, member, file, line);

    [HideInCallstack]
    public static void LogError(object message,
        [CallerMemberName] string member = "",
        [CallerFilePath] string file = "",
        [CallerLineNumber] int line = 0) => ShowLog(message, LogType.Error, member, file, line);

    [HideInCallstack]
    public static void LogException(System.Exception exception,
        [CallerMemberName] string member = "",
        [CallerFilePath] string file = "",
        [CallerLineNumber] int line = 0)
    {
        string fileName = Path.GetFileName(file);
        UnityEngine.Color logColor = debugSetting.GetLogColor(LogType.Exception);
        string colorTag = $"<color=#{ColorUtility.ToHtmlStringRGB(logColor)}>";
        string contextPrefix = $"[{fileName}:{line} - {member}]";
        string formattedMessage = $"{colorTag}{contextPrefix} {exception}{closeColorTag}";

        UDebug.LogError(formattedMessage);

        if (debugSetting.isFileSave && saveHandler != null)
            saveHandler.Append($"[Exception]\t{contextPrefix} {exception}");
    }

    [HideInCallstack]
    public static void LogFormat(string format, params object[] args) => LogFormat(LogType.Log, null, format, args);

    [HideInCallstack]
    public static void LogFormat(Object context, string format, params object[] args) => LogFormat(LogType.Log, context, format, args);

    [HideInCallstack]
    public static void LogWarningFormat(string format, params object[] args) => LogFormat(LogType.Warning, null, format, args);

    [HideInCallstack]
    public static void LogWarningFormat(Object context, string format, params object[] args) => LogFormat(LogType.Warning, context, format, args);

    [HideInCallstack]
    public static void LogErrorFormat(string format, params object[] args) => LogFormat(LogType.Error, null, format, args);

    [HideInCallstack]
    public static void LogErrorFormat(Object context, string format, params object[] args) => LogFormat(LogType.Error, context, format, args);

    [HideInCallstack]
    static void LogFormat(LogType logType, Object context,
        string format, object[] args,
        [CallerMemberName] string member = "",
        [CallerFilePath] string file = "",
        [CallerLineNumber] int line = 0)
    {
        string message = string.Format(format, args);
        string fileName = Path.GetFileName(file);
        UnityEngine.Color logColor = debugSetting.GetLogColor(logType);
        string colorTag = $"<color=#{ColorUtility.ToHtmlStringRGB(logColor)}>";
        string contextPrefix = $"[{fileName}:{line} - {member}]";
        string formattedMessage = $"{colorTag}{contextPrefix} {message}{closeColorTag}";

        switch (logType)
        {
            case LogType.Log:
                if (context != null) UDebug.Log(formattedMessage, context);
                else UDebug.Log(formattedMessage);
                break;
            case LogType.Warning:
                if (context != null) UDebug.LogWarning(formattedMessage, context);
                else UDebug.LogWarning(formattedMessage);
                break;
            case LogType.Error:
                if (context != null) UDebug.LogError(formattedMessage, context);
                else UDebug.LogError(formattedMessage);
                break;
        }

        if (debugSetting.isFileSave)
            saveHandler.Append($"[{logType}]\t{contextPrefix} {message}");
    }

    [HideInCallstack]
    static void ShowLog(object message, LogType logType, string member, string file, int line)
    {
        string fileName = Path.GetFileName(file);
        UnityEngine.Color logColor = debugSetting.GetLogColor(logType);
        string colorTag = $"<color=#{ColorUtility.ToHtmlStringRGB(logColor)}>";
        string contextPrefix = $"[{fileName}:{line} - {member}]";
        string formattedMessage = $"{colorTag}{contextPrefix} {message}{closeColorTag}";

        switch (logType)
        {
            case LogType.Log:
                UDebug.Log(formattedMessage);
                break;
            case LogType.Warning:
                UDebug.LogWarning(formattedMessage);
                break;
            case LogType.Error:
                UDebug.LogError(formattedMessage);
                break;
        }

        if (debugSetting.isFileSave)
            saveHandler.Append($"[{logType}]\t{contextPrefix} {message}");
    }
    #endregion

    #region Assert with Context
    [HideInCallstack]
    public static void Assert(bool condition,
        [CallerMemberName] string member = "",
        [CallerFilePath] string file = "",
        [CallerLineNumber] int line = 0)
    {
        if (!condition)
        {
            string fileName = Path.GetFileName(file);
            string contextPrefix = $"[{fileName}:{line} - {member}]";
            UnityEngine.Color logColor = debugSetting.GetLogColor(LogType.Assert);
            string colorTag = $"<color=#{ColorUtility.ToHtmlStringRGB(logColor)}>";
            string formattedMessage = $"{colorTag}{contextPrefix} Assertion failed{closeColorTag}";

            UDebug.LogError(formattedMessage);
            if (debugSetting.isFileSave)
                saveHandler.Append($"[Assert]\t{contextPrefix} Assertion failed");
        }
    }

    [HideInCallstack]
    public static void Assert(bool condition, object message,
        [CallerMemberName] string member = "",
        [CallerFilePath] string file = "",
        [CallerLineNumber] int line = 0)
    {
        if (!condition)
        {
            string fileName = Path.GetFileName(file);
            string contextPrefix = $"[{fileName}:{line} - {member}]";
            UnityEngine.Color logColor = debugSetting.GetLogColor(LogType.Assert);
            string colorTag = $"<color=#{ColorUtility.ToHtmlStringRGB(logColor)}>";
            string formattedMessage = $"{colorTag}{contextPrefix} {message}{closeColorTag}";

            UDebug.LogError(formattedMessage);
            if (debugSetting.isFileSave)
                saveHandler.Append($"[Assert]\t{contextPrefix} {message}");
        }
    }

    [HideInCallstack]
    public static void Assert(bool condition, object message, Object context,
        [CallerMemberName] string member = "",
        [CallerFilePath] string file = "",
        [CallerLineNumber] int line = 0)
    {
        if (!condition)
        {
            string fileName = Path.GetFileName(file);
            string contextPrefix = $"[{fileName}:{line} - {member}]";
            UnityEngine.Color logColor = debugSetting.GetLogColor(LogType.Assert);
            string colorTag = $"<color=#{ColorUtility.ToHtmlStringRGB(logColor)}>";
            string formattedMessage = $"{colorTag}{contextPrefix} {message}{closeColorTag}";

            UDebug.LogError(formattedMessage, context);
            if (debugSetting.isFileSave)
                saveHandler.Append($"[Assert]\t{contextPrefix} {message}");
        }
    }

    [HideInCallstack]
    public static void AssertFormat(bool condition, string format, params object[] args) => AssertFormat(condition, null, format, args);

    [HideInCallstack]
    public static void AssertFormat(bool condition, Object context, string format, params object[] args) => AssertFormatInternal(condition, context, format, args);

    [HideInCallstack]
    static void AssertFormatInternal(bool condition, Object context, string format, object[] args,
        [CallerMemberName] string member = "",
        [CallerFilePath] string file = "",
        [CallerLineNumber] int line = 0)
    {
        if (!condition)
        {
            string message = string.Format(format, args);
            string fileName = Path.GetFileName(file);
            string contextPrefix = $"[{fileName}:{line} - {member}]";
            UnityEngine.Color logColor = debugSetting.GetLogColor(LogType.Assert);
            string colorTag = $"<color=#{ColorUtility.ToHtmlStringRGB(logColor)}>";
            string formattedMessage = $"{colorTag}{contextPrefix} {message}{closeColorTag}";

            if (context != null) UDebug.LogError(formattedMessage, context);
            else UDebug.LogError(formattedMessage);

            if (debugSetting.isFileSave)
                saveHandler.Append($"[Assert]\t{contextPrefix} {message}");
        }
    }
    #endregion

    #region Unity Debug
    public static void DrawLine(Vector3 start, Vector3 end) => UDebug.DrawLine(start, end);
    public static void DrawLine(Vector3 start, Vector3 end, UnityEngine.Color color) => UDebug.DrawLine(start, end, color);
    public static void DrawLine(Vector3 start, Vector3 end, UnityEngine.Color color, float duration) => UDebug.DrawLine(start, end, color, duration);
    public static void DrawLine(Vector3 start, Vector3 end, UnityEngine.Color color, float duration, bool depthTest) => UDebug.DrawLine(start, end, color, duration, depthTest);

    public static void DrawRay(Vector3 start, Vector3 dir) => UDebug.DrawRay(start, dir);
    public static void DrawRay(Vector3 start, Vector3 dir, UnityEngine.Color color) => UDebug.DrawRay(start, dir, color);
    public static void DrawRay(Vector3 start, Vector3 dir, UnityEngine.Color color, float duration) => UDebug.DrawRay(start, dir, color, duration);
    public static void DrawRay(Vector3 start, Vector3 dir, UnityEngine.Color color, float duration, bool depthTest) => UDebug.DrawRay(start, dir, color, duration, depthTest);

    // Break
    public static void Break() => UDebug.Break();
    public static void DebugBreak() => UDebug.DebugBreak();

    // Properties
    public static ILogger logger => UDebug.unityLogger;
    public static ILogger unityLogger => UDebug.unityLogger;
    public static bool isDebugBuild => UDebug.isDebugBuild;
    public static bool developerConsoleVisible
    {
        get => UDebug.developerConsoleVisible;
        set => UDebug.developerConsoleVisible = value;
    }
    #endregion

    public static void Save() => saveHandler?.Save();
}