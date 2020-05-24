using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCustomizationLoader : MonoBehaviour
{
    [SerializeField] private UIPartResources[] characterPartRenderer = null;
    [System.Serializable]
    public class UIPartResources
    {
        public SpriteRenderer[] partRenderers;
    }

    public void LoadCharacter(CharacterInformation info)
    {
        for (int i = 0; i < info.partInformations.Length; i++)
        {
            ICharacterPartInformation partInfo = info.partInformations[i];

            if (characterPartRenderer[i].partRenderers.Length > 1)
            {
                // Error check here
                // Dont set sprite if character part have multiple images (ex. Pants, shoes, eyes)
                if (info.partInformations[i].Sprite != null)
                {
                    Debug.Log("Cannot assign sprite to a character part with multiple images! Something is going wrong here...");
                }
                else
                {
                    foreach (SpriteRenderer renderer in characterPartRenderer[i].partRenderers)
                    {
                        renderer.color = partInfo.Color;
                    }
                }
            }
            else
            {
                if (partInfo.Sprite != null)
                {
                    characterPartRenderer[i].partRenderers[0].sprite = partInfo.Sprite;
                }

                characterPartRenderer[i].partRenderers[0].color = partInfo.Color;
            }
        }
    }
}
