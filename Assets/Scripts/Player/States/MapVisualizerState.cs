using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="States/Player/Map Visualizer")]
public class MapVisualizerState : PlayerState
{
    public override bool AllowMovement => false;

    public override bool AllowMouseDirectionChange => false;

    public override CameraMode CameraMode => CameraMode.Drag;

    private MapVisualizer visualizer;

    public override void StartState(object[] args)
    {
        visualizer = (MapVisualizer)args[0];
        visualizer.ShowVisualizer();
    }

    public override void Execute()
    {
    }

    public override void EndState()
    {
        visualizer.HideVisualizer();
    }

}
