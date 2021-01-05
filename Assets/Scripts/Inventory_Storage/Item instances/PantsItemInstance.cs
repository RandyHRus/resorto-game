using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Newtonsoft.Json;

[System.Serializable]
public class PantsItemInstance : CosmeticItemInstance
{
    public override Color32? PrimaryColor => color;
    public override Color32? SecondaryColor => null;

    [SerializeField, JsonProperty] private Color32 color = Color.white;
    [JsonIgnore] public Color32 Color_ => color;

    public PantsItemInstance(AssetReference itemAsset, Color32 color) : base(itemAsset)
    {
        this.color = color;
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
            return (((PantsItemInstance)obj).GetItemInformation() == this.GetItemInformation() &&
                this.color.Equals(((PantsItemInstance)obj).color));
        }

    }

    public override int GetHashCode()
    {
        /*Since I cant use mutable fields in hash code, this should be fine
         * as long as I dont use it in a hash table
        */
        return GetItemInformation().GetHashCode();
    }
}
