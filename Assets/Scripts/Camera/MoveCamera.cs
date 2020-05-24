using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    private GameObject player;

    void Start()
    {
        try {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        catch (System.Exception)
        {
            Debug.Log("object with tag \"Player\" was not found");
        }
    }

    void LateUpdate()
    {
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -100);
    }
}
