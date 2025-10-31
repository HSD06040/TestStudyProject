using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HSD_Editor
{
    public class ScrollList
    {
        private readonly Color selectColor;
        private bool isRightButton;
        private Vector2 listScrollPosition;
        private EventButtonData rightButton = default;
        private Func<bool> selectFuc;
        private Action buttonAction;

        public ScrollList(Color selectColor = default)
        {
            this.selectColor = selectColor != default ? Color.cyan : Color.gray;            
        }

        public void Create(float height, List<ButtonMetaData> datas)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            listScrollPosition = EditorGUILayout.BeginScrollView(listScrollPosition, GUILayout.Height(height));

            datas.Sort((a, b) => a.name.CompareTo(b.name));

            for (int i = 0; i < datas.Count; i++)
            {
                bool isSelected = selectFuc != null ? selectFuc() : false;

                Color originalColor = GUI.backgroundColor;

                if (isSelected)
                {
                    GUI.backgroundColor = selectColor;
                }

                EditorGUILayout.BeginHorizontal(GUI.skin.box);

                if (datas[i].icon != null)
                {
                    Texture2D iconTexture = AssetPreview.GetAssetPreview(datas[i].icon);
                    if (iconTexture != null)
                    {
                        GUILayout.Label(iconTexture, GUILayout.Width(24), GUILayout.Height(24));
                    }
                    else
                    {
                        GUILayout.Space(28);
                    }
                }
                else
                {
                    GUILayout.Space(28);
                }

                if (GUILayout.Button(datas[i].name, EditorStyles.label) && buttonAction != null)
                {                    
                    buttonAction?.Invoke();
                }

                if(isRightButton && !string.IsNullOrEmpty(rightButton.metaData.name))
                {
                    GUI.backgroundColor = rightButton.metaData.color;

                    if (GUILayout.Button(rightButton.metaData.name, GUILayout.Width(50)))
                    {
                        rightButton.clickEvent?.Invoke();
                    }
                }                

                EditorGUILayout.EndHorizontal();

                GUI.backgroundColor = originalColor;
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }        

        public ScrollList SetRightButton(EventButtonData rightButton)
        {
            isRightButton = true;
            this.rightButton = rightButton;
            return this;
        }

        public ScrollList SetBaseButtonAction(Action buttonAction)
        {
            this.buttonAction = buttonAction;
            return this;
        }

        public ScrollList SetSelectFunc(Func<bool> selectFuc)
        {
            this.selectFuc = selectFuc;
            return this;
        }
    }
}