using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Newtonsoft.Json;

[System.Serializable]
public class TwoColorsCosmeticInstance : CosmeticItemInstance
{
    public override Color32? PrimaryColor => baseColor;
    public override Color32? SecondaryColor => topColor;

    [SerializeField, JsonProperty] private Color32 baseColor = default;
    [JsonIgnore] public Color32 BaseColor => baseColor;

    [SerializeField, JsonProperty] private Color32 topColor = default;
    [JsonIgnore] public Color32 TopColor => topColor;

    public TwoColorsCosmeticInstance(): base()
    {

    }

    public TwoColorsCosmeticInstance(AssetReference assetReference, Color32 baseColor, Color32 topColor): base(assetReference)
    {
        this.baseColor = baseColor;
        this.topColor = topColor;
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
            TwoColorsCosmeticInstance compare = (TwoColorsCosmeticInstance)obj;

            return (compare.GetItemInformation() == this.GetItemInformation() &&
                (this.BaseColor.Equals(compare.BaseColor)) &&
                (this.TopColor.Equals(compare.TopColor)));
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
