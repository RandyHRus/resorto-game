using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class DebugSideWindow : MonoBehaviour
{
    [SerializeField] private GameObject sideWindow = null;
    private List<Text> textComponents = new List<Text>();

    private static DebugSideWindow _instance;
    public static DebugSideWindow Instance { get { return _instance; } }
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

        sideWindow.SetActive(false);

        foreach (Transform t in sideWindow.transform.GetComponentsInChildren<Transform>())
        {
            Text text = t.GetComponent<Text>();
            if (text != null)
                textComponents.Add(text);
        }
    }

    private void Update()
    {
        #if UNITY_EDITOR
        if (Input.GetButtonDown("Dev"))
        {
            //Toggle
            sideWindow.SetActive(!sideWindow.activeInHierarchy);
        }

        if (sideWindow.activeInHierarchy)
        {
            PlayerState currentState = PlayerStateMachineManager.Instance.CurrentState;

            if (currentState is FollowNPCState followingNPC)
            {
                string[] toShow =
                    {
                        "State: " + followingNPC.NpcMono.CurrentState.ToString(),
                        "Schedule: " + followingNPC.NpcMono.CurrentSchedule.ToString()
                    };

                SetTexts(toShow);

            }
            else
            {
                SetTexts(new string[0]);
            }
        }
        #endif
    }

    private void SetTexts(string[] toShow)
    {
        for(int i = 0; i < textComponents.Count; i++)
        {
            if (i < toShow.Length)
            {
                textComponents[i].text = toShow[i];
            }
            else
            {
                textComponents[i].text = "";
            }
        }
    }
}
