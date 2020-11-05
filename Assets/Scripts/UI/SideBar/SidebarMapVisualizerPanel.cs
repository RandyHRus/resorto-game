using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SidebarMapVisualizerPanel : SidebarPanel
{
    [SerializeField] private MapVisualizer[] visualizers = null;

    private SelectionPanel<MapVisualizerSelectionComponentUI> visualizersPanel;

    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform t in transform.GetComponentsInChildren<Transform>())
        {
            if (t.tag == "List Field")
                visualizersPanel = new SelectionPanel<MapVisualizerSelectionComponentUI>(t.gameObject);
        }

        foreach (MapVisualizer v in visualizers)
        {
            visualizersPanel.InsertListComponent(new MapVisualizerSelectionComponentUI(v, visualizersPanel));
        }
    }
}
