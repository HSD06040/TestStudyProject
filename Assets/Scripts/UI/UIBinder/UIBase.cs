using UnityEngine;

public class UIBase : MonoBehaviour, IUI
{
    protected virtual void Awake()
    {
        UIBind.UIBinder.BindUI(this);
    }

    public virtual void Show()
    {
        gameObject.SetActive(true);
    }

    public virtual void Hide()
    {
        gameObject.SetActive(false);
    }
}