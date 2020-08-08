using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CreateBuildState: PlayerState
{
    public override bool AllowMovement { get { return true; } }
}
