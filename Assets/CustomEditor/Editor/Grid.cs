using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HSD_Editor
{

    public class Grid
    {
        public Vector2Int gridSize;
        public Vector2 nodeSize;
        public Color nodeColor;
        private readonly float iconRatio;
        private readonly GUIStyle guiStyle;
        private readonly TextAnchor textAnchor;

        private readonly EditorButtonEvent buttonEvent;

        private Vector2 scrollPosition;

        public Grid(Vector2Int gridSize, Vector2 nodeSize, Color backgroundColor, TextAnchor textAnchor, GUIStyle guiStyle, EditorButtonEvent buttonEvent, float iconRatio = 1)
        {
            this.gridSize = gridSize;
            this.nodeSize = nodeSize;
            this.iconRatio = iconRatio;
            this.nodeColor = backgroundColor;
            this.guiStyle = guiStyle;
            this.buttonEvent = buttonEvent;
            this.textAnchor = textAnchor;
        }

        public void Create(List<GridNodeData> gridDatas, float gridMaxHeight)
        {
            Dictionary<Vector2Int, GridNodeData> dataMap = new Dictionary<Vector2Int, GridNodeData>();
            foreach (var data in gridDatas)
            {
                dataMap[data.position] = data;
            }

            float verticalPadding = 10f;
            float availableHeight = gridMaxHeight - verticalPadding;

            float minRequiredHeight = nodeSize.y * gridSize.y;

            float calculatedNodeHeight = Mathf.Max(nodeSize.y, availableHeight / gridSize.y);
            float totalContentHeight = calculatedNodeHeight * gridSize.y;

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, EditorStyles.helpBox, GUILayout.Height(gridMaxHeight));

            float verticalHeight = totalContentHeight > availableHeight ? totalContentHeight : availableHeight;

            GUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Height(verticalHeight));

            for (int y = 0; y < gridSize.y; y++)
            {
                GUILayout.BeginHorizontal(GUILayout.Height(calculatedNodeHeight));

                for (int x = 0; x < gridSize.x; x++)
                {
                    Vector2Int currentPos = new Vector2Int(x, y);

                    if (dataMap.TryGetValue(currentPos, out GridNodeData data))
                    {
                        CreateNode(currentPos, data, calculatedNodeHeight);
                    }
                    else
                    {
                        CreateNode(currentPos, default(GridNodeData), calculatedNodeHeight);
                    }
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();

            EditorGUILayout.EndScrollView();
        }

        private void CreateNode(Vector2Int position, GridNodeData data, float height)
        {
            Rect nodeRect = GUILayoutUtility.GetRect(nodeSize.x, height);

            Color originalGUIColor = GUI.color;
            Color originalGUIBgColor = GUI.backgroundColor;

            GUI.backgroundColor = nodeColor;

            if (!HandleRightClickEvent(nodeRect, position) && GUI.Button(nodeRect, GUIContent.none, GUI.skin.button))
            {
                buttonEvent.leftClickAction?.Invoke();
            }

            GUI.backgroundColor = originalGUIBgColor;
            GUI.color = originalGUIColor;

            if (data.nodeIcon != null)
            {
                EditorDrawUtile.DrawIconInRectCenter(nodeRect, data.nodeIcon, iconRatio);
            }

            if (!string.IsNullOrEmpty(data.nodeName))
            {
                Rect posRect = new Rect(nodeRect.x, nodeRect.y + nodeRect.height - 10, nodeRect.width, 10);
                EditorDrawUtile.DrawLabel(posRect, data.nodeName, guiStyle, textAnchor, data.fontSize);
            }
        }

        private bool HandleRightClickEvent(Rect nodeRect, Vector2Int position)
        {
            Event currentEvent = Event.current;

            if (nodeRect.Contains(currentEvent.mousePosition) &&
                currentEvent.type == EventType.MouseDown &&
                currentEvent.button == 1)
            {

                buttonEvent.rightClickAction?.Invoke();
                currentEvent.Use();

                return true;
            }

            return false;
        }
    }
}