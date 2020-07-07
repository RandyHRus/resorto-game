using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandGenerationException : Exception
{
    public IslandGenerationException()
    {
    }

    public IslandGenerationException(string message)
        : base(message)
    {
    }

    public IslandGenerationException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
