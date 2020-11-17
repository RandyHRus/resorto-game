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

        Assert.AreEqual(new InGameTime(20, 30, 1) + new InGameTime(1, 20, 0), new InGameTime(21, 50, 1));
        Assert.AreEqual(new InGameTime(23, 59, 1) + new InGameTime(0, 1, 0), new InGameTime(0, 0, 2));
        Assert.AreEqual(new InGameTime(15, 30, 59) + new InGameTime(3, 50, 5), new InGameTime(19, 20, 64));
        Assert.AreEqual(new InGameTime(23, 30, 59) + new InGameTime(3, 50, 5), new InGameTime(3, 20, 65));

        Assert.AreEqual(new InGameTime(20, 30, 1) - new InGameTime(1, 20, 0), new InGameTime(19, 10, 1));
        Assert.AreEqual(new InGameTime(0, 0, 1) - new InGameTime(0, 1, 0), new InGameTime(23, 59, 0));
        Assert.AreEqual(new InGameTime(2, 30, 0) - new InGameTime(2, 30, 1), new InGameTime(0, 0, -1));
        Assert.AreEqual(new InGameTime(3, 0, 2) - new InGameTime(4, 50, 1), new InGameTime(22, 10, 0));

        Assert.AreEqual(new InGameTime(1,  1) +  new InGameTime(3,4), new InGameTime(4, 5));
        Assert.AreEqual(new InGameTime(23, 50) + new InGameTime(0, 10), new InGameTime(0, 0));
        Assert.AreEqual(new InGameTime(23, 50) + new InGameTime(0, 9), new InGameTime(23, 59));
        Assert.AreEqual(new InGameTime(23, 50) + new InGameTime(23, 59), new InGameTime(23, 49));

        Assert.AreEqual(new InGameTime(23, 50) - new InGameTime(1, 50), new InGameTime(22, 0));
        Assert.AreEqual(new InGameTime(0, 0) - new InGameTime(0, 1), new InGameTime(23, 59));
        Assert.AreEqual(new InGameTime(1, 1) - new InGameTime(1, 1), new InGameTime(0, 0));

        Assert.AreEqual(new InGameTime(60), new InGameTime(1, 0, 0));
        Assert.AreEqual(new InGameTime(70), new InGameTime(1, 10, 0));
        Assert.AreEqual(new InGameTime(1440), new InGameTime(0, 0, 1));
        Assert.AreEqual(new InGameTime(1441), new InGameTime(0, 1, 1));
        Assert.AreEqual(new InGameTime(320), new InGameTime(5, 20, 0));

        Assert.AreEqual(new InGameTime(320) <= new InGameTime(320), true);
        Assert.AreEqual(new InGameTime(320) >= new InGameTime(320), true);
        Assert.AreEqual(new InGameTime(321) > new InGameTime(320), true);
        Assert.AreEqual(new InGameTime(20,30,1) > new InGameTime(20,30,0), true);
        Assert.AreEqual(new InGameTime(20, 30, 1) > new InGameTime(20, 30, 1), false);
        Assert.AreEqual(new InGameTime(21, 30, 1) > new InGameTime(20, 30, 1), true);
        Assert.AreEqual(new InGameTime(23, 31, 1) > new InGameTime(20, 30, 2), false);
        Assert.AreEqual(new InGameTime(0, 0, 0) > new InGameTime(0, 0, 0), false);
        Assert.AreEqual(new InGameTime(0, 1, 0) > new InGameTime(0, 0, 0), true);
        Assert.AreEqual(new InGameTime(0, 1, 0) < new InGameTime(0, 2, 0), true);
    }
}
