using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System;
using UnityEngine.ResourceManagement.AsyncOperations;

public static class AssetsManager
{
    //Addressible key to asset
    private static Dictionary<string, object> database = new Dictionary<string, object>();

    private static string[] labelsToLoad = 
        {
            "Inventory Item",
            "Region",
            "Interest",
            "Character Part"
        };

    public static T GetAsset<T>(AssetReference key)
    {
        if (!key.RuntimeKeyIsValid())
            throw new System.Exception("Invalid key!");

        string keyValue = key.RuntimeKey.ToString();

        if (database.TryGetValue(keyValue, out object asset))
            return (T)asset;
        else
            throw new System.Exception("No asset with that key!");
    }

    public static async System.Threading.Tasks.Task LoadAssetsAsync()
    {
        database.Clear();

        foreach (string label in labelsToLoad)
        {
            // Loading locations by label
            var labelOperation = Addressables.LoadResourceLocationsAsync(label);
            var labelLocations = await labelOperation.Task;

            // Loading assets and making dictionary of location and corresponding asset
            var tempLocationIdDatabase = new Dictionary<string, object>();
            var operationHandles = new List<AsyncOperationHandle<object>>();
            foreach (var labelLocation in labelLocations)
            {
                var asyncOperationHandle = Addressables.LoadAssetAsync<object>(labelLocation.PrimaryKey);
                operationHandles.Add(asyncOperationHandle);
                asyncOperationHandle.Completed += handle =>
                   tempLocationIdDatabase.Add(labelLocation.InternalId, handle.Result);
            }

            // Awaiting for all asset loadings to be completed
            foreach (var asyncOperationHandle in operationHandles)
                await asyncOperationHandle.Task;

            foreach (var resourceLocator in Addressables.ResourceLocators)
            {
                foreach (var key in resourceLocator.Keys)
                {
                    var hasLocation = resourceLocator.Locate(key, typeof(object), out var keyLocations);
                    if (!hasLocation)
                        continue;
                    if (keyLocations.Count == 0)
                        continue;
                    if (keyLocations.Count > 1)
                        continue;

                    var keyLocationId = keyLocations[0].InternalId;
                    var locationMatched = tempLocationIdDatabase.TryGetValue(keyLocationId, out var asset);
                    if (!locationMatched)
                        continue;

                    database.Add(key.ToString(), asset);
                }
            }
        }
    }
}
