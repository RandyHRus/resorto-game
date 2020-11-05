using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class MapVisualizer : ScriptableObject
{
    [SerializeField] private string visualizerName = "";
    public string VisualizerName => visualizerName;

    [SerializeField] private Sprite icon = null;
    public Sprite Icon => icon;

    public abstract void ShowVisualizer();

    public abstract void HideVisualizer();
}
