using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using UnityEngine.AddressableAssets;

public class AddressableConverter //: JsonConverter
{
    /*
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var asset = value as AssetReference;
        string assetPath = asset.RuntimeKey;
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        throw new NotImplementedException("Unnecessary because CanRead is false. The type will skip the converter.");
    }

    public override bool CanRead
    {
        get { return false; }
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(AssetReference);
    }
    */
}
