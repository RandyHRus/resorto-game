using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using UnityEngine.AddressableAssets;

public class AssetReferenceConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var assetReference = value as AssetReference;
        var runTimeKey = assetReference.RuntimeKey;
        writer.WriteValue(runTimeKey);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        string runTimeKey = JValue.Load(reader).Value<string>();
        //Debug.Log(runTimeKey);
        return new AssetReference(runTimeKey);
    }

    public override bool CanRead
    {
        get { return true; }
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(AssetReference);
    }
}