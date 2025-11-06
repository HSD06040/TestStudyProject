using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    private readonly Stack<IUI> uiStack = new Stack<IUI>();

    public void Push(IUI ui)
    {
        if (uiStack.Count > 0)
        {
            // uiStack.Peek().Hide(); // 이전 UI를 가리려면 이 코드를 사용
        }

        uiStack.Push(ui);
        ui.Show();

        Debug.Log($"[UIManager] 푸쉬: {ui.GetType().Name}. 스텍 사이즈: {uiStack.Count}");
    }

    /// <summary>
    /// 현재 최상단 UI를 닫고 이전 UI로 돌아갑니다.
    /// </summary>
    public void Pop()
    {
        if (uiStack.Count == 0)
        {
            Debug.LogWarning("[UIManager] UI 스텍이 비어있습니다.");
            return;
        }

        IUI currentUI = uiStack.Pop();
        currentUI.Hide();

        if (uiStack.Count > 0)
        {
            uiStack.Peek().Show();
        }

        Debug.Log($"[UIManager] 팝: {currentUI.GetType().Name}. 스텍사이즈: {uiStack.Count}");
    }

    /// <summary>
    /// 모든 UI를 닫고 스택을 비웁니다.
    /// </summary>
    public void ClearStack()
    {
        while (uiStack.Count > 0)
        {
            IUI ui = uiStack.Pop();
            ui.Hide();
        }
    }
}
