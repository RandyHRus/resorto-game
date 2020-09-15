using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Fish")]
public class FishItemInformation : FoodItemInformation
{
    [SerializeField] private int millimetresLowerBound = 0;
    public int MillimetresLowerBound => millimetresLowerBound;

    [SerializeField] private int millimetresUpperBound = 0;
    public int MillimetresUpperBound => millimetresUpperBound;

    //Food is stackable but this is not
    public override bool Stackable { get { return false; } }
}
