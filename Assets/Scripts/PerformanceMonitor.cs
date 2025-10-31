using UnityEngine;
using System.Collections.Generic;
using System;

public class PerformanceMonitor : MonoBehaviour
{
    #region Settings
    [Header("Toggle")]
    [SerializeField] bool showMonitor = true;
    [SerializeField] KeyCode toggleKey = KeyCode.F12;

    [Header("Display Options")]
    [SerializeField] bool _showFPS = true;
    [SerializeField] bool _showGCMemory = true;
    [SerializeField] bool _showDrawCalls = false;
    [SerializeField] bool _showTriangles = false;

    public bool ShowFPS
    {
        get => _showFPS;
        set
        {
            if (_showFPS != value)
            {
                _showFPS = value;
                RefreshGraphs();
            }
        }
    }

    public bool ShowGCMemory
    {
        get => _showGCMemory;
        set
        {
            if (_showGCMemory != value)
            {
                _showGCMemory = value;
                RefreshGraphs();
            }
        }
    }

    public bool ShowDrawCalls
    {
        get => _showDrawCalls;
        set
        {
            if (_showDrawCalls != value)
            {
                _showDrawCalls = value;
                RefreshGraphs();
            }
        }
    }

    public bool ShowTriangles
    {
        get => _showTriangles;
        set
        {
            if (_showTriangles != value)
            {
                _showTriangles = value;
                RefreshGraphs();
            }
        }
    }

    [Header("Graph Settings")]
    [SerializeField] int graphWidth = 200;
    [SerializeField] int graphHeight = 100;
    [SerializeField] float updateInterval = 0.3f;
    [SerializeField] int maxDataPoints = 5;
    #endregion

    #region Graph Data
    List<GraphData> graphs = new List<GraphData>();
    float nextUpdateTime;
    float currentFPS;
    #endregion

    #region Styles
    GUIStyle labelStyle;
    GUIStyle boxStyle;
    GUIStyle titleStyle;
    GUIStyle valueStyle;
    Texture2D backgroundTexture;
    #endregion

    void Start()
    {
        InitializeGraphs();
        nextUpdateTime = Time.time + updateInterval;
    }

    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(toggleKey))
            showMonitor = !showMonitor;

        if (!showMonitor) return;

        UpdateFPS();

        if (Time.time >= nextUpdateTime)
        {
            UpdateAllGraphs();
            nextUpdateTime = Time.time + updateInterval;
        }
#endif
    }

    void OnGUI()
    {
#if UNITY_EDITOR
        if (!showMonitor) return;

        InitializeStyles();
        DrawMonitorPanel();
#endif
    }

    #region Initialization
    void InitializeGraphs()
    {
        graphs.Clear();

        if (_showFPS)
            graphs.Add(new GraphData("FPS", Color.green, GetFPS, 0, 120, currentFPS => GetFPSColor(currentFPS)));

        if (_showGCMemory)
            graphs.Add(new GraphData("GC Memory (MB)", Color.cyan, GetGCMemory, 0, 10));

        if (_showDrawCalls)
            graphs.Add(new GraphData("Draw Calls", Color.yellow, GetDrawCalls, 0, 1000));

        if (_showTriangles)
            graphs.Add(new GraphData("Triangles (K)", Color.magenta, GetTriangles, 0, 100));

        foreach (var graph in graphs)
            graph.Initialize(maxDataPoints);
    }

    void RefreshGraphs()
    {
        InitializeGraphs();
    }
    #endregion

    #region Property Change Validation
    void OnValidate()
    {
        // Inspector에서 값 변경시 자동 갱신
        if (Application.isPlaying && graphs != null)
        {
            RefreshGraphs();
        }
    }

    void InitializeStyles()
    {
        if (labelStyle == null)
        {
            labelStyle = new GUIStyle
            {
                fontSize = 16,
                fontStyle = FontStyle.Bold,
                normal = { textColor = Color.white }
            };
        }

        if (titleStyle == null)
        {
            titleStyle = new GUIStyle
            {
                fontSize = 10,
                alignment = TextAnchor.UpperCenter,
                normal = { textColor = Color.white }
            };
        }

        if (valueStyle == null)
        {
            valueStyle = new GUIStyle
            {
                fontSize = 9,
                normal = { textColor = Color.white }
            };
        }

        if (boxStyle == null)
        {
            boxStyle = new GUIStyle(GUI.skin.box);
            if (backgroundTexture == null)
            {
                backgroundTexture = new Texture2D(1, 1);
                backgroundTexture.SetPixel(0, 0, new Color(0, 0, 0, 0.7f));
                backgroundTexture.Apply();
            }
            boxStyle.normal.background = backgroundTexture;
        }
    }
    #endregion

    #region Update Logic
    void UpdateFPS()
    {
        float deltaTime = Time.unscaledDeltaTime;
        currentFPS += (1.0f / deltaTime - currentFPS) * 0.1f;
    }

    void UpdateAllGraphs()
    {
        foreach (var graph in graphs)
            graph.UpdateData();
    }
    #endregion

    #region Drawing
    void DrawMonitorPanel()
    {
        const float startX = 10;
        float startY = 10;
        const float spacing = 10;
        const float labelHeight = 30;

        float totalHeight = CalculateTotalHeight(labelHeight, spacing);
        GUI.Box(new Rect(startX - 5, startY - 5, graphWidth + 10, totalHeight), "", boxStyle);

        foreach (var graph in graphs)
        {
            DrawGraphSection(ref startY, startX, spacing, labelHeight, graph);
        }

        DrawToggleHint(startY, startX);
    }

    void DrawGraphSection(ref float startY, float startX, float spacing, float labelHeight, GraphData graph)
    {
        // 현재 값 라벨
        labelStyle.normal.textColor = graph.GetCurrentColor();
        GUI.Label(new Rect(startX, startY, graphWidth, labelHeight), graph.GetLabel(), labelStyle);

        // 그래프
        startY += labelHeight;
        DrawGraph(new Rect(startX, startY, graphWidth, graphHeight), graph);

        startY += graphHeight + spacing;
    }

    void DrawGraph(Rect rect, GraphData graph)
    {
        GUI.Box(rect, "");

        // 타이틀
        GUI.Label(new Rect(rect.x, rect.y + 2, rect.width, 15), graph.title, titleStyle);

        // 라인 그래프
        DrawGraphLines(rect, graph);

        // Min/Max 값
        DrawMinMaxLabels(rect, graph);

        // 현재 값
        DrawCurrentValue(rect, graph);
    }

    void DrawGraphLines(Rect rect, GraphData graph)
    {
        if (graph.history.Count < 2) return;

        float stepX = rect.width / (graph.history.Count - 1);

        for (int i = 0; i < graph.history.Count - 1; i++)
        {
            Vector2 start = CalculateGraphPoint(rect, i, graph.history[i], stepX, graph.minValue, graph.maxValue);
            Vector2 end = CalculateGraphPoint(rect, i + 1, graph.history[i + 1], stepX, graph.minValue, graph.maxValue);
            DrawLine(start, end, graph.color, 2f);
        }
    }

    Vector2 CalculateGraphPoint(Rect rect, int index, float value, float stepX, float minValue, float maxValue)
    {
        float x = rect.x + index * stepX;
        float normalizedY = Mathf.InverseLerp(minValue, maxValue, value);
        float y = rect.y + rect.height - (normalizedY * rect.height);
        return new Vector2(x, y);
    }

    void DrawMinMaxLabels(Rect rect, GraphData graph)
    {
        valueStyle.normal.textColor = Color.white;
        GUI.Label(new Rect(rect.x + 2, rect.y + 15, 50, 15), $"Max: {graph.maxValue:0}", valueStyle);
        GUI.Label(new Rect(rect.x + 2, rect.y + rect.height - 15, 50, 15), $"Min: {graph.minValue:0}", valueStyle);
    }

    void DrawCurrentValue(Rect rect, GraphData graph)
    {
        if (graph.history.Count > 0)
        {
            valueStyle.normal.textColor = graph.color;
            float currentValue = graph.history[graph.history.Count - 1];
            GUI.Label(new Rect(rect.x + rect.width - 60, rect.y + rect.height / 2 - 7, 60, 15),
                $"Now: {currentValue:0.0}", valueStyle);
        }
    }

    void DrawToggleHint(float startY, float startX)
    {
        labelStyle.fontSize = 12;
        labelStyle.normal.textColor = Color.gray;
        GUI.Label(new Rect(startX, startY, graphWidth, 20), $"Press {toggleKey} to toggle", labelStyle);
        labelStyle.fontSize = 16; // Reset
    }

    void DrawLine(Vector2 start, Vector2 end, Color color, float width)
    {
        Vector2 direction = end - start;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float distance = direction.magnitude;

        GUIUtility.RotateAroundPivot(angle, start);

        Color originalColor = GUI.color;
        GUI.color = color;
        GUI.DrawTexture(new Rect(start.x, start.y - width / 2, distance, width), Texture2D.whiteTexture);
        GUI.color = originalColor;

        GUIUtility.RotateAroundPivot(-angle, start);
    }
    #endregion

    #region Helpers
    float CalculateTotalHeight(float labelHeight, float spacing)
    {
        return graphs.Count * (labelHeight + graphHeight + spacing) + 30;
    }

    Color GetFPSColor(float fps)
    {
        if (fps >= 60) return Color.green;
        if (fps >= 30) return Color.yellow;
        return Color.red;
    }
    #endregion

    #region Data Getters
    float GetFPS() => currentFPS;
    float GetGCMemory() => System.GC.GetTotalMemory(false) / 1048576f;
    float GetDrawCalls() => UnityEngine.Profiling.Profiler.GetRuntimeMemorySizeLong(null) / 1024f; // 임시
    float GetTriangles() => 0; // 렌더링 통계는 런타임에서 직접 접근 어려움
    #endregion

    void OnDestroy()
    {
        if (backgroundTexture != null)
            Destroy(backgroundTexture);
    }
}

#region Graph Data Class
public class GraphData
{
    public string title;
    public Color color;
    public List<float> history;
    public float minValue;
    public float maxValue;

    Func<float> dataGetter;
    Func<float, Color> colorGetter;

    public GraphData(string title, Color color, Func<float> dataGetter, float minValue, float maxValue, Func<float, Color> colorGetter = null)
    {
        this.title = title;
        this.color = color;
        this.dataGetter = dataGetter;
        this.minValue = minValue;
        this.maxValue = maxValue;
        this.colorGetter = colorGetter;
        this.history = new List<float>();
    }

    public void Initialize(int dataPoints)
    {
        history.Clear();
        for (int i = 0; i < dataPoints; i++)
            history.Add(0);
    }

    public void UpdateData()
    {
        float value = dataGetter.Invoke();
        history.RemoveAt(0);
        history.Add(value);

        // 동적으로 maxValue 조정 (GC 메모리 같은 경우)
        if (title.Contains("GC"))
        {
            float max = Mathf.Max(history.ToArray()) * 1.2f;
            maxValue = Mathf.Max(max, 10);
        }
    }

    public string GetLabel()
    {
        float current = history.Count > 0 ? history[history.Count - 1] : 0;
        return $"{title}: {current:0.0}";
    }

    public Color GetCurrentColor()
    {
        if (colorGetter != null)
        {
            float current = history.Count > 0 ? history[history.Count - 1] : 0;
            return colorGetter.Invoke(current);
        }
        return color;
    }
}
#endregion