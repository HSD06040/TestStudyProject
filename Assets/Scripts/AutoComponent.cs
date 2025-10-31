using System.Reflection;
using UnityEngine;

public abstract class AutoComponent : MonoBehaviour
{
    protected virtual void Reset()
    {
        var fields = GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        foreach (var field in fields)
        {
            if (field.FieldType.IsSubclassOf(typeof(Component)))
            {
                if (field.GetValue(this) == null)
                {
                    var component = GetComponent(field.FieldType);
                    if (component != null)
                    {
                        field.SetValue(this, component);
                        Debug.Log($"Auto-assigned {field.Name}");
                    }
                }
            }
        }
    }
}