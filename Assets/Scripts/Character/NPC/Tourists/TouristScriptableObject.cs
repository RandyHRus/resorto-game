using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

[CreateAssetMenu(menuName = "Character/Tourist")]
public class TouristScriptableObject : CharacterScriptableObject
{
    [SerializeField] private TextAsset dialogueFile = null;
    public TextAsset DialogueFile => dialogueFile;
}