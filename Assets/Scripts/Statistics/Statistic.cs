using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Statistic
{
    public delegate void ValueChanged(int value);
    public event ValueChanged OnValueChanged;

    private int value;
    public int Value
    {
        get
        {
            return value;
        }
        set
        {
            this.value = value;
            OnValueChanged?.Invoke(this.value);
        }
    }

    public string Name { get; private set; }

    public Statistic(string name)
    {
        this.Name = name;
    }

    public void Add(int amount)
    {
        Value = Value + amount;
    }

    public void Set(int amount)
    {
        Value = amount;
    }
}
