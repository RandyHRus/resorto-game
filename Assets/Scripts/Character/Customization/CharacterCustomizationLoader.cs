using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCustomizationLoader : MonoBehaviour
{
    [SerializeField] private bool useImages = false;
    [ConditionalHide("useImages", false, true), SerializeField] private CharacterSpriteRenderers spriteRenderers = null;
    [ConditionalHide("useImages", false, false), SerializeField] private CharacterImageRenderers images = null;

    private SpriteRendererOrImage renderers;
    private Animator animator;

    public abstract class SpriteRendererOrImage
    {
        public abstract Spritable EyebrowRenderer { get; }
        public abstract Spritable HairFrontRenderer { get; }
        public abstract Spritable HairBackRenderer { get; }
        public abstract Spritable[] SkinColorRenderers { get; }
        public abstract Spritable BraRenderer { get; }
        public abstract Spritable ShirtFrontRenderer1 { get; }
        public abstract Spritable ShirtFrontRenderer2 { get; }
        public abstract Spritable ShirtBackRenderer1 { get; }
        public abstract Spritable ShirtBackRenderer2 { get; }
        public abstract Spritable HatFrontRenderer1 { get; }
        public abstract Spritable HatFrontRenderer2 { get; }
        public abstract Spritable HatBackRenderer1 { get; }
        public abstract Spritable HatBackRenderer2 { get; }
        public abstract Spritable[] PantsRenderers { get; }
        public abstract Spritable[] EyeRenderers { get; }
    }

    [System.Serializable]
    public class CharacterSpriteRenderers : SpriteRendererOrImage
    {
        [SerializeField] private SpritableSpriteRenderer eyebrowRenderer = null;
        public override Spritable EyebrowRenderer => eyebrowRenderer;

        [SerializeField] private SpritableSpriteRenderer hairFrontRenderer = null;
        public override Spritable HairFrontRenderer => hairFrontRenderer;

        [SerializeField] private SpritableSpriteRenderer hairBackRenderer = null;
        public override Spritable HairBackRenderer => hairBackRenderer;

        [SerializeField] private SpritableSpriteRenderer[] skinColorRenderers = null;
        public override Spritable[] SkinColorRenderers => skinColorRenderers;

        [SerializeField] private SpritableSpriteRenderer braRenderer = null;
        public override Spritable BraRenderer => braRenderer;

        [SerializeField] private SpritableSpriteRenderer shirtFrontRenderer1 = null;
        public override Spritable ShirtFrontRenderer1 => shirtFrontRenderer1;

        [SerializeField] private SpritableSpriteRenderer shirtFrontRenderer2 = null;
        public override Spritable ShirtFrontRenderer2 => shirtFrontRenderer2;

        [SerializeField] private SpritableSpriteRenderer shirtBackRenderer1 = null;
        public override Spritable ShirtBackRenderer1 => shirtBackRenderer1;

        [SerializeField] private SpritableSpriteRenderer shirtBackRenderer2 = null;
        public override Spritable ShirtBackRenderer2 => shirtBackRenderer2;

        [SerializeField] private SpritableSpriteRenderer hatFrontRenderer1 = null;
        public override Spritable HatFrontRenderer1 => hatFrontRenderer1;
        [SerializeField] private SpritableSpriteRenderer hatFrontRenderer2 = null;
        public override Spritable HatFrontRenderer2 => hatFrontRenderer2;
        [SerializeField] private SpritableSpriteRenderer hatBackRenderer1 = null;
        public override Spritable HatBackRenderer1 => hatBackRenderer1;
        [SerializeField] private SpritableSpriteRenderer hatBackRenderer2 = null;
        public override Spritable HatBackRenderer2 => hatBackRenderer2;

        [SerializeField] private SpritableSpriteRenderer[] pantsRenderers = null;
        public override Spritable[] PantsRenderers => pantsRenderers;

        [SerializeField] private SpritableSpriteRenderer[] eyeRenderers = null;
        public override Spritable[] EyeRenderers => eyeRenderers;
    }


    [System.Serializable]
    public class CharacterImageRenderers: SpriteRendererOrImage
    {
        [SerializeField] private SpritableImage eyebrowRenderer = null;
        public override Spritable EyebrowRenderer => eyebrowRenderer;

        [SerializeField] private SpritableImage hairFrontRenderer = null;
        public override Spritable HairFrontRenderer => hairFrontRenderer;

        [SerializeField] private SpritableImage hairBackRenderer = null;
        public override Spritable HairBackRenderer => hairBackRenderer;

        [SerializeField] private SpritableImage[] skinColorRenderers = null;
        public override Spritable[] SkinColorRenderers => skinColorRenderers;

        [SerializeField] private SpritableImage braRenderer = null;
        public override Spritable BraRenderer => braRenderer;

        [SerializeField] private SpritableImage shirtFrontRenderer1 = null;
        public override Spritable ShirtFrontRenderer1 => shirtFrontRenderer1;

        [SerializeField] private SpritableImage shirtFrontRenderer2 = null;
        public override Spritable ShirtFrontRenderer2 => shirtFrontRenderer2;

        [SerializeField] private SpritableImage shirtBackRenderer1 = null;
        public override Spritable ShirtBackRenderer1 => shirtBackRenderer1;

        [SerializeField] private SpritableImage shirtBackRenderer2 = null;
        public override Spritable ShirtBackRenderer2 => shirtBackRenderer2;

        [SerializeField] private SpritableImage hatFrontRenderer1 = null;
        public override Spritable HatFrontRenderer1 => hatFrontRenderer1;
        [SerializeField] private SpritableImage hatFrontRenderer2 = null;
        public override Spritable HatFrontRenderer2 => hatFrontRenderer2;
        [SerializeField] private SpritableImage hatBackRenderer1 = null;
        public override Spritable HatBackRenderer1 => hatBackRenderer1;
        [SerializeField] private SpritableImage hatBackRenderer2 = null;
        public override Spritable HatBackRenderer2 => hatBackRenderer2;

        [SerializeField] private SpritableImage[] pantsRenderers = null;
        public override Spritable[] PantsRenderers => pantsRenderers;

        [SerializeField] private SpritableImage[] eyeRenderers = null;
        public override Spritable[] EyeRenderers => eyeRenderers;
    }

    public abstract class Spritable
    {
        public abstract void SetColor(Color32 color);
        public abstract void SetSprite(Sprite sprite);
        public abstract void Enable(bool enable);
    }

    [System.Serializable]
    public class Spritable<T1> : Spritable
    {
        [SerializeField] private T1 spritable = default;

        public override void SetColor(Color32 color)
        {
            if (spritable is SpriteRenderer renderer)
            {
                renderer.color = color;
            }
            else if (spritable is Image image)
            {
                image.color = color;
            }
            else
            {
                throw new System.Exception("Not a spriteRenderer or image");
            }
        }

        public override void SetSprite(Sprite sprite)
        {
            if (spritable is SpriteRenderer renderer)
            {
                renderer.sprite = sprite;
            }
            else if (spritable is Image image)
            {
                image.sprite = sprite;
            }
            else
            {
                throw new System.Exception("Not a spriteRenderer or image");
            }
        }

        public override void Enable(bool enable)
        {
            if (spritable is SpriteRenderer renderer)
            {
                renderer.enabled = enable;
            }
            else if (spritable is Image image)
            {
                image.enabled = enable;
            }
            else
            {
                throw new System.Exception("Not a spriteRenderer or image");
            }
        }
    }

    [System.Serializable] public class SpritableImage : Spritable<Image> { }
    [System.Serializable] public class SpritableSpriteRenderer : Spritable<SpriteRenderer> { }


    private void Awake()
    {
        renderers = useImages ? (SpriteRendererOrImage)images: (SpriteRendererOrImage)spriteRenderers;
        animator = GetComponent<Animator>();
    }

    public void SetEyes(Color32 color)
    {
        for (int i = 0; i < renderers.EyeRenderers.Length; i++)
        {
            renderers.EyeRenderers[i].SetColor(color);
        }
    }

    public void SetSkin(Color32 color)
    {
        for (int i = 0; i < renderers.SkinColorRenderers.Length; i++)
        {
            renderers.SkinColorRenderers[i].SetColor(color);
        }
    }

    public void SetEyebrowsColor(Color32 color)
    {
        renderers.EyebrowRenderer.SetColor(color);
    }

    public void SetEyebrows(CharacterEyebrows eyebrows)
    {
        renderers.EyebrowRenderer.SetSprite(eyebrows.Sprite);
    }

    public void SetHair(CharacterHair hair)
    {
        renderers.HairFrontRenderer.Enable(hair != null && hair.FrontHair != null && animator.GetFloat("Vertical") < 0);
        renderers.HairBackRenderer.Enable(hair != null && hair.BackHair != null && animator.GetFloat("Vertical") > 0);

        renderers.HairFrontRenderer.SetSprite(hair?.FrontHair);
        renderers.HairBackRenderer.SetSprite(hair?.BackHair);
    }

    public void SetHairColor(Color32 color)
    {
        renderers.HairFrontRenderer.SetColor(color);
        renderers.HairBackRenderer.SetColor(color);
    }

    public void SetSex(CharacterSex sex)
    {
        renderers.BraRenderer.Enable(sex == CharacterSex.Female);
    }
    
    public void LoadHatInstance(HatItemInstance hat)
    {
        if (hat != null)
        {
            SetHat((CharacterHatItemInformation)hat.ItemInformation);

            if (hat.PrimaryColor != null)
                SetHatBaseColor((Color32)hat.PrimaryColor);

            if (hat.SecondaryColor != null)
                SetHatColorableColor((Color32)hat.SecondaryColor);
        }
        else
        {
            SetHat(null);
        }
    }

    public void SetHat(CharacterHatItemInformation hat)
    {
        renderers.HatFrontRenderer1.SetSprite(hat?.BaseSpritePair.SpriteFront);
        renderers.HatFrontRenderer1.Enable(hat != null && hat.BaseSpritePair.SpriteFront != null && animator.GetFloat("Vertical") < 0);

        renderers.HatFrontRenderer2.SetSprite(hat?.ColorableSpritePair.SpriteFront);
        renderers.HatFrontRenderer2.Enable(hat != null  && hat.ColorableSpritePair.SpriteFront != null && animator.GetFloat("Vertical") < 0);

        renderers.HatBackRenderer1.SetSprite(hat?.BaseSpritePair.SpriteBack);
        renderers.HatBackRenderer1.Enable(hat != null && hat.BaseSpritePair.SpriteBack != null && animator.GetFloat("Vertical") > 0);

        renderers.HatBackRenderer2.SetSprite(hat?.ColorableSpritePair.SpriteBack);
        renderers.HatBackRenderer2.Enable(hat != null && hat.ColorableSpritePair.SpriteBack != null && animator.GetFloat("Vertical") > 0);
    }

    public void SetHatBaseColor(Color32 color)
    {
        renderers.HatFrontRenderer1.SetColor(color);
        renderers.HatBackRenderer1.SetColor(color);
    }

    public void SetHatColorableColor(Color32 color)
    {
        renderers.HatFrontRenderer2.SetColor(color);
        renderers.HatBackRenderer2.SetColor(color);
    }

    public void LoadShirtInstance(ShirtItemInstance shirt)
    {
        if (shirt != null)
        {
            SetShirt((CharacterShirtItemInformation)shirt.ItemInformation);

            if (shirt.BaseColor != null)
                SetShirtBaseColor((Color32)shirt.BaseColor);

            if (shirt.ColorableColor != null) 
                SetShirtColorableColor((Color32)shirt.ColorableColor);
        }
        else
        {
            SetShirt(null);
        }
    }

    public void SetShirt(CharacterShirtItemInformation shirt)
    {
        renderers.ShirtFrontRenderer1.SetSprite(shirt?.BaseSpritePair.SpriteFront);
        renderers.ShirtFrontRenderer1.Enable(shirt != null && shirt.BaseSpritePair.SpriteFront != null && animator.GetFloat("Vertical") < 0);

        renderers.ShirtBackRenderer1.SetSprite(shirt?.BaseSpritePair.SpriteBack);
        renderers.ShirtBackRenderer1.Enable(shirt != null && shirt.BaseSpritePair.SpriteBack != null && animator.GetFloat("Vertical") > 0);

        renderers.ShirtFrontRenderer2.SetSprite(shirt?.ColorableSpritePair.SpriteFront);
        renderers.ShirtFrontRenderer2.Enable(shirt != null && shirt.ColorableSpritePair.SpriteFront != null && animator.GetFloat("Vertical") < 0);

        renderers.ShirtBackRenderer2.SetSprite(shirt?.ColorableSpritePair.SpriteBack);
        renderers.ShirtBackRenderer2.Enable(shirt != null && shirt.ColorableSpritePair.SpriteBack != null && animator.GetFloat("Vertical") > 0);
    }

    public void SetShirtBaseColor(Color32 color)
    {
        renderers.ShirtFrontRenderer1.SetColor(color);
        renderers.ShirtBackRenderer1.SetColor(color);
    }

    public void SetShirtColorableColor(Color32 color)
    {
        renderers.ShirtFrontRenderer2.SetColor(color);
        renderers.ShirtBackRenderer2.SetColor(color);
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
        for (int i = 0; i < renderers.PantsRenderers.Length; i++)
        {
            renderers.PantsRenderers[i].Enable(pants != null);
        }
    }

    public void SetPantsColor(Color32 color)
    {
        for (int i = 0; i < renderers.PantsRenderers.Length; i++)
        {
            renderers.PantsRenderers[i].SetColor(color);
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
