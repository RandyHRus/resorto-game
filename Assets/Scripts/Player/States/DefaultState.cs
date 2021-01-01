using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "States/Player/Default")]
public class DefaultState : PlayerState
{
    private Transform playerTransform;

    [SerializeField] private Sprite talkIndicatorSprite = null, entryIndicatorSprite = null;

    private GameObject interactIndicatorInstance = null;
    private Transform interactIndicatorTransform;
    private Image interactIndicatorImage;

    private float interactRange = 4f;

    public override bool AllowMovement => true;
    public override bool AllowMouseDirectionChange => true;
    public override CameraMode CameraMode => CameraMode.Follow;

    public override void Initialize()
    {
        base.Initialize();

        interactIndicatorInstance = GameObject.FindGameObjectWithTag("Interact Indicator");

        interactIndicatorTransform = interactIndicatorInstance.transform;
        interactIndicatorImage = interactIndicatorInstance.GetComponent<Image>();
        interactIndicatorInstance.SetActive(false);

        playerTransform = ResourceManager.Instance.Player;
    }

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
                            NPCMonoBehaviour npc = tMin.GetComponent<NPCMonoBehaviour>();

                            Dialogue dialogue = npc.GetDialogue();
                            object[] args = new object[] { npc.NpcInformation.NpcName, dialogue };
                            PlayerStateMachineManager.Instance.SwitchState<PlayerTalkingState>(args);
                        }
                        break;

                    case ("Entry"):

                        interactIndicatorImage.sprite = entryIndicatorSprite;
                        interactIndicatorTransform.position = new Vector2(tMin.position.x, tMin.position.y + 1f);

                        if (Input.GetButtonDown("Interact"))
                        {
                            //TODO
                            //SceneTransitions.TransitionScene("A");
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
                        Vector2Int mouseTilePos = TileInformationManager.Instance.GetMouseTile();
                        if (TileInformationManager.Instance.TryGetTileInformation(mouseTilePos, out TileInformation mouseTileInfo))
                        {
                            mouseTileInfo.ClickInteract();
                        }
                    }
                }
                else
                {
                    new WarningMessage("Too far away!");
                }
            }
        }

        //Inventory
        if (Input.GetButtonDown("Inventory"))
        {
            if (InventoryManager.Instance.IsInventoryOpen)
                InventoryManager.Instance.TryHideInventory();
            else
                InventoryManager.Instance.ShowInventory();
        }
    }

    public override void StartState(object[] args)
    {
        UIManager.Instance.ReloadSavedUI();
    }

    public override void EndState()
    {
        interactIndicatorInstance.SetActive(false);
    }
}
