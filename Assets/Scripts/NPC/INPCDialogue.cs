using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface INPCDialogue
{
    string Name { get; set; }

    bool GetStandardDialogue(out Dialogue dialogue);
}
