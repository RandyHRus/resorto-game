using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character Body/Hair")]
public class CharacterHair : CharacterCustomizableBodyPart
{
    [SerializeField] private Sprite frontHair = null;
    public Sprite FrontHair => frontHair;

    [SerializeField] private Sprite backHair = null;
    public Sprite BackHair => backHair;
}
