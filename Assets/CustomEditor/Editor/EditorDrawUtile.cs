using UnityEditor;
using UnityEngine;
using System;

namespace HSD_Editor
{
    public static class EditorDrawUtile
    {
        public static void DrawIconInRectCenter(Rect rect, Sprite icon, float iconRatio)
        {
            if (icon == null) return;
            float iconWidth = rect.width * iconRatio;
            float iconHeight = rect.height * iconRatio;
            float iconX = rect.x + (rect.width - iconWidth) / 2;
            float iconY = rect.y + (rect.height - iconHeight) / 2;
            Rect iconRect = new Rect(iconX, iconY, iconWidth, iconHeight);
            GUI.DrawTexture(iconRect, icon.texture, ScaleMode.ScaleToFit);
        }

        public static void DrawLabel(Rect rect, string text, GUIStyle style, TextAnchor textAnchor = TextAnchor.MiddleLeft, int fontSize = 10)
        {
            GUIStyle centeredStyle = new GUIStyle(EditorStyles.miniLabel);
            centeredStyle.alignment = textAnchor;
            centeredStyle.fontSize = fontSize;
            EditorGUILayout.LabelField(text, style);
        }

        #region EditorEventHandler
        public static void EventHandler(ref int value, string fieldName, Action @event)
        {
            EditorGUI.BeginChangeCheck();
            value = EditorGUILayout.IntField(fieldName, value);
            if (EditorGUI.EndChangeCheck())
                @event?.Invoke();
        }

        public static void EventHandler(ref float value, string fieldName, Action @event)
        {
            EditorGUI.BeginChangeCheck();
            value = EditorGUILayout.FloatField(fieldName, value);
            if (EditorGUI.EndChangeCheck())
                @event?.Invoke();
        }

        public static void EventHandler(ref double value, string fieldName, Action @event)
        {
            EditorGUI.BeginChangeCheck();
            value = EditorGUILayout.DoubleField(fieldName, value);
            if (EditorGUI.EndChangeCheck())
                @event?.Invoke();
        }

        public static void EventHandler(ref long value, string fieldName, Action @event)
        {
            EditorGUI.BeginChangeCheck();
            value = EditorGUILayout.LongField(fieldName, value);
            if (EditorGUI.EndChangeCheck())
                @event?.Invoke();
        }

        public static void EventHandler(ref bool value, string fieldName, Action @event)
        {
            EditorGUI.BeginChangeCheck();
            value = EditorGUILayout.Toggle(fieldName, value);
            if (EditorGUI.EndChangeCheck())
                @event?.Invoke();
        }

        public static void EventHandler(ref string value, string fieldName, Action @event)
        {
            EditorGUI.BeginChangeCheck();
            value = EditorGUILayout.TextField(fieldName, value);
            if (EditorGUI.EndChangeCheck())
                @event?.Invoke();
        }

        public static void EventHandler(ref Vector2 value, string fieldName, Action @event)
        {
            EditorGUI.BeginChangeCheck();
            value = EditorGUILayout.Vector2Field(fieldName, value);
            if (EditorGUI.EndChangeCheck())
                @event?.Invoke();
        }

        public static void EventHandler(ref Vector3 value, string fieldName, Action @event)
        {
            EditorGUI.BeginChangeCheck();
            value = EditorGUILayout.Vector3Field(fieldName, value);
            if (EditorGUI.EndChangeCheck())
                @event?.Invoke();
        }

        public static void EventHandler(ref Vector2Int value, string fieldName, Action @event)
        {
            EditorGUI.BeginChangeCheck();
            value = EditorGUILayout.Vector2IntField(fieldName, value);
            if (EditorGUI.EndChangeCheck())
                @event?.Invoke();
        }

        public static void EventHandler(ref Vector3Int value, string fieldName, Action @event)
        {
            EditorGUI.BeginChangeCheck();
            value = EditorGUILayout.Vector3IntField(fieldName, value);
            if (EditorGUI.EndChangeCheck())
                @event?.Invoke();
        }

        public static void EventHandler(ref Color value, string fieldName, Action @event)
        {
            EditorGUI.BeginChangeCheck();
            value = EditorGUILayout.ColorField(fieldName, value);
            if (EditorGUI.EndChangeCheck())
                @event?.Invoke();
        }

        public static void EventHandler(ref AnimationCurve value, string fieldName, Action @event)
        {
            EditorGUI.BeginChangeCheck();
            value = EditorGUILayout.CurveField(fieldName, value);
            if (EditorGUI.EndChangeCheck())
                @event?.Invoke();
        }

        public static void EventHandler(ref Gradient value, string fieldName, Action @event)
        {
            EditorGUI.BeginChangeCheck();
            value = EditorGUILayout.GradientField(fieldName, value);
            if (EditorGUI.EndChangeCheck())
                @event?.Invoke();
        }

        // Enum 타입
        public static void EventHandler<T>(ref T value, string fieldName, Action @event) where T : System.Enum
        {
            EditorGUI.BeginChangeCheck();
            value = (T)EditorGUILayout.EnumPopup(fieldName, value);
            if (EditorGUI.EndChangeCheck())
                @event?.Invoke();
        }
    }
#endregion EditorEventHandler

}