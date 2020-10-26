using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITileObjectFunctions
{
    void Initialize(BuildOnTile buildData);

    void StepOn();

    void StepOff();
}
