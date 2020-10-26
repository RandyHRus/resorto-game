using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCClickOn : MonoBehaviour, INonTileClickable
{
    public delegate void Click();
    public event Click OnClick;

    public void NearbyAndOnClick()
    {
        OnClick?.Invoke();
    }
}
