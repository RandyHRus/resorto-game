using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineIndicatorManager {

    private class OutlineIndicator {

        public OutlineIndicator()
        {
            Object = new GameObject("Outline indicator");
            Renderer = Object.AddComponent<SpriteRenderer>();
            Transform = Object.transform;

            Renderer.sprite = ResourceManager.Instance.BoxOutline;
            Renderer.sortingLayerName = "Indicator";
            Renderer.drawMode = SpriteDrawMode.Sliced;
            Object.SetActive(false);
        }

        public Transform Transform { get; private set; }
        public SpriteRenderer Renderer { get; private set; }
        public GameObject Object { get; private set; }
    }

    private static OutlineIndicator outlineIndicator;

    //Initializer
    static OutlineIndicatorManager()
    {
        outlineIndicator = new OutlineIndicator();
    }

    public void SetSizeAndPosition(Vector2Int bottomLeft, Vector2Int topRight)
    {
        int xSize = topRight.x - bottomLeft.x + 1;
        int ySize = topRight.y - bottomLeft.y + 1;

        outlineIndicator.Renderer.size = new Vector2(xSize, ySize);
        outlineIndicator.Transform.position = new Vector2(bottomLeft.x + (xSize / 2f) - 0.5f, bottomLeft.y + (ySize / 2f) - 0.5f);
    }

    //Remember to toggle false before destruction
    public void Toggle(bool show)
    {
        outlineIndicator.Object.SetActive(show);
    }

    public void SetColor(Color32 color)
    {
        outlineIndicator.Renderer.color = color;
    }
}
