using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Newtonsoft.Json;

public class InventoryItemInstance
{
    [SerializeField, JsonProperty, JsonConverter(typeof(AssetReferenceConverter))] private AssetReference itemInformationAsset = null;

    [JsonIgnore] public AssetReference ItemInformationAsset => itemInformationAsset;

    private InventoryItemInformation cachedItemInformation = null;
    public InventoryItemInformation GetItemInformation() 
    {
        if (cachedItemInformation == null)
        {
            cachedItemInformation = AssetsManager.GetAsset<InventoryItemInformation>(itemInformationAsset);
        }

        if (cachedItemInformation == null)
            throw new System.Exception("Something went wrong");

        return cachedItemInformation;
    }

    //Default constructor for JSON desealization
    public InventoryItemInstance()
    {

    }

    public InventoryItemInstance(AssetReference itemInformationAsset)
    {
        this.itemInformationAsset = itemInformationAsset;
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
            return ((InventoryItemInstance)obj).GetItemInformation() == this.GetItemInformation();
        }
    }

    public override int GetHashCode()
    {
        return GetItemInformation().GetHashCode();
    }

    public virtual void ShowMessage(int count)
    {
        new ItemGainMessage(this, count);
    }
}
