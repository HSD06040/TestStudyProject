using System.Collections.Generic;
using UnityEngine;

namespace HSD_Editor
{
    public class ButtonPanel
    {
        public void Create(Rect rect, List<EventButtonData> eventButtonDatas)
        {
            int count = eventButtonDatas.Count;
            float buttonWidth = rect.width / count;
            float buttonHeight = rect.height;

            Color originalGUIColor = GUI.backgroundColor;

            for (int i = 0; i < count; i++)
            {
                Rect buttonRect = new Rect(
                    rect.x + (i * buttonWidth),
                    rect.y,
                    buttonWidth,
                    buttonHeight
                );

                EventButtonData buttonData = eventButtonDatas[i];

                if (buttonData.metaData.color == default)
                {
                    GUI.backgroundColor = originalGUIColor;
                }
                else
                {
                    GUI.backgroundColor = buttonData.metaData.color;
                }

                if (GUI.Button(buttonRect, buttonData.metaData.name))
                {
                    if (buttonData.clickEvent != null)
                        buttonData.clickEvent?.Invoke();
                }
            }

            GUI.backgroundColor = originalGUIColor;
        }
    }
}