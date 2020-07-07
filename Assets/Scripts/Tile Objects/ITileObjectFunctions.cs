using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITileObjectFunctions
{
    void Initialize(ObjectOnTile objectData);

    void StepOn();

    void StepOff();

    void ClickInteract();
}
