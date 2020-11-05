using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapVisualizerSelectionComponentUI : ListComponentUI
{
    private readonly MapVisualizer visualizer;

    public MapVisualizerSelectionComponentUI(MapVisualizer visualizer, SelectionPanel<MapVisualizerSelectionComponentUI> parent): base(ResourceManager.Instance.MapVisualizerComponentUI, parent.ObjectTransform)
    {
        this.visualizer = visualizer;

        foreach (Transform t in ObjectTransform.GetComponentsInChildren<Transform>())
        {
            if (t.tag == "Icon Field")
            {
                t.GetComponent<Image>().sprite = visualizer.Icon;
            }
            else if (t.tag == "Name Field")
            {
                OutlinedText text = new OutlinedText(t.gameObject);
                text.SetText(visualizer.VisualizerName);
            }
        }
    }

    public override void OnClick()
    {
        base.OnClick();

        PlayerStateMachineManager.Instance.SwitchState<MapVisualizerState>(new object[] { visualizer });
    }
}
