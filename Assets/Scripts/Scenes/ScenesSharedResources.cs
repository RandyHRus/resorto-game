using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class ScenesSharedResources : MonoBehaviour
{
    [SerializeField] private CharacterScriptableObject defaultCharacter = null;
    public CharacterScriptableObject DefaultCharacter => defaultCharacter;

    [SerializeField] private AssetReference[] eyebrowsOptions = null;
    public AssetReference[] EyebrowsOptions => eyebrowsOptions;

    [SerializeField] private AssetReference[] hairOptions = null;
    public AssetReference[] HairOptions => hairOptions;

    [SerializeField] private AssetReference[] shirtOptions = null;
    public AssetReference[] ShirtOptions => shirtOptions;

    [SerializeField] private AssetReference pants = null;
    public AssetReference Pants => pants;

    private static ScenesSharedResources _instance;
    public static ScenesSharedResources Instance { get { return _instance; } }
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        DontDestroyOnLoad(gameObject);
    }
}
