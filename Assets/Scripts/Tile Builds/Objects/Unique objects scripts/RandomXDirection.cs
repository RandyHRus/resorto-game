using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomXDirection : MonoBehaviour
{
    void Awake()
    {
        transform.localScale = new Vector3(Random.Range(0, 2) == 1 ? 1: -1, 1, 1);
    }
}
