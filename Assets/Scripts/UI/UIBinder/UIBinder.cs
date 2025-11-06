using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UIBind
{
    public class BindingAttribute : System.Attribute
    {
        public string objectName;

        public BindingAttribute(string objectName = "")
        {
            this.objectName = objectName;
        }
    }

    public static class UIBinder
    {
        private static readonly Dictionary<int, bool> bindDict = new Dictionary<int, bool>(100);
        const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

        #region Initialization and Scene Load Handling
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeAndBindAll()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;

            PerformBinding();
        }
        
        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            PerformBinding();
        }

        private static void PerformBinding()
        {
            UIBase[] uIBases = UnityEngine.Object.FindObjectsByType<UIBase>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            ).Where(
                u => !bindDict.TryGetValue(u.GetInstanceID(), out bool value) || !value
            ).ToArray();

            foreach (var uiBase in uIBases)
            {
                BindUI(uiBase);
            }
        }
        #endregion

        public static void BindUI(UIBase uiBase)
        {
            if(bindDict.TryGetValue(uiBase.GetInstanceID(), out bool isBind))
            {
                if (isBind)
                    return;
            }

            var type = uiBase.GetType();
            var fields = uiBase.GetType().GetFields(bindingFlags)
                .Where(member => Attribute.IsDefined(member, typeof(BindingAttribute)));            

            foreach (var field in fields)
            {
                var fieldType = field.FieldType;
                var value = field.GetValue(uiBase);

                if (value != null)
                    continue;

                string objectName = ((BindingAttribute)Attribute.GetCustomAttribute(field, typeof(BindingAttribute))).objectName;
                
                if(objectName.IsNullOrEmpty())
                    objectName = field.Name;

                var @object = FindGameObjectInHierarchy(uiBase.transform, objectName);

                if(@object == null)
                {
                    Debug.LogError($"[UIBinder] '{objectName}'과 일치하는 이름의 오브젝트를 찾지 못했습니다. '{field.Name}' / '{uiBase.name}'");
                    continue;
                }

                var component = @object.GetComponent(fieldType);

                if (component == null)
                {
                    Debug.LogError($"[UIBinder] '{objectName}' 오브젝트에서 '{fieldType}' 컴포넌트를 찾지 못했습니다. '{field.Name}' / '{uiBase.name}'.");
                    continue;
                }

                field.SetValue(uiBase, component);
            }

            bindDict[uiBase.GetInstanceID()] = true;
        }

        private static GameObject FindGameObjectInHierarchy(Transform owner, string objectName)
        {
            foreach (Transform child in owner)
            {
                if (child.name == objectName)
                    return child.gameObject;

                var result = FindGameObjectInHierarchy(child, objectName);

                if (result != null)
                    return result;
            }

            return null;
        }        
    }
}