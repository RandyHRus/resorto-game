using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Assertions;

public class TestSpace : MonoBehaviour
{
    public void Start()
    {
        ArrayHashSet<int> set = new ArrayHashSet<int>();
        set.Add(1);
        Assert.AreEqual<int>(1, set.Count);
        set.Add(2);
        Assert.AreEqual<int>(2, set.Count);
        set.Add(3);
        Assert.AreEqual<int>(3, set.Count);
        set.Add(4);
        Assert.AreEqual<int>(4, set.Count);
        set.Add(5);
        Assert.AreEqual<int>(5, set.Count);
        set.Remove(2);
        Assert.AreEqual<int>(4, set.Count);
    }
}
