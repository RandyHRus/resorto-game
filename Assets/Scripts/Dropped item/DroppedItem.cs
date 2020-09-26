using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedItem : MonoBehaviour
{
    public InventoryItemInstance ItemInstance { get; private set; }
    public int Count { get; private set; }

    [SerializeField] private Transform innerTransform = null;

    private float waveHeight = 0.1f;
    private float waveSpeed = 4f;
    private float timer;

    public delegate void Callback(DroppedItem droppedItem);

    private readonly float itemFlySpeed = 5f;

    public bool Moving { get; private set; }

    private Coroutine bounceCoroutine;

    private static ItemInformationDisplayWithCountUI itemInformationDisplayUI;

    public void Initialize(InventoryItemInstance itemInstance, int count, Vector2 positionOnGround, float dropHeight, float xSpeed)
    {
        float zPosition = DynamicZDepth.GetDynamicZDepth(positionOnGround.y, 0);

        void OnBounceMove(float h, float xChange)
        {
            Move(h, xChange);
        }

        void OnBounceEnd()
        {
            Moving = false;
        }

        void Move(float h, float xChange)
        {
            transform.position += new Vector3(xChange, 0, 0);
            innerTransform.localPosition = new Vector2(0, h);
        }

        this.ItemInstance = itemInstance;
        this.Count = count;
        innerTransform.GetComponent<SpriteRenderer>().color = ResourceManager.Instance.ItemTagColors[(int)itemInstance.ItemInformation.Tag];

        timer = 0;

        bounceCoroutine = StartCoroutine(BounceEffect.Bounce(dropHeight, xSpeed, OnBounceMove, OnBounceEnd));
        Moving = true;

        itemInformationDisplayUI = new ItemInformationDisplayWithCountUI();
    }

    public void AddToStack(int count)
    {
        this.Count += count;
    }

    void Update()
    {
        if (!Moving)
        {
            timer += Time.deltaTime;
            //Hover
            float offset = Mathf.Sin(waveSpeed * timer) * waveHeight;
            innerTransform.localPosition = new Vector2(0, offset);
        }
    }

    public void StartFlyToTarget(Transform target, Callback onFlyEnd)
    {
        StartCoroutine(FlyToTarget(target, onFlyEnd));
    }

    private IEnumerator FlyToTarget(Transform target, Callback onFlyEnd)
    {
        if (bounceCoroutine != null)
        {
            StopCoroutine(bounceCoroutine);
        }
        Moving = true;

        Vector2 goal, startPos;
        float distanceTravelled, distanceToTravel;
        RecalculateGoal();

        while (Vector2.Distance(transform.position, target.position) > 0.1f)
        {
            if (goal != (Vector2)target.position)
            {
                RecalculateGoal();
            }

            distanceTravelled += Time.deltaTime * itemFlySpeed;
            if (distanceTravelled > distanceToTravel)
                distanceTravelled = distanceToTravel;

            Vector2 proposedPos = Vector2.Lerp(startPos, goal, distanceTravelled / distanceToTravel);

            transform.position = new Vector3(proposedPos.x, proposedPos.y, transform.position.z);
            //innerTransform.position = new Vector2(0, proposedPos.y);

            yield return 0;
        }

        Moving = false;
        onFlyEnd(this);

        void RecalculateGoal()
        {
            startPos = transform.position;
            goal = target.position;
            distanceTravelled = 0;
            distanceToTravel = Vector2.Distance(startPos, goal);
        }
    }

    private void OnMouseEnter()
    {
        if (!CheckMouseOverUI.IsMouseOverUI())
        {
            itemInformationDisplayUI.Show(true);
            itemInformationDisplayUI.SetItem(ItemInstance, Count);
            MouseUIInformationDisplayManager.SetShownUI(itemInformationDisplayUI);
        }
    }

    private void OnMouseOver()
    {
        if (!CheckMouseOverUI.IsMouseOverUI())
        {
            itemInformationDisplayUI.Show(true);
            itemInformationDisplayUI.ObjectTransform.position = ((Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition)) + new Vector2(1.5f, -0.5f);
        }
        else
        {
            itemInformationDisplayUI.Show(false);
        }
    }

    private void OnMouseExit()
    {
        itemInformationDisplayUI.Show(false);
    }
}
