using System;
using UnityEngine;

namespace HSD_Editor
{
    public struct GridNodeData
    {
        public readonly Vector2Int position;
        public readonly int fontSize;
        public readonly string nodeName;
        public readonly Sprite nodeIcon;

        public GridNodeData(Vector2Int position, string nodeName, Sprite nodeIcon = null, int fontSize = 8)
        {
            this.position = position;
            this.fontSize = fontSize;
            this.nodeName = nodeName;
            this.nodeIcon = nodeIcon;
        }
    }

    public struct EventButtonData
    {
        public readonly Action clickEvent;
        public readonly ButtonMetaData metaData;

        public EventButtonData(string name, Action clickEvent = null, Color color = default, Sprite icon = null, int fontSize = 8)
        {
            this.clickEvent = clickEvent;
            this.metaData = new ButtonMetaData(name, icon, color, fontSize);
        }
    }

    public struct ButtonMetaData
    {
        public readonly int fontSize;
        public readonly string name;
        public readonly Sprite icon;
        public readonly Color color;

        public ButtonMetaData(string nodeName, Sprite nodeIcon = null, Color color = default, int fontSize = 8)
        {
            this.fontSize = fontSize;
            this.name = nodeName;
            this.icon = nodeIcon;
            this.color = color;
        }
    }

    public struct EditorButtonEvent
    {
        public readonly Action leftClickAction;
        public readonly Action rightClickAction;

        public EditorButtonEvent(Action leftClickAction, Action rightClickAction)
        {
            this.leftClickAction = leftClickAction;
            this.rightClickAction = rightClickAction;
        }
    }
}
