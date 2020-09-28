using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SidebarPanel : MonoBehaviour
{
    public delegate void PanelDelegate();
    public event PanelDelegate OnPanelOpen;
    public event PanelDelegate OnPanelClosed;

    protected virtual void Awake()
    {
        StartCoroutine(DisableOnStart());
    }

    public void OnOpen()
    {
        gameObject.SetActive(true);
        OnPanelOpen?.Invoke();
    }

    public void OnClose()
    {
        gameObject.SetActive(false);
        OnPanelClosed?.Invoke();
    }

    IEnumerator DisableOnStart()
    {
        yield return 0; //Wait for initialization

        gameObject.SetActive(false);
    }
}
