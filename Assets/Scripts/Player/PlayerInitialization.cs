using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInitialization : MonoBehaviour
{
    private static PlayerInitialization _instance;
    public static PlayerInitialization Instance { get { return _instance; } }
    private void Awake()
    {
        //Singleton
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;
            }
        }

        IslandGenerationPipeline.IslandCompleted += SetInitialLocation;
    }

    private void Start()
    {
        //Load character
        GetComponent<CharacterCustomizationLoader>().LoadCustomization(PlayerCharacterCustomization.Customization);
    }

    //Moves player onto starting position
    private void SetInitialLocation(IslandStartingPosition startingPosition)
    {
        transform.position = startingPosition.ActualStartingPosition;
        GetComponent<PlayerMovement>().InitializeLayerAndDepth();
    }

}
