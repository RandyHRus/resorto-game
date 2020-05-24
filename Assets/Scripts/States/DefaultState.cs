using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultState : MonoBehaviour, IPlayerState
{
    [SerializeField] private GameObject player = null;
    private Transform playerTransform;

    private ContactFilter2D npcFilter;
    [SerializeField] private GameObject talkIndicatorInstance = null;
    private Transform talkIndicatorTransform;


    private static DefaultState _instance;
    public static DefaultState Instance { get { return _instance; } }
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
        playerTransform = player.transform;

        npcFilter = new ContactFilter2D();
        npcFilter.useTriggers = false;
        npcFilter.SetLayerMask(1 << LayerMask.NameToLayer("NPC"));
        npcFilter.useLayerMask = true;

        talkIndicatorTransform = talkIndicatorInstance.transform;
        talkIndicatorInstance.SetActive(false);
    }

    public bool AllowMovement { get { return true; } }

    public void Execute()
    {
        //Check for nearby NPC's
        {
            //Find closest NPC
            Transform tMin = null;

            List<Collider2D> results = new List<Collider2D>();
            Physics2D.OverlapBox(playerTransform.position, new Vector2(2.5f, 2.5f), 0f, npcFilter, results);

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

            if (talkIndicatorInstance.activeSelf != (tMin != null))
                talkIndicatorInstance.SetActive(tMin != null);

            if (tMin != null)
            {
                talkIndicatorTransform.position = new Vector2(tMin.position.x, tMin.position.y + 1.5f);

                //Try to start dialogue
                if (Input.GetButtonDown("Interact"))
                {
                    INPCDialogue dialogueComponent = tMin.GetComponent<INPCDialogue>();

                    if (dialogueComponent.GetStandardDialogue(out Dialogue dialogue))
                    {
                        object[] args = new object[] { dialogueComponent.Name, dialogue };
                        PlayerStatesManager.Instance.TrySwitchState(PlayerTalkingState.Instance, args);
                    }
                }
            }
        }
    }

    public void StartState(object[] args)
    {
        //Nothing needed yet
    }

    public bool TryEndState()
    {
        talkIndicatorInstance.SetActive(false);
        return true;
    }
}
