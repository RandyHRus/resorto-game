using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WildlifeInformation: ScriptableObject
{
    [SerializeField] private string _name = "";
    public string Name => _name;

    public abstract bool TrySpawn(Vector2 pos, out WildlifeBehaviour behaviourScript);
}
