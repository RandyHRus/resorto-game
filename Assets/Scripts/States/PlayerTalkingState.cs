using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTalkingState : MonoBehaviour, IPlayerState
{
    [SerializeField] private GameObject dialogueBox = null;
    [SerializeField] private Text dialogueText = null;
    [SerializeField] private Text nameText = null;

    private bool dialoguePlaying;
    private bool dialogueTyping;
    private int nextNode;
    private DialogueElement currentElement;
    private Coroutine typeCoroutine;
    private Dictionary<int, DialogueElement> dialogueMap;

    private float dialogueLettersPerSecond = 60f; //DialogueSpeed

    private static PlayerTalkingState _instance;
    public static PlayerTalkingState Instance { get { return _instance; } }

    public bool AllowMovement { get { return false; } }

    private void Awake()
    {
        //Singleton
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;
            }
        }

        dialogueBox.SetActive(false);
    }

    //args[0] = name
    //args[1] = dialogue
    public void StartState(object[] args)
    {
        if (dialoguePlaying)
            return;

        nameText.text = (string)args[0];
        Dialogue dialogue = (Dialogue)args[1];

        dialogueMap = new Dictionary<int, DialogueElement>();
        foreach (DialogueElement element in dialogue.DialogueElements)
        {
            dialogueMap.Add(element.Node, element);
        }

        dialogueBox.SetActive(true);
        dialoguePlaying = true;

        nextNode = 1;
        if (dialogueMap.TryGetValue(nextNode, out currentElement))
        {
            nextNode = currentElement.NextNode;
            typeCoroutine = StartCoroutine(TypeDialogueNode(currentElement));
        }
    }

    public void Execute()
    {
        if (Input.GetButtonDown("Submit"))
        {
            //Case when dialogue was already typing, show full dialogue node text
            if (dialogueTyping)
            {
                StopCoroutine(typeCoroutine);
                dialogueText.text = currentElement.Text;
                dialogueTyping = false;
            }
            //Case when no next node, End dialogue when NextNode does not exist in dialogue element
            else if (nextNode == 0)
            {
                EndDialogueAndState();
            }
            else if (dialogueMap.TryGetValue(nextNode, out currentElement))
            {
                nextNode = currentElement.NextNode;
                typeCoroutine = StartCoroutine(TypeDialogueNode(currentElement));
            }
            else
            {
                Debug.LogError("Error: Next node not found. Ending dialogue.");
                EndDialogueAndState();
            }
        }
    }

    private void EndDialogueAndState()
    {
        dialogueBox.SetActive(false);
        dialoguePlaying = false;
        PlayerStatesManager.Instance.TrySwitchState(DefaultState.Instance, null);
    }

    public bool TryEndState()
    {
        return !dialoguePlaying;
    }

    IEnumerator TypeDialogueNode(DialogueElement element)
    {
        dialogueTyping = true;

        dialogueText.text = "";

        char[] textArray = element.Text.ToCharArray();
        int index = 0;
        while (true)
        {
            int textCount = Mathf.RoundToInt(dialogueLettersPerSecond * Time.deltaTime);
            if (textCount == 0)
                textCount = 1;

            int startIndex = index;
            for (int i = startIndex; i < startIndex + textCount; i++)
            {
                if (i >= textArray.Length)
                {
                    goto EndType;
                }
                else
                {
                    dialogueText.text += textArray[i];
                    index++;
                }
            }
            yield return 0;
        }
        EndType:
        dialogueTyping = false;
    }
}
