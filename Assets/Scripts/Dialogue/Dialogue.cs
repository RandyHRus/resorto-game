using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Dialogue
{
    public List<DialogueElement> DialogueElements { get; set; }
}

[System.Serializable]
public class DialogueElement
{
    public int Node { get; set; }
    public string Text { get; set; }
    //public DialogueOption[] Options { get; set; }
    public int NextNode { get; set; }
}

[System.Serializable]
public class DialogueOption
{
    public string Text { get; set; }
    public int NextNode { get; set; }
}