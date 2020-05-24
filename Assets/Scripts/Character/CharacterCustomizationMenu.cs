using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CharacterCustomizationMenu : MonoBehaviour
{
    public static CharacterInformation playerCharacter = null;

    [SerializeField] private ColorWheel colorWheel = null;

    public UIPartResources[] uiPartResources;
    [System.Serializable]
    public class UIPartResources
    {
        public SpriteRenderer[] uipartSpriteRenderers;
        public Image colorPicker;
        public Image slotImage;
    }

    private SpriteOptions[] spriteOptions = new SpriteOptions[6];
    private class SpriteOptions
    {
        public int currentIndex;
        public Sprite[] sprites;

        public SpriteOptions(Sprite[] sprites)
        {
            currentIndex = 0;
            this.sprites = sprites;
        }
    }

    private Dictionary<CharacterPart, UIPartResources> partToUIResourcesMap;

    private List<GameObject> currentShownColors = new List<GameObject>();
    private CharacterPart currentCharacterPart = 0;

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

        for (int i = 0; i < spriteOptions.Length; i++)
        {
            if (CharacterManager.Instance.partToSpritesOptionsMap.TryGetValue((CharacterPart)i, out Sprite[] sprites))
            {
                spriteOptions[i] = new SpriteOptions(sprites);
            }
        }

        playerCharacter = new CharacterInformation();

        CreateRandomCharacter();
    }

    public void SelectColorOption(Color32 color)
    {
        playerCharacter.partInformations[(int)currentCharacterPart].Color = color;
        RefreshDisplay();
    }

    public void NextPartOption(int partNum)
    {
        if (uiPartResources[partNum].slotImage != null)
        {
            spriteOptions[partNum].currentIndex++;
            if (spriteOptions[partNum].currentIndex >= spriteOptions[partNum].sprites.Length)
            {
                spriteOptions[partNum].currentIndex = 0;
            }
            playerCharacter.partInformations[partNum].Sprite = spriteOptions[partNum].sprites[spriteOptions[partNum].currentIndex];
        }
        else
        {
            Debug.Log("This part number does not have a UI slot image! This should not happen!");
        }

        RefreshDisplay();
    }

    public void PreviousPartOption(int partNum)
    {
        if (uiPartResources[partNum].slotImage != null)
        {
            spriteOptions[partNum].currentIndex--;
            if (spriteOptions[partNum].currentIndex < 0)
            {
                spriteOptions[partNum].currentIndex = spriteOptions[partNum].sprites.Length - 1;
            }
            playerCharacter.partInformations[partNum].Sprite = spriteOptions[partNum].sprites[spriteOptions[partNum].currentIndex];
        }
        else
        {
            Debug.Log("This part number does not have a UI slot image! This should not happen!");
        }

        RefreshDisplay();
    }

    private void SelectPartOption(Sprite sprite)
    {
        playerCharacter.partInformations[(int)currentCharacterPart].Sprite = sprite;

        RefreshDisplay();
    }

    public void CreateRandomCharacter()
    {
        playerCharacter = CharacterManager.Instance.CreateRandomCharacter();

        RefreshDisplay();
    }

    private void RefreshDisplay()
    {
        for (int i = 0; i < uiPartResources.Length; i++)
        {
            UIPartResources resource = uiPartResources[i];

            ICharacterPartInformation info = playerCharacter.partInformations[i];

            //Refresh character display
            {
                if (resource.uipartSpriteRenderers.Length > 1)
                {
                    // Error check here
                    // Dont set sprite if character part have multiple images (ex. Pants, shoes, eyes)
                    if (info.Sprite != null)
                    {
                        Debug.Log("Cannot assign sprite to a character part with multiple images! Something is going wrong here...");
                    }
                    else
                    {
                        foreach (SpriteRenderer renderer in resource.uipartSpriteRenderers)
                        {
                            renderer.color = info.Color;
                        }
                    }
                }
                else
                {
                    if (info.Sprite != null)
                    {
                        resource.uipartSpriteRenderers[0].sprite = info.Sprite;
                    }
                    resource.uipartSpriteRenderers[0].color = info.Color;
                }
            }
            //Refresh color picker color
            {
                if (resource.colorPicker != null)
                {
                    resource.colorPicker.color = info.Color;
                }
            }
            //Refresh parts slot
            {
                if (resource.slotImage != null)
                {
                    resource.slotImage.sprite = info.Sprite;
                }
            }
        }
    }

    public void OnColorPickerClick(int partNum)
    {
        currentCharacterPart = (CharacterPart)partNum;
        colorWheel.MovePointAndSliderToColor(playerCharacter.partInformations[partNum].Color);
    }

    public void Finish()
    {
        SceneManager.LoadScene("Main");
    }
}
