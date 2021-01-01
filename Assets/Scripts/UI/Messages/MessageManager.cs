using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageManager : MonoBehaviour
{
    [SerializeField] private GameObject itemGainMessageBox = null;
    public GameObject ItemGainMessageBox => itemGainMessageBox;

    [SerializeField] private GameObject warningMessageBox = null;
    public GameObject WarningMessageBox => warningMessageBox;

    [SerializeField] private GameObject fishGainMessageBox = null;
    public GameObject FishGainMessageBox => fishGainMessageBox;

    [SerializeField] private GameObject cosmeticGainMessageBox = null;
    public GameObject CosmeticGainMessageBox => cosmeticGainMessageBox;

    [SerializeField] private GameObject touristArrivedMessage = null;
    public GameObject TouristArrivedMessage => touristArrivedMessage;

    [SerializeField] private GameObject touristLeftMessage = null;
    public GameObject TouristLeftMessage => touristLeftMessage;

    [SerializeField] private Canvas canvas = null;
    public Canvas Canvas => canvas;

    private static readonly float boxFadeSpeed = 1f;
    private static readonly int paddingBetween = 50;
    private static readonly int maxMessages = 6;

    private static readonly int messagesXPosWhenSidebarOpen   = -250;
    private static readonly int messagesXPosWhenSidebarClosed = -80;
    private int currentSideBarXPos;

    private Queue<MessageBox> messageBoxes;

    private static MessageManager _instance;
    public static MessageManager Instance { get { return _instance; } }
    private void Awake()
    {
        //Singleton
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;
            }
        }
        messageBoxes = new Queue<MessageBox>();

        currentSideBarXPos = messagesXPosWhenSidebarClosed;
    }

    private void Start()
    {
        InventoryManager.Instance.OnItemGained += (InventoryItemInstance item, int count) => { item.ShowMessage(count); };
        Sidebar.Instance.OnSidebarOpened += MoveOutMessages;
        Sidebar.Instance.OnSidebarClosed += MoveInMessages;
        TouristsManager.Instance.OnTouristAdded += OnTouristAddedHandler;
        TouristsManager.Instance.OnTouristRemoved += OnTouristRemovedHandler;
    }

    private void OnDestroy()
    {
        InventoryManager.Instance.OnItemGained -= (InventoryItemInstance item, int count) => { item.ShowMessage(count); };
        Sidebar.Instance.OnSidebarOpened -= MoveOutMessages;
        Sidebar.Instance.OnSidebarClosed -= MoveInMessages;
        TouristsManager.Instance.OnTouristAdded -= OnTouristAddedHandler;
        TouristsManager.Instance.OnTouristRemoved -= OnTouristRemovedHandler;
    }

    private void OnTouristAddedHandler(TouristMonoBehaviour mono)
    {
        new TouristArrivedMessage(mono.TouristComponents);
    }

    private void OnTouristRemovedHandler(TouristMonoBehaviour mono)
    {
        new TouristLeftMessage(mono.TouristComponents);
    }

    public void ShowMessage(MessageBox newMessageBox)
    {
        newMessageBox.RectTransform.anchoredPosition = new Vector2(currentSideBarXPos, 40);
        foreach (MessageBox messageBox in messageBoxes)
        {
            Vector2 pos = messageBox.RectTransform.anchoredPosition;
            messageBox.RectTransform.anchoredPosition = new Vector2(pos.x, pos.y + paddingBetween);
        }

        messageBoxes.Enqueue(newMessageBox);

        if (messageBoxes.Count > maxMessages)
        {
            messageBoxes.Peek().Destroy();
            messageBoxes.Dequeue();
        }
    }

    private void Update()
    {
        foreach (MessageBox messageBox in messageBoxes)
        {
            if (messageBox.timeRemaining <= 0 && messageBox.alpha > 0)
                messageBox.alpha -= boxFadeSpeed * Time.deltaTime;
            else
                messageBox.timeRemaining -= Time.deltaTime;
        }

        while (messageBoxes.Count != 0 && messageBoxes.Peek().alpha <= 0)
        {
            messageBoxes.Peek().Destroy();
            messageBoxes.Dequeue();
        }
    } 

    private void MoveOutMessages()
    {
        currentSideBarXPos = messagesXPosWhenSidebarOpen;

        foreach (MessageBox m in messageBoxes)
        {
            m.RectTransform.anchoredPosition = new Vector2(currentSideBarXPos, m.RectTransform.anchoredPosition.y);
        }
    }

    private void MoveInMessages()
    {
        currentSideBarXPos = messagesXPosWhenSidebarClosed;

        foreach (MessageBox m in messageBoxes)
        {
            m.RectTransform.anchoredPosition = new Vector2(currentSideBarXPos, m.RectTransform.anchoredPosition.y);
        }
    }


}
