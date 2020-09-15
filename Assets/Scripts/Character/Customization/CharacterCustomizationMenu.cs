using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class CharacterCustomizationMenu : MonoBehaviour
{
    [SerializeField] private CharacterEyebrows[] eyebrowsOptions = null;
    [SerializeField] private CharacterHair[] hairOptions = null;
    [SerializeField] private CharacterShirtItemInformation[] shirtOptions = null;
    [SerializeField] private CharacterPantsItemInformation pants = null;

    [SerializeField] private CharacterScriptableObject defaultCharacter = null;

    private ColorCustomizationPart currentColorPart = 0;
    private CharacterSex currentSex = 0;
    private int currentEyebrowIndex = 0, currentHairIndex = 0, currentShirtIndex = 0;
    private TurnDirection currentTurnDirection = 0;

    private List<Color32> commonSkinColors = new List<Color32>()
    {
        new Color32(141, 85, 36, 255),
        new Color32(198, 134, 66, 255),
        new Color32(224, 172, 105, 255),
        new Color32(241, 194, 125, 255),
        new Color32(255, 219, 172, 255)
    };


    //Customization Menu UI Components
    //*************************************************************
    [SerializeField] private GameObject colorPickerUI = null;
    [SerializeField] private ColorWheel colorWheel = null;
    [SerializeField] private CharacterCustomizationLoader customizationLoader = null;
    [SerializeField] private Transform characterTransform = null;
    [SerializeField] private Animator characterAnimator = null;

    [SerializeField] private GameObject eyebrowsSelectionText = null;
    private OutlinedText eyebrowSelectionOutlinedText;

    [SerializeField] private GameObject sexSelectionText = null;
    private OutlinedText sexSelectionOutlinedText;

    [SerializeField] private GameObject hairSelectionText = null;
    private OutlinedText hairSelectionOutlinedText;

    [SerializeField] private GameObject shirtSelectionText = null;
    private OutlinedText shirtSelectionOutlinedText;

    [EnumNamedArray(typeof(ColorCustomizationPart)), SerializeField] private Image[] colorPickerImage = null;

    [SerializeField] private GameObject nameInputDisplayText = null;
    [SerializeField] private Text fakeText = null;
    [SerializeField] private InputField nameInputField = null;
    private OutlinedText nameInputText;

    private bool caretMoved = false;


    //*************************************************************
    //*************************************************************

    private static CharacterCustomizationMenu _instance;
    public static CharacterCustomizationMenu Instance { get { return _instance; } }
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

        colorWheel.OnColorChanged += (Color32 color) => SetPartColor(currentColorPart, color);

        eyebrowSelectionOutlinedText = new OutlinedText(eyebrowsSelectionText);
        shirtSelectionOutlinedText = new OutlinedText(shirtSelectionText);
        hairSelectionOutlinedText = new OutlinedText(hairSelectionText);
        sexSelectionOutlinedText = new OutlinedText(sexSelectionText);

        LoadCustomization(defaultCharacter.CharacterCustomization);
    }

    private void Update()
    {
        //Close color picker when cancel button is pressed
        if (Input.GetButtonDown("Cancel"))
        {
            colorPickerUI.SetActive(false);
        }

        //Close color picker UI if mouse is not on color picker UI
        if (Input.GetButtonDown("Primary"))
        {
            List<RaycastResult> results = CheckMouseOverUI.GetEventSystemRaycastResults();
            bool mouseOnColorPicker = false;
            foreach (RaycastResult r in results)
            {
                if (r.gameObject == colorPickerUI)
                {
                    mouseOnColorPicker = true;
                    break;
                }
            }

            if (!mouseOnColorPicker)
            {
                colorPickerUI.SetActive(false);
            }
        }

        nameInputText = new OutlinedText(nameInputDisplayText);

        if (!caretMoved)
        {
            Transform caret = nameInputField.gameObject.transform.Find("Name input Input Caret");
            if (caret != null)
            {
                caret.SetAsLastSibling();
                caretMoved = true;
            }
        }
    }

    private void LateUpdate()
    {
        nameInputText.SetText(fakeText.text);
    }

    public void SwitchSex()
    {
        CharacterSex proposedSex = (currentSex == CharacterSex.Male) ? CharacterSex.Female : CharacterSex.Male;
        SetSex(proposedSex);
    }

    private void SetSex(CharacterSex sex)
    {
        currentSex = sex;
        customizationLoader.SetSex(sex);
        sexSelectionOutlinedText.SetText(sex == CharacterSex.Male ? "Male" : "Female");
    }

    public void ChangeEyebrowOption(bool next)
    {
        int nextEyebrowIndex = currentEyebrowIndex + (next ? 1 : -1);
        if (nextEyebrowIndex < 0)
            nextEyebrowIndex = eyebrowsOptions.Length - 1;
        else if (nextEyebrowIndex >= eyebrowsOptions.Length)
            nextEyebrowIndex = 0;

        SetEyebrowOption(nextEyebrowIndex);
    }

    private void SetEyebrowOption(int index)
    {
        currentEyebrowIndex = index;
        customizationLoader.SetEyebrows(eyebrowsOptions[currentEyebrowIndex]);
        eyebrowSelectionOutlinedText.SetText(currentEyebrowIndex.ToString());
    }

    public void ChangeHairOption(bool next)
    {
        int nextHairIndex = currentHairIndex + (next ? 1 : -1);
        if (nextHairIndex < 0)
            nextHairIndex = hairOptions.Length - 1;
        else if (nextHairIndex >= hairOptions.Length)
            nextHairIndex = 0;

        SetHairOption(nextHairIndex);
    }

    private void SetHairOption(int index)
    {
        currentHairIndex = index;
        customizationLoader.SetHair(hairOptions[currentHairIndex]);
        hairSelectionOutlinedText.SetText(currentHairIndex.ToString());
    }

    public void ChangeShirtOption(bool next)
    {
        int nextShirtIndex = currentShirtIndex + (next ? 1 : -1);
        if (nextShirtIndex < 0)
            nextShirtIndex = shirtOptions.Length - 1;
        else if (nextShirtIndex >= shirtOptions.Length)
            nextShirtIndex = 0;

        SetShirtOption(nextShirtIndex);
    }

    private void SetShirtOption(int index)
    {
        currentShirtIndex = index;

        colorPickerImage[(int)ColorCustomizationPart.ShirtBase].enabled = shirtOptions[currentShirtIndex] != null && shirtOptions[currentShirtIndex].BaseColorable;
        colorPickerImage[(int)ColorCustomizationPart.ShirtColorable].enabled = shirtOptions[currentShirtIndex] != null && shirtOptions[currentShirtIndex].HasColorable;

        customizationLoader.SetShirt(shirtOptions[currentShirtIndex]);
        shirtSelectionOutlinedText.SetText(currentShirtIndex.ToString());
    }

    public void SetPartColor(ColorCustomizationPart part, Color32 color)
    {
        colorPickerImage[(int)part].color = color;
        
        switch (part)
        {
            case (ColorCustomizationPart.Eyebrows):
                customizationLoader.SetEyebrowsColor(color);
                break;
            case (ColorCustomizationPart.Eyes):
                customizationLoader.SetEyes(color);
                break;
            case (ColorCustomizationPart.Hair):
                customizationLoader.SetHairColor(color);
                break;
            case (ColorCustomizationPart.Pants):
                customizationLoader.SetPantsColor(colorPickerImage[(int)ColorCustomizationPart.Pants].color);
                break;
            case (ColorCustomizationPart.ShirtBase):
                customizationLoader.SetShirtBaseColor(colorPickerImage[(int)ColorCustomizationPart.ShirtBase].color);
                break;
            case (ColorCustomizationPart.ShirtColorable):
                customizationLoader.SetShirtColorableColor(colorPickerImage[(int)ColorCustomizationPart.ShirtColorable].color);
                break;
            case (ColorCustomizationPart.Skin):
                customizationLoader.SetSkin(colorPickerImage[(int)ColorCustomizationPart.Skin].color);
                break;
            default:
                throw new System.NotImplementedException();
        }
    }

    public void OnColorPickerClick(int option)
    {
        colorPickerUI.SetActive(true);
        colorPickerUI.transform.position = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector2(1.2f, 0);

        currentColorPart = (ColorCustomizationPart)option;
        Color32 proposedColor = colorPickerImage[option].color;
        colorWheel.MovePointAndSliderToColor(proposedColor);
    }

    public void RandomizeButtonPressed()
    {
        LoadCustomization(CreateRandomCharacter());
    }

    public CharacterCustomization CreateRandomCharacter()
    {
        Color32 GetRandomColor()
        {
            return new Color32((byte)Random.Range(0, 255), (byte)Random.Range(0, 255), (byte)Random.Range(0, 255), 255);
        }

        CharacterEyebrows eyebrows = eyebrowsOptions[Random.Range(0, eyebrowsOptions.Length)];
        Color32 eyebrowsColor = GetRandomColor();

        CharacterHair hair = hairOptions[Random.Range(0, hairOptions.Length)];
        Color32 hairColor = GetRandomColor();

        Color32 skinColor;
        //Most times, pick a common skin color
        if (Random.Range(0, 10) < 8)
        {
            skinColor = commonSkinColors[Random.Range(0, commonSkinColors.Count)];
        }
        else
        {
            skinColor = GetRandomColor();
        }

        Color32 eyesColor = GetRandomColor();

        PantsItemInstance pantsInstance = new PantsItemInstance(pants, GetRandomColor());

        ShirtItemInstance shirtItem = new ShirtItemInstance(shirtOptions[Random.Range(0, shirtOptions.Length)], GetRandomColor(), GetRandomColor());

        //Sex will not be randomized
        return new CharacterCustomization("", eyebrows, eyebrowsColor, hair, hairColor, skinColor, eyesColor, currentSex, null, shirtItem, pantsInstance);
    }

    private void LoadCustomization(CharacterCustomization customization)
    {
        SetEyebrowOption(System.Array.IndexOf(eyebrowsOptions, customization.Eyebrows));
        SetHairOption(System.Array.IndexOf(hairOptions, customization.Hair));
        SetShirtOption(System.Array.IndexOf(shirtOptions, customization.Shirt?.ItemInformation));

        SetSex(customization.Sex);

        SetPartColor(ColorCustomizationPart.Eyebrows, customization.EyebrowsColor);
        SetPartColor(ColorCustomizationPart.Eyes,     customization.EyesColor);
        SetPartColor(ColorCustomizationPart.Hair,     customization.HairColor);

        if (customization.Pants != null)
        {
            SetPartColor(ColorCustomizationPart.Pants, customization.Pants.Color_);
        }

        if (customization.Shirt != null)
        {
            if (customization.Shirt.BaseColor != null)
                SetPartColor(ColorCustomizationPart.ShirtBase, (Color32)customization.Shirt.BaseColor);

            if (customization.Shirt.ColorableColor != null)
                SetPartColor(ColorCustomizationPart.ShirtColorable, (Color32)customization.Shirt.ColorableColor);
        }

        SetPartColor(ColorCustomizationPart.Skin,     customization.SkinColor);
    }

    public void Finish()
    {
        Color32 shirtBaseColor = colorPickerImage[(int)ColorCustomizationPart.ShirtBase].color;
        Color shirtColorableColor = colorPickerImage[(int)ColorCustomizationPart.ShirtColorable].color;

        PlayerCharacterCustomization.Customization = new CharacterCustomization(
            nameInputField.text,
            eyebrowsOptions[currentEyebrowIndex], 
            colorPickerImage[(int)ColorCustomizationPart.Eyebrows].color,
            hairOptions[currentHairIndex], colorPickerImage[(int)ColorCustomizationPart.Hair].color, 
            colorPickerImage[(int)ColorCustomizationPart.Skin].color,
            colorPickerImage[(int)ColorCustomizationPart.Eyes].color, currentSex, 
            null, 
            new ShirtItemInstance(shirtOptions[currentShirtIndex], shirtBaseColor, shirtColorableColor), 
            new PantsItemInstance(pants, colorPickerImage[(int)ColorCustomizationPart.Pants].color));

        SceneManager.LoadScene("Loading");
    }

    public void TurnCW()
    {
        currentTurnDirection++;
        if ((int)currentTurnDirection >= System.Enum.GetNames(typeof(TurnDirection)).Length)
        {
            currentTurnDirection = 0;
        }
        SetTurnDirection();
    }

    public void TurnCCW()
    {
        currentTurnDirection--;
        if ((int)currentTurnDirection < 0)
        {
            currentTurnDirection = (TurnDirection)(System.Enum.GetNames(typeof(TurnDirection)).Length - 1);
        }
        SetTurnDirection();
    }

    private void SetTurnDirection()
    {
        switch (currentTurnDirection)
        {
            case (TurnDirection.FrontRight):
                characterTransform.localScale = new Vector3(50, 50, 1);
                characterAnimator.SetBool("FaceFront", true);
                break;
            case (TurnDirection.FrontLeft):
                characterTransform.localScale = new Vector3(-50, 50, 1);
                characterAnimator.SetBool("FaceFront", true);
                break;
            case (TurnDirection.BackLeft):
                characterTransform.localScale = new Vector3(-50, 50, 1);
                characterAnimator.SetBool("FaceFront", false);
                break;
            case (TurnDirection.BackRight):
                characterTransform.localScale = new Vector3(50, 50, 1);
                characterAnimator.SetBool("FaceFront", false);
                break;
            default:
                throw new System.NotImplementedException();
        }
    }

    private enum TurnDirection
    {
        FrontRight,
        FrontLeft,
        BackLeft,
        BackRight
    }
}

public enum ColorCustomizationPart
{
    Hair,
    Skin,
    Pants,
    Eyebrows,
    Eyes,
    ShirtBase,
    ShirtColorable
}