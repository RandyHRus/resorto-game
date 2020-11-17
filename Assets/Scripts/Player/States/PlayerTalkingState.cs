using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "States/Player/Player talking")]
public class PlayerTalkingState : PlayerState
{
    private GameObject dialogueBox = null;
    private OutlinedText dialogueText = null;
    private OutlinedText nameText = null;

    private bool dialogueTyping;
    private int nextNode;
    private DialogueElement currentElement;
    private Coroutine typeCoroutine;
    private Dictionary<int, DialogueElement> dialogueMap;

    private readonly float dialogueLettersPerSecond = 60f; //DialogueSpeed

    public override bool AllowMovement => false;
    public override bool AllowMouseDirectionChange => false;
    public override CameraMode CameraMode => CameraMode.Follow;

    public override void Initialize()
    {
        base.Initialize();

        dialogueBox = ResourceManager.Instance.DialogueBoxInstance;
        foreach (Transform t in dialogueBox.transform)
        {
            if (t.tag == "Name Field")
            {
                nameText = new OutlinedText(t.gameObject);
            }
            else if (t.tag == "Text Field")
            {
                dialogueText = new OutlinedText(t.gameObject);
            }
        }
        dialogueBox.SetActive(false);
    }

    //args[0] = name
    //args[1] = dialogue
    public override void StartState(object[] args)
    {
        //if (dialoguePlaying)
        //    return;
        PauseManager.Instance.PauseGame(true);

        nameText.SetText((string)args[0]);
        Dialogue dialogue = (Dialogue)args[1];

        dialogueMap = new Dictionary<int, DialogueElement>();
        foreach (DialogueElement element in dialogue.DialogueElements)
        {
            dialogueMap.Add(element.Node, element);
        }

        dialogueBox.SetActive(true);

        nextNode = 1;
        if (dialogueMap.TryGetValue(nextNode, out currentElement))
        {
            nextNode = currentElement.NextNode;
            typeCoroutine = Coroutines.Instance.StartCoroutine(TypeDialogueNode(currentElement));
        }
    }

    public override void Execute()
    {
        if (Input.GetButtonDown("Submit"))
        {
            //Case when dialogue was already typing, show full dialogue node text
            if (dialogueTyping)
            {
                Coroutines.Instance.StopCoroutine(typeCoroutine);
                dialogueText.SetText(currentElement.Text);
                dialogueTyping = false;
            }
            //Case when no next node, End dialogue when NextNode does not exist in dialogue element
            else if (nextNode == 0)
            {
                PlayerStateMachineManager.Instance.SwitchState<DefaultState>();
            }
            else if (dialogueMap.TryGetValue(nextNode, out currentElement))
            {
                nextNode = currentElement.NextNode;
                typeCoroutine = Coroutines.Instance.StartCoroutine(TypeDialogueNode(currentElement));
            }
            else
            {
                Debug.LogError("Error: Next node not found. Ending dialogue.");
                PlayerStateMachineManager.Instance.SwitchState<DefaultState>();
            }
        }
    }

    public override void EndState()
    {
        dialogueBox.SetActive(false);
        PauseManager.Instance.UnPauseGame();
    }

    IEnumerator TypeDialogueNode(DialogueElement element)
    {
        dialogueTyping = true;

        string currentText = "";
        dialogueText.SetText(currentText);

        char[] textArray = element.Text.ToCharArray();
        int index = 0;
        while (true)
        {
            int textCount = Mathf.RoundToInt(dialogueLettersPerSecond * Time.unscaledDeltaTime);
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
                    currentText += textArray[i];
                    dialogueText.SetText(currentText);
                    index++;
                }
            }
            yield return 0;
        }
        EndType:
        dialogueTyping = false;
    }
}
