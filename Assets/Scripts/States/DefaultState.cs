using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "States/Default")]
public class DefaultState : PlayerState
{
    private GameObject player = null;
    private Transform playerTransform;

    [SerializeField] private Sprite talkIndicatorSprite = null, entryIndicatorSprite = null;

    private GameObject interactIndicatorInstance = null;
    private Transform interactIndicatorTransform;
    private Image interactIndicatorImage;

    private float interactRange = 4f;


    public override void Initialize()
    {
        base.Initialize();

        interactIndicatorInstance = GameObject.FindGameObjectWithTag("Interact Indicator");

        interactIndicatorTransform = interactIndicatorInstance.transform;
        interactIndicatorImage = interactIndicatorInstance.GetComponent<Image>();
        interactIndicatorInstance.SetActive(false);

        player = GameObject.FindGameObjectWithTag("Player");
        playerTransform = player.transform;
    }

    public override bool AllowMovement { get { return true; } }

    public override void Execute()
    {
        //Check for nearby NPC's
        {
            //Find closest Interactable
            Transform tMin = null;

            Collider2D[] results = Physics2D.OverlapBoxAll(playerTransform.position, new Vector2(2.5f, 2.5f), 0f, 1 << LayerMask.NameToLayer("Interactable"));

            float minDist = Mathf.Infinity;
            Vector2 currentPos = playerTransform.position;
            foreach (Collider2D col in results)
            {
                Transform t = col.transform;

                float dist = Vector2.Distance(t.position, currentPos);
                if (dist < minDist)
                {
                    tMin = t;
                    minDist = dist;
                }
            }

            if (interactIndicatorInstance.activeSelf != (tMin != null))
                interactIndicatorInstance.SetActive(tMin != null);

            if (tMin != null) {
                switch (tMin.tag)
                {
                    case ("NPC"):

                        interactIndicatorImage.sprite = talkIndicatorSprite;
                        interactIndicatorTransform.position = new Vector2(tMin.position.x, tMin.position.y + 1.5f);

                        if (Input.GetButtonDown("Interact"))
                        {
                            INPCDialogue dialogueComponent = tMin.GetComponent<INPCDialogue>();

                            if (dialogueComponent.GetStandardDialogue(out Dialogue dialogue))
                            {
                                object[] args = new object[] { dialogueComponent.Name, dialogue };
                                PlayerStateMachine.Instance.TrySwitchState<PlayerTalkingState>(args);
                            }
                        }
                        break;

                    case ("Entry"):

                        interactIndicatorImage.sprite = entryIndicatorSprite;
                        interactIndicatorTransform.position = new Vector2(tMin.position.x, tMin.position.y + 1f);

                        if (Input.GetButtonDown("Interact"))
                        {
                            SceneTransitions.TransitionScene("A");
                        }

                        break;
                }
            }
        }

        //Click interact
        {
            if (CheckMouseOverUI.GetButtonDownAndNotOnUI("Secondary"))
            {
                Vector2 mousePos = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
                if (Vector2.Distance(mousePos, playerTransform.position) < interactRange)
                {
                    bool nonTileClickableFound = false;
                    {
                        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
                        if (hit.collider != null)
                        {
                            INonTileClickable component = hit.transform.GetComponentInChildren<INonTileClickable>();
                            if (component == null) component = hit.transform.GetComponentInParent<INonTileClickable>();
                            if (component != null)
                            {
                                component.NearbyAndOnClick();
                                nonTileClickableFound = true;
                            }
                        }
                    }
                    //Interact with tile objects
                    if (nonTileClickableFound == false)
                    {
                        Vector3Int mouseTilePos = TileInformationManager.Instance.GetMouseTile();
                        TileInformation mouseTileInfo = TileInformationManager.Instance.GetTileInformation(mouseTilePos);
                        if (mouseTileInfo != null)
                        {
                            mouseTileInfo.BuildsOnTile.ClickInteract();
                        }
                    }
                }
                else
                {
                    new WarningMessage("Too far away!");
                }
            }
        }
    }

    public override void StartState(object[] args)
    {
        //Nothing needed yet
    }

    public override bool TryEndState()
    {
        interactIndicatorInstance.SetActive(false);
        return true;
    }
}
