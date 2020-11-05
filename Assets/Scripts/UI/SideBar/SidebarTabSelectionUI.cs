using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SidebarTabSelectionUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private SidebarTab targetPanelTab = 0;
    private SidebarPanel targetPanel;

    private RectTransform rectTransform;
    private Vector2 defaultPosition;
    private Vector2 slideOutTargetPosition;
    private float slideSpeed = 600f;
    private Coroutine currentCoroutine;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        defaultPosition = rectTransform.anchoredPosition;
        slideOutTargetPosition = defaultPosition - new Vector2(80, 0);

        GetComponent<Button>().onClick.AddListener(OnButtonClick);
    }

    private void Start()
    {
        targetPanel = Sidebar.Instance.GetPanel(targetPanelTab);
        targetPanel.OnPanelClosed += SlideIn;
        targetPanel.OnPanelOpen += SlideOut;
    }

    private void OnButtonClick()
    {
        Sidebar.Instance.OpenSidebar(targetPanelTab);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SlideOut();
    }

    private void SlideOut()
    {
        void OnProgress(float position)
        {
            rectTransform.anchoredPosition = new Vector2(position, defaultPosition.y);
        }

        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        currentCoroutine = StartCoroutine(LerpEffect.LerpSpeed(rectTransform.anchoredPosition.x, slideOutTargetPosition.x, slideSpeed, OnProgress, null, false));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SlideIn();
    }

    private void SlideIn()
    {
        void OnProgress(float position)
        {
            rectTransform.anchoredPosition = new Vector2(position, defaultPosition.y);
        }

        //Should not be closed if current panel is this
        if (Sidebar.Instance.CurrentPanel == targetPanel)
            return;

        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        currentCoroutine = StartCoroutine(LerpEffect.LerpSpeed(rectTransform.anchoredPosition.x, defaultPosition.x, slideSpeed, OnProgress, null, false));
    }
}
