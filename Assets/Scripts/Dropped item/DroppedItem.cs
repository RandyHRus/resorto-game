using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedItem : MonoBehaviour
{
    public InventoryItemInformation ItemInfo { get; private set; }
    public int Count { get; private set; }

    private Transform _transform;
    private float waveHeight = 0.05f;
    private float waveSpeed = 2f;
    private float timer;
    private Vector2 defaultPosition;
    public bool IsFresh { get; private set; }

    public void Initialize(InventoryItemInformation itemInfo, int count, bool pickupableInstantly)
    {
        this.ItemInfo = itemInfo;
        this.Count = count;
        GetComponent<SpriteRenderer>().color = ResourceManager.Instance.ItemTagColors[(int)itemInfo.Tag];
        IsFresh = !pickupableInstantly;
    }

    public void AddToStack(int count)
    {
        this.Count += count;
    }

    private void Start()
    {
        _transform = transform;
        defaultPosition = _transform.position;
        timer = 0;
    }

    void Update()
    {
        timer += Time.deltaTime;
        //Hover
        float offset = Mathf.Sin(waveSpeed * timer) * waveHeight;
        _transform.position = new Vector2(defaultPosition.x, defaultPosition.y + offset);
    }

    public void UnfreshItem()
    {
        IsFresh = false;
    }
}
