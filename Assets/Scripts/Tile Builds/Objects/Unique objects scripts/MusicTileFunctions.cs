using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MusicTileFunctions : MonoBehaviour, ITileObjectFunctions
{
    [SerializeField] private Sprite normalSprite = null;
    [SerializeField] private Sprite downSprite = null;
    private SpriteRenderer sprRenderer;

    public void Start()
    {
        sprRenderer = GetComponent<SpriteRenderer>();
        if (sprRenderer == null)
            Debug.Log("No sprite renderer found!");
    }

    public void Initialize(BuildOnTile objectData)
    {

    }

    public void StepOn()
    {
        sprRenderer.sprite = downSprite;
    }

    public void StepOff()
    {
        sprRenderer.sprite = normalSprite;
    }

    public void ClickInteract()
    {

    }
}
