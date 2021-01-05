using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.AddressableAssets;

public class CharacterCustomizationMenu : MonoBehaviour
{
    private ColorCustomizationPart currentColorPart = 0;
    private CharacterSex currentSex = CharacterSex.Female;
    private int currentEyebrowIndex = 0, currentHairIndex = 0, currentShirtIndex = 0;
    private TurnDirection currentTurnDirection = 0;

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

    private CharacterCustomization loadedCustomization;

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

        eyebrowSelectionOutlinedText = new OutlinedText(eyebrowsSelectionText);
        shirtSelectionOutlinedText = new OutlinedText(shirtSelectionText);
        hairSelectionOutlinedText = new OutlinedText(hairSelectionText);
        sexSelectionOutlinedText = new OutlinedText(sexSelectionText);

        CharacterCustomization defaultCustomization = CharacterCustomization.Copy(ScenesSharedResources.Instance.DefaultCharacter.CharacterCustomization);
        customizationLoader.LoadCustomization(defaultCustomization);
        loadedCustomization = defaultCustomization;
        LoadCustomization(defaultCustomization);
    }

    private void OnEnable()
    {
        colorWheel.OnColorChanged += OnColorWheelChangedHandler;
    }

    private void OnDisable()
    {
        colorWheel.OnColorChanged -= OnColorWheelChangedHandler;
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

    private void OnColorWheelChangedHandler(Color32 color)
    {
        SetPartColor(currentColorPart, color);
    }

    public void SwitchSex()
    {
        CharacterSex proposedSex = (currentSex == CharacterSex.Male) ? CharacterSex.Female : CharacterSex.Male;
        SetSex(proposedSex);
    }

    private void SetSex(CharacterSex sex)
    {
        currentSex = sex;
        loadedCustomization.Sex = sex;
        sexSelectionOutlinedText.SetText(sex == CharacterSex.Male ? "Male" : "Female");
    }

    public void ChangeEyebrowOption(bool next)
    {
        int nextEyebrowIndex = currentEyebrowIndex + (next ? 1 : -1);
        if (nextEyebrowIndex < 0)
            nextEyebrowIndex = ScenesSharedResources.Instance.EyebrowsOptions.Length - 1;
        else if (nextEyebrowIndex >= ScenesSharedResources.Instance.EyebrowsOptions.Length)
            nextEyebrowIndex = 0;

        SetEyebrowOption(nextEyebrowIndex);
    }

    private void SetEyebrowOption(int index)
    {
        currentEyebrowIndex = index;
        eyebrowSelectionOutlinedText.SetText(currentEyebrowIndex.ToString());
        loadedCustomization.Eyebrows = ScenesSharedResources.Instance.EyebrowsOptions[currentEyebrowIndex];
    }

    public void ChangeHairOption(bool next)
    {
        int nextHairIndex = currentHairIndex + (next ? 1 : -1);
        if (nextHairIndex < 0)
            nextHairIndex = ScenesSharedResources.Instance.HairOptions.Length - 1;
        else if (nextHairIndex >= ScenesSharedResources.Instance.HairOptions.Length)
            nextHairIndex = 0;

        SetHairOption(nextHairIndex);
    }

    private void SetHairOption(int index)
    {
        currentHairIndex = index;
        hairSelectionOutlinedText.SetText(currentHairIndex.ToString());
        loadedCustomization.Hair = ScenesSharedResources.Instance.HairOptions[currentHairIndex];
    }

    public void ChangeShirtOption(bool next)
    {
        int nextShirtIndex = currentShirtIndex + (next ? 1 : -1);
        if (nextShirtIndex < 0)
            nextShirtIndex = ScenesSharedResources.Instance.ShirtOptions.Length - 1;
        else if (nextShirtIndex >= ScenesSharedResources.Instance.ShirtOptions.Length)
            nextShirtIndex = 0;

        SetShirtOption(nextShirtIndex);
    }

    private void SetShirtOption(int index)
    {
        currentShirtIndex = index;

        AssetReference shirtAsset = ScenesSharedResources.Instance.ShirtOptions[currentShirtIndex];
        CharacterShirtItemInformation shirtItemInfo = AssetsManager.GetAsset<CharacterShirtItemInformation>(shirtAsset);
        colorPickerImage[(int)ColorCustomizationPart.ShirtBase].transform.parent.gameObject.SetActive(ScenesSharedResources.Instance.ShirtOptions[currentShirtIndex] != null);
        colorPickerImage[(int)ColorCustomizationPart.ShirtTop].transform.parent.gameObject.SetActive(ScenesSharedResources.Instance.ShirtOptions[currentShirtIndex] != null);

        loadedCustomization.Shirt = new TwoColorsCosmeticInstance(loadedCustomization.Shirt.ItemInformationAsset,
                                colorPickerImage[(int)ColorCustomizationPart.ShirtBase].color,
                                colorPickerImage[(int)ColorCustomizationPart.ShirtTop].color);
        shirtSelectionOutlinedText.SetText(currentShirtIndex.ToString());
    }

    public void SetPartColor(ColorCustomizationPart part, Color32 color)
    {
        colorPickerImage[(int)part].color = color;
        
        switch (part)
        {
            case (ColorCustomizationPart.Eyebrows):
                loadedCustomization.EyebrowsColor = color;
                break;
            case (ColorCustomizationPart.Eyes):
                loadedCustomization.EyesColor = color;
                break;
            case (ColorCustomizationPart.Hair):
                loadedCustomization.HairColor = color;
                break;
            case (ColorCustomizationPart.Pants):
                loadedCustomization.Pants = new PantsItemInstance(loadedCustomization.Pants.ItemInformationAsset, colorPickerImage[(int)ColorCustomizationPart.Pants].color);
                break;
            case (ColorCustomizationPart.ShirtBase):
                {
                    CharacterShirtItemInformation shirtInfo = AssetsManager.GetAsset<CharacterShirtItemInformation>(loadedCustomization.Shirt.ItemInformationAsset);
                    loadedCustomization.Shirt = new TwoColorsCosmeticInstance(loadedCustomization.Shirt.ItemInformationAsset,
                                colorPickerImage[(int)ColorCustomizationPart.ShirtBase].color,
                                colorPickerImage[(int)ColorCustomizationPart.ShirtTop].color);
                    break;
                }
            case (ColorCustomizationPart.ShirtTop):
                {
                    CharacterShirtItemInformation shirtInfo = AssetsManager.GetAsset<CharacterShirtItemInformation>(loadedCustomization.Shirt.ItemInformationAsset);
                    loadedCustomization.Shirt = new TwoColorsCosmeticInstance(loadedCustomization.Shirt.ItemInformationAsset,
                                colorPickerImage[(int)ColorCustomizationPart.ShirtBase].color,
                                colorPickerImage[(int)ColorCustomizationPart.ShirtTop].color);
                    break;
                }
            case (ColorCustomizationPart.Skin):
                loadedCustomization.SkinColor = colorPickerImage[(int)ColorCustomizationPart.Skin].color;
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
        LoadCustomization(CharacterCustomization.RandomCharacterCustomization(currentSex));
    }

    private void LoadCustomization(CharacterCustomization customization)
    {
        int GetIndexOf(AssetReference[] array, AssetReference reference)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i].RuntimeKey.ToString() == reference.RuntimeKey.ToString())
                {
                    return i;
                }
            }

            return -1;
        }

        SetEyebrowOption(GetIndexOf(ScenesSharedResources.Instance.EyebrowsOptions, customization.Eyebrows));
        SetHairOption(GetIndexOf(ScenesSharedResources.Instance.HairOptions, customization.Hair));
        SetShirtOption(GetIndexOf(ScenesSharedResources.Instance.ShirtOptions, customization.Shirt?.ItemInformationAsset));

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
            SetPartColor(ColorCustomizationPart.ShirtBase, customization.Shirt.BaseColor);
            SetPartColor(ColorCustomizationPart.ShirtTop, customization.Shirt.TopColor);
        }

        SetPartColor(ColorCustomizationPart.Skin,     customization.SkinColor);
    }

    public void Finish()
    {
        Color32 shirtBaseColor = colorPickerImage[(int)ColorCustomizationPart.ShirtBase].color;
        Color32 shirtColorableColor = colorPickerImage[(int)ColorCustomizationPart.ShirtTop].color;

        PlayerCustomization.Character = loadedCustomization;
        PlayerCustomization.PlayerName = nameInputField.text;

        LoadingScene.sceneName = "Main";
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
                characterAnimator.SetFloat("Vertical", -1);
                break;
            case (TurnDirection.FrontLeft):
                characterTransform.localScale = new Vector3(-50, 50, 1);
                characterAnimator.SetFloat("Vertical", -1);
                break;
            case (TurnDirection.BackLeft):
                characterTransform.localScale = new Vector3(-50, 50, 1);
                characterAnimator.SetFloat("Vertical", 1);
                break;
            case (TurnDirection.BackRight):
                characterTransform.localScale = new Vector3(50, 50, 1);
                characterAnimator.SetFloat("Vertical", 1);
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
    ShirtTop
}