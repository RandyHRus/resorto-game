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

    [EnumNamedArray(typeof(SidebarTab)), SerializeField] private SidebarPanel[] sidebarPanels = null;
    private Dictionary<SidebarTab, SidebarPanel> sidebarPanelDict = new Dictionary<SidebarTab, SidebarPanel>();

    public delegate void SidebarOpenedDelegate();
    public static event SidebarOpenedDelegate OnSidebarOpened; //Only should run when sidebar first opens, not when switched

    public delegate void SidebarClosedDelegate();
    public static event SidebarClosedDelegate OnSidebarClosed;

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

        for (int i = 0; i < sidebarPanels.Length; i++)
        {
            sidebarPanelDict.Add((SidebarTab)i, sidebarPanels[i]);
        }
    }

    public SidebarPanel GetPanel(SidebarTab tab)
    {
        return sidebarPanelDict[tab];
    }

    public void OpenSidebar(SidebarTab tab)
    {
        OpenSidebar(sidebarPanelDict[tab]);
    }

    public void OpenSidebar(SidebarPanel panel)
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
                OnSidebarOpened?.Invoke();
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

        if (CurrentPanel == null)
            return;

        SidebarPanel oldPanel = CurrentPanel;
        CurrentPanel = null;
        oldPanel.OnClose();

        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        currentCoroutine = StartCoroutine(LerpEffect.LerpSpeed(rectTransform.anchoredPosition.x, defaultPosition.x, slideSpeed, OnProgress, null));

        OnSidebarClosed?.Invoke();
    }
}

public enum SidebarTab
{
    Build,
    Terrain,
    Region,
    Tasks,
    Statistics,
    Tourists,
    Settings
}