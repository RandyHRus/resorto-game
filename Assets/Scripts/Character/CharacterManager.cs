using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{

    [SerializeField] private Sprite[] shirtSprites = null;
    [SerializeField] private Sprite[] hairSprites = null;
    [SerializeField] private Sprite[] eyesSprites = null;
    public Dictionary<CharacterPart, Sprite[]> partToSpritesOptionsMap;

    private static CharacterManager _instance;
    public static CharacterManager Instance { get { return _instance; } }
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
        DontDestroyOnLoad(transform.gameObject); //Character manager will be shared across scenes

        partToSpritesOptionsMap = new Dictionary<CharacterPart, Sprite[]>()
        {
            { CharacterPart.eyes,      eyesSprites },
            { CharacterPart.skin,      null },
            { CharacterPart.pants,     null },
            { CharacterPart.shoes,     null },
            { CharacterPart.shirt,     shirtSprites },
            { CharacterPart.hair,      hairSprites }
        };
    }

    public CharacterInformation CreateRandomCharacter()
    {
        CharacterInformation charInfo = new CharacterInformation();

        for (int i = 0; i < charInfo.partInformations.Length; i++) {
            charInfo.partInformations[i].Color = new Color32((byte)Random.Range(0, 255), (byte)Random.Range(0, 255), (byte)Random.Range(0, 255), 255);

            if (partToSpritesOptionsMap.TryGetValue((CharacterPart)i, out Sprite[] sprites))
            {
                if (sprites != null && sprites.Length >= 1)
                {
                    try
                    {
                        charInfo.partInformations[i].Sprite = sprites[Random.Range(0, sprites.Length)];
                    } catch (System.Exception e)
                    {
                        Debug.Log(e);
                    }
                }
            }
        }

        return charInfo;
    }
}

public enum CharacterPart
{
    eyes,
    skin,
    pants,
    shoes,
    shirt,
    hair
}