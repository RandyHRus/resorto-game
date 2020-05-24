using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterInformation
{
    [SerializeField] private CharacterPartSpritePreDefined eyes = new CharacterPartSpritePreDefined();
    [SerializeField] private CharacterPartSpritePreDefined skin = new CharacterPartSpritePreDefined();
    [SerializeField] private CharacterPartSpritePreDefined pants = new CharacterPartSpritePreDefined();
    [SerializeField] private CharacterPartSpritePreDefined shoes = new CharacterPartSpritePreDefined();
    [SerializeField] private CharacterPartSpriteEditable shirt = new CharacterPartSpriteEditable();
    [SerializeField] private CharacterPartSpriteEditable hair = new CharacterPartSpriteEditable();

    public ICharacterPartInformation[] partInformations;

    public CharacterInformation()
    {
        partInformations = new ICharacterPartInformation[]
        {
            eyes,
            skin,
            pants,
            shoes,
            shirt,
            hair
        };
    }
}

public interface ICharacterPartInformation
{
    Color32 Color { get; set; }
    Sprite Sprite { get; set; }
}

[System.Serializable]
public class CharacterPartSpriteEditable: ICharacterPartInformation
{
    [SerializeField] private Color32 color;
    public Color32 Color {
        get
        {
            return color;
        }
        set {
            color = value;
        }
    }
    [SerializeField] private Sprite sprite;
    public Sprite Sprite
    {
        get
        {
            return sprite;
        }
        set
        {
            sprite = value;
        }
    }
}

[System.Serializable]
public class CharacterPartSpritePreDefined : ICharacterPartInformation
{
    [SerializeField] private Color32 color;
    public Color32 Color
    {
        get
        {
            return color;
        }
        set
        {
            color = value;
        }
    }
    public Sprite Sprite {
        get
        {
            return null;
        }
        set
        {
            throw new System.Exception("Cannot set sprite of character part with predefined sprite");
        }
    }
}