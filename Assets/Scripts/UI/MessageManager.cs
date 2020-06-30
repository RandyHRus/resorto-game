using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageManager : MonoBehaviour
{
    [SerializeField] private GameObject itemGainMessageBox = null;
    [SerializeField] private Canvas canvas = null;
    private Queue<MessageBox> messageBoxes;

    private float boxFadeSpeed = 0.01f;
    private float boxShowTime = 3;
    private int paddingBetween = 100;
    private int maxMessages = 3;


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

        InventoryManager.OnItemGained += (InventoryItem item, int count) => { new ItemGainMessage(item, count); };
    }

    private abstract class MessageBox: UIObject
    {
        private CanvasGroup group;
        public float alpha {
            get {
                return group.alpha;
            }
            set {
                group.alpha = value;
            }
        }
        public float timeRemaining;

        public MessageBox(GameObject prefab): base(prefab, Instance.canvas.transform)
        {
            group = ObjectInScene.GetComponent<CanvasGroup>();
            timeRemaining = Instance.boxShowTime;

            RectTransform.anchoredPosition = new Vector2(-120, 60);
            foreach(MessageBox messageBox in Instance.messageBoxes)
            {
                Vector2 pos = messageBox.RectTransform.anchoredPosition;
                messageBox.RectTransform.anchoredPosition = new Vector2(pos.x, pos.y + Instance.paddingBetween);
            }

            Instance.messageBoxes.Enqueue(this);

            if (Instance.messageBoxes.Count > Instance.maxMessages)
            {
                Destroy(Instance.messageBoxes.Peek().ObjectInScene);
                Instance.messageBoxes.Dequeue();
            }
        }
    }

    private class ItemGainMessage : MessageBox
    {
        public ItemGainMessage(InventoryItem item, int count) : base(Instance.itemGainMessageBox)
        {
            //Find child components
            Transform t = ObjectInScene.transform;
            foreach (Transform tr in t)
            {
                if (tr.tag == "Item Count Field")
                {
                    tr.GetComponent<Text>().text = "x" + count.ToString();
                }
                else if (tr.tag == "Item Name Field")
                {
                    tr.GetComponent<Text>().text = item.itemName;
                }
                else if (tr.tag == "Item Icon Field")
                {
                    tr.GetComponent<Image>().sprite = item.itemIcon;
                }
            }
        }
    }

    private void Update()
    {
        foreach (MessageBox messageBox in messageBoxes)
        {
            if (messageBox.timeRemaining <= 0 && messageBox.alpha > 0)
                messageBox.alpha -= boxFadeSpeed;
            else
                messageBox.timeRemaining -= Time.deltaTime;
        }

        while (messageBoxes.Count != 0 && messageBoxes.Peek().alpha <= 0)
        {
            Destroy(messageBoxes.Peek().ObjectInScene);
            messageBoxes.Dequeue();
        }
    } 

}
