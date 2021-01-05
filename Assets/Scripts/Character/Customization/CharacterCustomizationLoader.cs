using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using System;

public class CharacterCustomizationLoader : MonoBehaviour
{
    private CharacterCustomization currentLoadedCustomization = null;

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

    private void OnEyesColorChangedHandler(object[] args = null)
    {
        for (int i = 0; i < renderers.EyeRenderers.Length; i++)
        {
            renderers.EyeRenderers[i].SetColor(currentLoadedCustomization.EyesColor);
        }
    }

    private void OnSkinColorChangedHandler(object[] args = null)
    {
        for (int i = 0; i < renderers.SkinColorRenderers.Length; i++)
        {
            renderers.SkinColorRenderers[i].SetColor(currentLoadedCustomization.SkinColor);
        }
    }

    private void OnEyebrowsColorChangedHandler(object[] args = null)
    {
        renderers.EyebrowRenderer.SetColor(currentLoadedCustomization.EyebrowsColor);
    }

    private void OnEyebrowsChangedHandler(object[] args = null)
    {
        CharacterEyebrows eyebrows = AssetsManager.GetAsset<CharacterEyebrows>(currentLoadedCustomization.Eyebrows);
        renderers.EyebrowRenderer.SetSprite(eyebrows.Sprite);
    }

    private void OnHairChangedHandler(object[] args = null)
    {
        //renderers.HairFrontRenderer.Enable(hair != null && hair.FrontHair != null && animator.GetFloat("Vertical") <= 0);
        //renderers.HairBackRenderer.Enable(hair != null && hair.BackHair != null && animator.GetFloat("Vertical") > 0);

        CharacterHair hair = AssetsManager.GetAsset<CharacterHair>(currentLoadedCustomization.Hair);

        renderers.HairFrontRenderer.SetSprite(hair.FrontHair);
        renderers.HairBackRenderer.SetSprite(hair.BackHair);
    }

    private void OnHairColorChangedHandler(object[] args = null)
    {
        renderers.HairFrontRenderer.SetColor(currentLoadedCustomization.HairColor);
        renderers.HairBackRenderer.SetColor(currentLoadedCustomization.HairColor);
    }

    private void OnSexChangedHandler(object[] args = null)
    {
        renderers.BraRenderer.Enable(currentLoadedCustomization.Sex == CharacterSex.Female);
    }

    private void OnHatChangedHandler(object[] args = null)
    {
        CharacterHatItemInformation hat;
        if (currentLoadedCustomization.Hat != null)
        {
            hat = (CharacterHatItemInformation)currentLoadedCustomization.Hat.GetItemInformation();
        }
        else
        {
            hat = null;
        }
        //Sets hat
        {
            renderers.HatFrontRenderer1.SetSprite(hat?.BaseSpritePair.SpriteFront);
            renderers.HatFrontRenderer1.Enable(hat != null && animator.GetFloat("Vertical") <= 0);

            renderers.HatFrontRenderer2.SetSprite(hat?.TopSpritePair.SpriteFront);
            renderers.HatFrontRenderer2.Enable(hat != null && animator.GetFloat("Vertical") <= 0);

            renderers.HatBackRenderer1.SetSprite(hat?.BaseSpritePair.SpriteBack);
            renderers.HatBackRenderer1.Enable(hat != null && animator.GetFloat("Vertical") > 0);

            renderers.HatBackRenderer2.SetSprite(hat?.TopSpritePair.SpriteBack);
            renderers.HatBackRenderer2.Enable(hat != null && animator.GetFloat("Vertical") > 0);
        }

        if (currentLoadedCustomization.Hat != null)
        {
            //Set base color
            renderers.HatFrontRenderer1.SetColor(currentLoadedCustomization.Hat.BaseColor);
            renderers.HatBackRenderer1.SetColor(currentLoadedCustomization.Hat.BaseColor);

            //Set top color
            renderers.HatFrontRenderer2.SetColor(currentLoadedCustomization.Hat.TopColor);
            renderers.HatBackRenderer2.SetColor(currentLoadedCustomization.Hat.TopColor);
        }
    }

    private void OnShirtChangedHandler(object[] args = null)
    {
        CharacterShirtItemInformation shirt;
        if (currentLoadedCustomization.Shirt != null)
        {
            shirt = (CharacterShirtItemInformation)currentLoadedCustomization.Shirt.GetItemInformation();
        }
        else
        {
            shirt = null;
        }
        //Set shirt
        {
            renderers.ShirtFrontRenderer1.SetSprite(shirt?.BaseSpritePair.SpriteFront);
            renderers.ShirtFrontRenderer1.Enable(shirt != null && animator.GetFloat("Vertical") <= 0);

            renderers.ShirtFrontRenderer2.SetSprite(shirt?.TopSpritePair.SpriteFront);
            renderers.ShirtFrontRenderer2.Enable(shirt != null && animator.GetFloat("Vertical") <= 0);

            renderers.ShirtBackRenderer1.SetSprite(shirt?.BaseSpritePair.SpriteBack);
            renderers.ShirtBackRenderer1.Enable(shirt != null && animator.GetFloat("Vertical") > 0);

            renderers.ShirtBackRenderer2.SetSprite(shirt?.TopSpritePair.SpriteBack);
            renderers.ShirtBackRenderer2.Enable(shirt != null && animator.GetFloat("Vertical") > 0);
        }

        if (currentLoadedCustomization.Shirt != null)
        {
            //Set base color
            renderers.ShirtFrontRenderer1.SetColor(currentLoadedCustomization.Shirt.BaseColor);
            renderers.ShirtBackRenderer1.SetColor(currentLoadedCustomization.Shirt.BaseColor);

            //Set top color
            renderers.ShirtFrontRenderer2.SetColor(currentLoadedCustomization.Shirt.TopColor);
            renderers.ShirtBackRenderer2.SetColor(currentLoadedCustomization.Shirt.TopColor);
        }
    }

    private void OnPantsChangedHandler(object[] args = null)
    {
        // Enable/Disable pants renderers depending on if pants is null or not
        for (int i = 0; i < renderers.PantsRenderers.Length; i++)
        {
            renderers.PantsRenderers[i].Enable(currentLoadedCustomization.Pants != null);
        }

        if (currentLoadedCustomization.Pants != null)
        {
            //Set color
            for (int i = 0; i < renderers.PantsRenderers.Length; i++)
            {
                renderers.PantsRenderers[i].SetColor(currentLoadedCustomization.Pants.Color_);
            }
        }
    }

    public void LoadCustomization(CharacterCustomization customization)
    {
        if (currentLoadedCustomization != null)
        {
            UnsubscribeCurrent();
        }
        currentLoadedCustomization = customization;

        OnEyebrowsChangedHandler();
        OnEyebrowsColorChangedHandler();
        OnHairChangedHandler();
        OnHairColorChangedHandler();
        OnSkinColorChangedHandler();
        OnEyesColorChangedHandler();
        OnSexChangedHandler();
        OnHatChangedHandler();
        OnShirtChangedHandler();
        OnPantsChangedHandler();

        currentLoadedCustomization.CustomizationPartChangedEventManager.Subscribe("eyebrows",           OnEyebrowsChangedHandler);
        currentLoadedCustomization.CustomizationPartChangedEventManager.Subscribe("eyebrowsColor",      OnEyebrowsColorChangedHandler);
        currentLoadedCustomization.CustomizationPartChangedEventManager.Subscribe("hair",               OnHairChangedHandler);
        currentLoadedCustomization.CustomizationPartChangedEventManager.Subscribe("hairColor",          OnHairColorChangedHandler);
        currentLoadedCustomization.CustomizationPartChangedEventManager.Subscribe("skinColor",          OnSkinColorChangedHandler);
        currentLoadedCustomization.CustomizationPartChangedEventManager.Subscribe("eyesColor",          OnEyesColorChangedHandler);
        currentLoadedCustomization.CustomizationPartChangedEventManager.Subscribe("sex",                OnSexChangedHandler);
        currentLoadedCustomization.CustomizationPartChangedEventManager.Subscribe("hat",                OnHatChangedHandler);
        currentLoadedCustomization.CustomizationPartChangedEventManager.Subscribe("shirt",              OnShirtChangedHandler);
        currentLoadedCustomization.CustomizationPartChangedEventManager.Subscribe("pants",              OnPantsChangedHandler);
    }

    private void UnsubscribeCurrent()
    {
        currentLoadedCustomization.CustomizationPartChangedEventManager.Unsubscribe("eyebrows", OnEyebrowsChangedHandler);
        currentLoadedCustomization.CustomizationPartChangedEventManager.Unsubscribe("eyebrowsColor", OnEyebrowsColorChangedHandler);
        currentLoadedCustomization.CustomizationPartChangedEventManager.Unsubscribe("hair", OnHairChangedHandler);
        currentLoadedCustomization.CustomizationPartChangedEventManager.Unsubscribe("hairColor", OnHairColorChangedHandler);
        currentLoadedCustomization.CustomizationPartChangedEventManager.Unsubscribe("skinColor", OnSkinColorChangedHandler);
        currentLoadedCustomization.CustomizationPartChangedEventManager.Unsubscribe("eyesColor", OnEyesColorChangedHandler);
        currentLoadedCustomization.CustomizationPartChangedEventManager.Unsubscribe("sex", OnSexChangedHandler);
        currentLoadedCustomization.CustomizationPartChangedEventManager.Unsubscribe("hat", OnHatChangedHandler);
        currentLoadedCustomization.CustomizationPartChangedEventManager.Unsubscribe("shirt", OnShirtChangedHandler);
        currentLoadedCustomization.CustomizationPartChangedEventManager.Unsubscribe("pants", OnPantsChangedHandler);
    }

    private void OnDestroy()
    {
        if (currentLoadedCustomization != null)
        {
            UnsubscribeCurrent();
        }
    }
}
