using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCustomizationLoader : MonoBehaviour
{
    [SerializeField] private SpriteRenderer eyebrowRenderer = null;

    [SerializeField] private SpriteRenderer hairFrontRenderer = null;
    [SerializeField] private SpriteRenderer hairBackRenderer = null;

    [SerializeField] private SpriteRenderer[] skinColorRenderers = null;

    [SerializeField] private SpriteRenderer braRenderer = null;

    [SerializeField] private SpriteRenderer shirtFrontRenderer1 = null;
    [SerializeField] private SpriteRenderer shirtFrontRenderer2 = null;
    [SerializeField] private SpriteRenderer shirtBackRenderer1 = null;
    [SerializeField] private SpriteRenderer shirtBackRenderer2 = null;

    [SerializeField] private SpriteRenderer hatFrontRenderer1 = null;
    [SerializeField] private SpriteRenderer hatFrontRenderer2 = null;
    [SerializeField] private SpriteRenderer hatBackRenderer1 = null;
    [SerializeField] private SpriteRenderer hatBackRenderer2 = null;

    [SerializeField] private SpriteRenderer[] pantsRenderers = null;

    [SerializeField] private SpriteRenderer[] eyeRenderers = null;

    public void SetEyes(Color32 color)
    {
        for (int i = 0; i < eyeRenderers.Length; i++)
        {
            eyeRenderers[i].color = color;
        }
    }

    public void SetSkin(Color32 color)
    {
        for (int i = 0; i < skinColorRenderers.Length; i++)
        {
            skinColorRenderers[i].color = color;
        }
    }

    public void SetEyebrowsColor(Color32 color)
    {
        eyebrowRenderer.color = color;
    }

    public void SetEyebrows(CharacterEyebrows eyebrows)
    {
        eyebrowRenderer.sprite = eyebrows.Sprite;
    }

    public void SetHair(CharacterHair hair)
    {
        hairFrontRenderer.sprite = hair?.FrontHair;
        hairBackRenderer.sprite = hair?.BackHair;
    }

    public void SetHairColor(Color32 color)
    {
        hairFrontRenderer.color = color;
        hairBackRenderer.color = color;
    }

    public void SetSex(CharacterSex sex)
    {
        braRenderer.enabled = (sex == CharacterSex.Female);
    }
    
    public void LoadHatInstance(HatItemInstance hat)
    {
        if (hat != null)
        {
            SetHat((CharacterHatItemInformation)hat.ItemInformation);
            SetHatBaseColor(hat.BaseColor);
            SetHatColorableColor(hat.ColorableColor);
        }
        else
        {
            SetHat(null);
        }
    }

    public void SetHat(CharacterHatItemInformation hat)
    {
        hatFrontRenderer1.sprite = hat?.BaseSpritePair.SpriteFront;
        hatFrontRenderer2.sprite = hat?.ColorableSpritePair.SpriteFront;
        hatBackRenderer1.sprite = hat?.BaseSpritePair.SpriteBack;
        hatBackRenderer2.sprite = hat?.ColorableSpritePair.SpriteBack;
    }

    public void SetHatBaseColor(Color32 color)
    {
        hatFrontRenderer1.color = color;
        hatBackRenderer1.color = color;
    }

    public void SetHatColorableColor(Color32 color)
    {
        hatFrontRenderer2.color = color;
        hatBackRenderer2.color = color;
    }

    public void LoadShirtInstance(ShirtItemInstance shirt)
    {
        if (shirt != null)
        {
            SetShirt((CharacterShirtItemInformation)shirt.ItemInformation);
            SetShirtBaseColor(shirt.BaseColor);
            SetShirtColorableColor(shirt.ColorableColor);
        }
        else
        {
            SetShirt(null);
        }
    }

    public void SetShirt(CharacterShirtItemInformation shirt)
    {
        shirtFrontRenderer1.sprite = shirt?.BaseSpritePair.SpriteFront;
        shirtBackRenderer1.sprite = shirt?.BaseSpritePair.SpriteBack;
        shirtFrontRenderer2.sprite = shirt?.ColorableSpritePair.SpriteFront;
        shirtBackRenderer2.sprite = shirt?.ColorableSpritePair.SpriteBack;
    }

    public void SetShirtBaseColor(Color32 color)
    {
        shirtFrontRenderer1.color = color;
        shirtBackRenderer1.color = color;
    }

    public void SetShirtColorableColor(Color32 color)
    {
        shirtFrontRenderer2.color = color;
        shirtBackRenderer2.color = color;
    }

    public void LoadPantsInstance(PantsItemInstance pants)
    {
        if (pants != null)
        {
            SetPants((CharacterPantsItemInformation)pants.ItemInformation);
            SetPantsColor(pants.Color_);
        }
        else
        {
            SetPants(null);
        }
    }

    public void SetPants(CharacterPantsItemInformation pants)
    {
        // Little weird with pants because we only have 1 type,
        // Enable/Disable pants renderers depending on if pants is null or not
        for (int i = 0; i < pantsRenderers.Length; i++)
        {
            pantsRenderers[i].enabled = (pants != null);
        }
    }

    public void SetPantsColor(Color32 color)
    {
        for (int i = 0; i < pantsRenderers.Length; i++)
        {
            pantsRenderers[i].color = color;
        }
    } 

    public void LoadCustomization(CharacterCustomization customization)
    {
        SetEyes(customization.EyesColor);
        SetSkin(customization.SkinColor);
        SetEyebrows(customization.Eyebrows);
        SetEyebrowsColor(customization.EyebrowsColor);
        SetHair(customization.Hair);
        SetHairColor(customization.HairColor);
        SetSex(customization.Sex);

        LoadHatInstance(customization.Hat);
        LoadShirtInstance(customization.Shirt);
        LoadPantsInstance(customization.Pants);
    }
}
