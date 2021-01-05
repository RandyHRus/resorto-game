using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;

public class LoadingScene : MonoBehaviour
{
    public static bool assetsLoaded = false;
    public static string sceneName = "Character Customization";

    private async void Start()
    {
        if (!assetsLoaded)
        {
            await AssetsManager.LoadAssetsAsync();
            Debug.Log("Done loading assets");
            assetsLoaded = true;
        }
        SceneManager.LoadSceneAsync(sceneName);

        //yield return new WaitForSeconds(0.3f);
    }
}
