using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItemInstance
{
    public virtual InventoryItemInformation ItemInformation { get; private set; }

    public InventoryItemInstance(InventoryItemInformation itemInformation)
    {
        this.ItemInformation = itemInformation;
    }

    public override bool Equals(object obj)
    {
        //Check for null and compare run-time types.
        if ((obj == null) || !this.GetType().Equals(obj.GetType()))
        {
            return false;   
        }
        else
        {
            return ((InventoryItemInstance)obj).ItemInformation == this.ItemInformation;
        }

    }

    public override int GetHashCode()
    {
        return ItemInformation.GetHashCode();
    }

    public virtual void ShowMessage(int count)
    {
        new ItemGainMessage(this, count);
    }
}
