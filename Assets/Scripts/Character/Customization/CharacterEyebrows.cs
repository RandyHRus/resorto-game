using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character Body/Eyebrows")]
public class CharacterEyebrows : CharacterCustomizableBodyPart
{
    [SerializeField] private Sprite sprite = null;
    public Sprite Sprite => sprite;
}
