using System;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditorInternal;
using UnityEngine;
using System.Reflection;

public class CustomDebugLocationOpener
{
    [OnOpenAsset(0)]
    static bool OnOpenAsset(int instance, int line)
    {
        string name = EditorUtility.InstanceIDToObject(instance).name;

        // Debug.cs 파일이 열릴 때만 처리
        if (name != "Debug") return false;

        string stack_trace = GetStackTrace();
        if (!string.IsNullOrEmpty(stack_trace))
        {
            // 스택 트레이스에서 모든 매칭 찾기
            MatchCollection matches = Regex.Matches(stack_trace, @"\(at (.+?):(\d+)\)");

            foreach (Match match in matches)
            {
                if (match.Success)
                {
                    string path = match.Groups[1].Value.Trim();
                    string lineStr = match.Groups[2].Value;

                    // Debug.cs가 아닌 첫 번째 파일을 찾음
                    if (!path.EndsWith("Debug.cs"))
                    {
                        int targetLine = Convert.ToInt32(lineStr);
                        string fullpath = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("Assets"));
                        fullpath = fullpath + path;

                        InternalEditorUtility.OpenFileAtLineExternal(fullpath, targetLine);
                        return true;
                    }
                }
            }
        }
        return false;
    }

    static string GetStackTrace()
    {
        var assembly_unity_editor = Assembly.GetAssembly(typeof(EditorWindow));
        if (assembly_unity_editor == null) return null;

        var type_console_window = assembly_unity_editor.GetType("UnityEditor.ConsoleWindow");
        if (type_console_window == null) return null;

        var field_console_window = type_console_window.GetField("ms_ConsoleWindow", BindingFlags.Static | BindingFlags.NonPublic);
        if (field_console_window == null) return null;

        var instance_console_window = field_console_window.GetValue(null);
        if (instance_console_window == null) return null;

        if ((object)EditorWindow.focusedWindow == instance_console_window)
        {
            var field_active_text = type_console_window.GetField("m_ActiveText", BindingFlags.Instance | BindingFlags.NonPublic);
            if (field_active_text == null) return null;

            string value_active_text = field_active_text.GetValue(instance_console_window).ToString();
            return value_active_text;
        }
        return null;
    }
}