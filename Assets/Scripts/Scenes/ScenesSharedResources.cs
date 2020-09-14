using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenesSharedResources : MonoBehaviour
{
    [SerializeField] private CharacterScriptableObject defaultCharacter = null;
    public CharacterScriptableObject DefaultCharacter => defaultCharacter;


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
