using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ColorWheel : MonoBehaviour, IPointerClickHandler, IDragHandler
{
    [SerializeField] private GameObject pickedPoint = null;
    [SerializeField] private Slider brightnessSlider = null; 
    private Transform pickedPointTransform;

    private float realRadius;
    private Vector2 wheelPos;

    private float h;
    private float s;
    private float v;

    private void Awake()
    {
        pickedPointTransform = pickedPoint.transform;
        wheelPos = gameObject.GetComponent<RectTransform>().position;

        Vector3[] v = new Vector3[4];
        gameObject.GetComponent<RectTransform>().GetWorldCorners(v);
        Vector2 wheelEdge = new Vector2(v[0].x, wheelPos.y);

        realRadius = Vector2.Distance(wheelEdge, wheelPos);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        MovePointToMouse();
    }

    public void OnDrag(PointerEventData eventData)
    {
        MovePointToMouse();
    }

    private float wheelColorOffsetCorrectionAngle = -35f;

    private void MovePointToMouse()
    {       
        Vector2 mouseToWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector2 offsetFromCenter = new Vector2(mouseToWorld.x - wheelPos.x, mouseToWorld.y - wheelPos.y);

        if (offsetFromCenter.magnitude > realRadius)
        {
            offsetFromCenter = offsetFromCenter.normalized * realRadius;
        }

        Vector3 pointPos = new Vector3(wheelPos.x + offsetFromCenter.x, wheelPos.y + offsetFromCenter.y, 1);
        pickedPointTransform.position =pointPos;

        h = MathFunctions.Mod((MathFunctions.GetAngleBetweenPoints(wheelPos, pointPos) + wheelColorOffsetCorrectionAngle), 360) / 360f; //Need to mod again to account for offset
        //h = mod(((Mathf.Atan2(pointPos.y - wheelPos.y, pointPos.x - wheelPos.x) * 180 / Mathf.PI + wheelColorOffsetCorrectionAngle) / 360f), 1);
        s = offsetFromCenter.magnitude / realRadius;

        SetCharacterPartColor();
    }

    public void MovePointAndSliderToColor(Color32 color)
    {
        Color.RGBToHSV(color, out h, out s, out v);

        //Move point
        {
            float distance = s * realRadius;
            float angleRad = ((h * 360f) - wheelColorOffsetCorrectionAngle) * Mathf.Deg2Rad;

            float xOffset = Mathf.Cos(angleRad) * distance;
            float yOffset = Mathf.Sin(angleRad) * distance;

            pickedPointTransform.position = new Vector3(wheelPos.x + xOffset, wheelPos.y + yOffset, 1);
        }
        //Move slider
        {
            brightnessSlider.value = v;
        }
    }


    public void OnBrightnessSliderChanged()
    {
        v = brightnessSlider.value;

        SetCharacterPartColor();
    }

    private void SetCharacterPartColor()
    {
        Color32 col = Color.HSVToRGB(h, s, v);
        CharacterCustomizationMenu.Instance.SelectColorOption(col);
    }
}
