using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RandomNPCName
{
    private static string[] names =
    {
        "Adam",
        "Amy",
        "Andrew",
        "Annie",

        "Ben",
        "Caitlyn",
        "Dylan",
        "Einstein",
        "Franky",
        "Gene",
        "Harris",
        "Ian",
        "Jessica",
        "Krystal",
        "Lenny",
        "Mush",
        "Natsuki",
        "Omer",
        "Purin",
        "Queen",
        "Randy",
        "Sandy",
        "Tom",
        "Uni",
        "Venis",
        "Wonton",
        "Xavier",
        "Yen",
        "Zen"      
    };

    public static string GetRandomNPCName()
    {
        return names[Random.Range(0, names.Length)];
    }
}
