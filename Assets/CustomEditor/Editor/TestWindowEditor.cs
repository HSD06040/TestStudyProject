using UnityEditor;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;

namespace HSD_Editor
{
    [InitializeOnLoad]
    public class TestWindowEditor : EditorWindow
    {
        private Grid grid;
        private ScrollList scrollList;
        private ButtonPanel buttonPanel;

        private Vector2 mainScrollPosition;

        private float gridMaxHeight = 300;
        private Vector2Int gridSize = new Vector2Int(10, 10);
        private Vector2 nodeSize = new Vector2(5f, 5f);
        private Color nodeColor = Color.cyan;

        #region Const
        // EditorPrefs 키 상수
        private const string PREF_GRID_MAX_HEIGHT = "TestWindow_GridMaxHeight";
        private const string PREF_GRID_SIZE_X = "TestWindow_GridSizeX";
        private const string PREF_GRID_SIZE_Y = "TestWindow_GridSizeY";
        private const string PREF_NODE_SIZE_X = "TestWindow_NodeSizeX";
        private const string PREF_NODE_SIZE_Y = "TestWindow_NodeSizeY";
        private const string PREF_NODE_COLOR_R = "TestWindow_NodeColorR";
        private const string PREF_NODE_COLOR_G = "TestWindow_NodeColorG";
        private const string PREF_NODE_COLOR_B = "TestWindow_NodeColorB";
        private const string PREF_NODE_COLOR_A = "TestWindow_NodeColorA"; 
        #endregion

        [MenuItem("HSD_Editor/Test")]
        public static void ShowWindow()
        {
            GetWindow<TestWindowEditor>("Test Window");
        }

        private void OnEnable()
        {
            LoadPrefs();
            Init();
        }

        private void OnDisable()
        {
            SavePrefs(); 
        }

        private void Init()
        {
            if (grid != null && scrollList != null && buttonPanel != null)
                return;

            GUIStyle style = new GUIStyle(EditorStyles.miniLabel);

            grid = new Grid(gridSize, nodeSize, nodeColor, TextAnchor.MiddleCenter, style, 
                new EditorButtonEvent(LeftClickEvent, RightClickEvent), .8f
                );

            scrollList = new ScrollList();

            buttonPanel = new ButtonPanel();
        }

        private void OnGUI()
        {
            if (grid == null || scrollList == null || buttonPanel == null)
            {
                Init();
            }

            mainScrollPosition = EditorGUILayout.BeginScrollView(mainScrollPosition);

            EditorGUI.BeginChangeCheck();

            GUIStyle boldStyle = new GUIStyle(EditorStyles.label)
            {
                fontStyle = FontStyle.Bold
            };

            EditorGUILayout.LabelField("그리드 패널 최소 높이", boldStyle);
            gridMaxHeight = EditorGUILayout.Slider(gridMaxHeight, 100f, 1000f);
            EditorGUILayout.LabelField("그리드 사이즈", boldStyle);
            gridSize = EditorGUILayout.Vector2IntField(GUIContent.none, gridSize);
            EditorGUILayout.LabelField("노드 사이즈", boldStyle);
            nodeSize = EditorGUILayout.Vector2Field(GUIContent.none, nodeSize);
            EditorGUILayout.LabelField("노드 색상", boldStyle);
            nodeColor = EditorGUILayout.ColorField(GUIContent.none, nodeColor);

            if (EditorGUI.EndChangeCheck())
            {
                RefleshGrid();
            }

            #region Test
            List<GridNodeData> datas = new List<GridNodeData> {
                new GridNodeData(new Vector2Int(0,0), "Node 1", null),
                new GridNodeData(new Vector2Int(1,1), "Node 2", null),
                new GridNodeData(new Vector2Int(2,2), "Node 3", null),
                new GridNodeData(new Vector2Int(3,3), "Node 4", null),
                new GridNodeData(new Vector2Int(4,4), "Node 5", null),
            };

            List<ButtonMetaData> buttonMetaDatas = new List<ButtonMetaData>
            {
                new ButtonMetaData("Button 1", null),
                new ButtonMetaData("Button 2", null),
                new ButtonMetaData("Button 3", null),
                new ButtonMetaData("Button 4", null),
                new ButtonMetaData("Button 5", null),
            };

            List<EventButtonData> eventButtonDatas = new List<EventButtonData>
            {
                new EventButtonData("Add", LeftClickEvent, Color.red, fontSize:20),
                new EventButtonData("Remove", RightClickEvent, Color.blue, fontSize:25),
            };
            #endregion

            float buttonPanelHeight = 30f;
            Rect buttonPanelRect = GUILayoutUtility.GetRect(
                GUIContent.none,
                GUIStyle.none,
                GUILayout.ExpandWidth(true),
                GUILayout.Height(buttonPanelHeight)
            );

            buttonPanel.Create(buttonPanelRect, eventButtonDatas);
            EditorGUILayout.Space(10);

            grid.Create(datas, gridMaxHeight);

            EditorGUILayout.Space(10);
            scrollList.SetRightButton(new EventButtonData("Button", RightClickEvent, Color.cyan)).
                SetBaseButtonAction(LeftClickEvent).SetSelectFunc(IsSelected).Create(100, buttonMetaDatas);
            EditorGUILayout.Space(10);
            EditorGUILayout.GradientField("Gradient Field", new Gradient());
            EditorGUILayout.EndScrollView();
        }

        private void RefleshGrid()
        {
            if (grid == null)
            {
                Init();
                return;
            }

            List<GridNodeData> griddata = new List<GridNodeData>();

            for (int i = 0; i < 5; i++)
            {
                griddata.Add(new GridNodeData(
                    new Vector2Int(UnityEngine.Random.Range(0, gridSize.x), UnityEngine.Random.Range(0, gridSize.y)),
                    $"Node {i + 1}"
                    ));
            }

            grid.gridSize = gridSize;
            grid.nodeSize = nodeSize;
            grid.nodeColor = nodeColor;

            grid.Create(griddata, gridMaxHeight);
        }

        private void LeftClickEvent()
        {
            Debug.Log("왼쪽 클릭");
        }

        private void RightClickEvent()
        {
            Debug.Log("오른쪽 클릭");
        }

        private bool IsSelected()
        {
            return true;
        }

        private void LoadPrefs()
        {
            gridMaxHeight = EditorPrefs.GetFloat(PREF_GRID_MAX_HEIGHT, 300f);
            gridSize = new Vector2Int(
                EditorPrefs.GetInt(PREF_GRID_SIZE_X, 10),
                EditorPrefs.GetInt(PREF_GRID_SIZE_Y, 10)
            );
            nodeSize = new Vector2(
                EditorPrefs.GetFloat(PREF_NODE_SIZE_X, 5f),
                EditorPrefs.GetFloat(PREF_NODE_SIZE_Y, 5f)
            );
            nodeColor = new Color(
                EditorPrefs.GetFloat(PREF_NODE_COLOR_R, 0f),
                EditorPrefs.GetFloat(PREF_NODE_COLOR_G, 1f),
                EditorPrefs.GetFloat(PREF_NODE_COLOR_B, 1f),
                EditorPrefs.GetFloat(PREF_NODE_COLOR_A, 1f)
            );
        }

        private void SavePrefs()
        {
            EditorPrefs.SetFloat(PREF_GRID_MAX_HEIGHT, gridMaxHeight);
            EditorPrefs.SetInt(PREF_GRID_SIZE_X, gridSize.x);
            EditorPrefs.SetInt(PREF_GRID_SIZE_Y, gridSize.y);
            EditorPrefs.SetFloat(PREF_NODE_SIZE_X, nodeSize.x);
            EditorPrefs.SetFloat(PREF_NODE_SIZE_Y, nodeSize.y);
            EditorPrefs.SetFloat(PREF_NODE_COLOR_R, nodeColor.r);
            EditorPrefs.SetFloat(PREF_NODE_COLOR_G, nodeColor.g);
            EditorPrefs.SetFloat(PREF_NODE_COLOR_B, nodeColor.b);
            EditorPrefs.SetFloat(PREF_NODE_COLOR_A, nodeColor.a);
        }
    }
}