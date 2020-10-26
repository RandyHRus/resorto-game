using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Overrides the tourist dialogue type for a state
public interface ITouristStateDialogue
{
    TouristDialogueType GetTouristDialogueType();
}
