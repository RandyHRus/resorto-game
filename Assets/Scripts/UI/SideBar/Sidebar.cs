using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sidebar : MonoBehaviour
{
    private RectTransform rectTransform;
    private Vector2 defaultPosition;
    private Vector2 openPosition;
    private Coroutine currentCoroutine;
    private float slideSpeed = 1100f;

    public SidebarPanel CurrentPanel { get; private set; }

    private static Sidebar _instance;
    public static Sidebar Instance { get { return _instance; } }
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        rectTransform = GetComponent<RectTransform>();
        defaultPosition = rectTransform.anchoredPosition;
        openPosition = defaultPosition - new Vector2(rectTransform.sizeDelta.x, 0);
    }

    public void OpenSideBar(SidebarPanel panel)
    {
        void Open()
        {
            void OnProgress(float position)
            {
                rectTransform.anchoredPosition = new Vector2(position, defaultPosition.y);
            }

            if (currentCoroutine != null)
                StopCoroutine(currentCoroutine);

            currentCoroutine = StartCoroutine(LerpEffect.LerpSpeed(rectTransform.anchoredPosition.x, openPosition.x, slideSpeed, OnProgress, null));
        }

        void Close()
        {
            void OnProgress(float position)
            {
                rectTransform.anchoredPosition = new Vector2(position, defaultPosition.y);
            }

            void OnEnd()
            {
                Open();
            }

            if (currentCoroutine != null)
                StopCoroutine(currentCoroutine);

            currentCoroutine = StartCoroutine(LerpEffect.LerpSpeed(rectTransform.anchoredPosition.x, defaultPosition.x, slideSpeed, OnProgress, OnEnd));
        }

        if (CurrentPanel == panel)
        {
            return;
        }
        else
        {
            panel.OnOpen();

            if (CurrentPanel != null)
            {
                SidebarPanel oldPanel = CurrentPanel;
                CurrentPanel = panel;
                oldPanel.OnClose();
                Close();
            }
            else
            {
                CurrentPanel = panel;
                Open();
            }
        }
    }

    public void CloseSidebar()
    {
        void OnProgress(float position)
        {
            rectTransform.anchoredPosition = new Vector2(position, defaultPosition.y);
        }

        SidebarPanel oldPanel = CurrentPanel;
        CurrentPanel = null;
        oldPanel.OnClose();

        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        currentCoroutine = StartCoroutine(LerpEffect.LerpSpeed(rectTransform.anchoredPosition.x, defaultPosition.x, slideSpeed, OnProgress, null));
    }
}

public enum SidebarTab
{
    Create,
    Tasks,
    Statistics,
    Tourists,
    Settings
}