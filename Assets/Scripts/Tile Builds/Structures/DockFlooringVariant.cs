using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Structure Variants/Dock flooring")]
public class DockFlooringVariant : FlooringVariantBase
{
    [SerializeField] private Sprite supportTop = null;
    public Sprite SupportTop => supportTop;

    [SerializeField] private Sprite supportBottom = null;
    public Sprite SupportBottom => supportBottom;
}
