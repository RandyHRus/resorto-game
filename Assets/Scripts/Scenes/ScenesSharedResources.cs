using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenesSharedResources : MonoBehaviour
{
    [SerializeField] private CharacterScriptableObject defaultCharacter = null;
    public CharacterScriptableObject DefaultCharacter => defaultCharacter;

    [SerializeField] private CharacterEyebrows[] eyebrowsOptions = null;
    public CharacterEyebrows[] EyebrowsOptions => eyebrowsOptions;

    [SerializeField] private CharacterHair[] hairOptions = null;
    public CharacterHair[] HairOptions => hairOptions;

    [SerializeField] private CharacterShirtItemInformation[] shirtOptions = null;
    public CharacterShirtItemInformation[] ShirtOptions => shirtOptions;

    [SerializeField] private CharacterPantsItemInformation pants = null;
    public CharacterPantsItemInformation Pants => pants;

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
