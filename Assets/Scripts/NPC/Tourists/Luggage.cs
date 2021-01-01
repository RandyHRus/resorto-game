using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Luggage : MonoBehaviour
{
    [SerializeField] private Sprite[] luggageVariants = null;

    void Awake()
    {
        GetComponent<SpriteRenderer>().sprite = luggageVariants[Random.Range(0, luggageVariants.Length)];
    }
}
