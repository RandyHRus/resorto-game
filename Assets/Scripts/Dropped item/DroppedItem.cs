using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedItem : MonoBehaviour
{
    public InventoryItemInformation ItemInfo { get; private set; }
    private int count;

    private Transform _transform;
    private float waveHeight = 0.05f;
    private float waveSpeed = 2f;

    private float timer;

    private Vector2 defaultPosition;

    public void Initialize(InventoryItemInformation itemInfo, int count)
    {
        this.ItemInfo = itemInfo;
        this.count = count;
        GetComponent<SpriteRenderer>().sprite = itemInfo.ItemIcon;
    }

    public void AddToStack(int count)
    {
        this.count += count;
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
}
