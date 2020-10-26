using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCHunger
{
    private int Value;

    public void EatFood(FoodItemInformation food)
    {
        int proposedValue = Value + food.Restoration;

        if (proposedValue >= 10)
            Value = 10;
        else
            Value = proposedValue;
    }
}
