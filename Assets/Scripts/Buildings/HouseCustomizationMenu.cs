using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class HouseCustomizationMenu : MonoBehaviour
{
    [EnumNamedArray(typeof(HousePartIndex)), SerializeField]
    private PartResources[] partResources = new PartResources[Enum.GetNames(typeof(HousePartIndex)).Length];

    [SerializeField] private HouseCustomizationLoader uiLoader = null;

    [SerializeField] BuildingStructureVariant houseStructure = null;

    [System.Serializable]
    private class PartResources
    {
        public SpriteOption[] spriteOptions = null;
        public Image slotImage = null;
    }

    [System.Serializable]
    public class SpriteOption
    {
        public Sprite icon;
        public Sprite actualSprite;
    }

    private class CurrentIndex
    {
        public int index;
    }

    private HouseCustomization houseCustomization = new HouseCustomization();

    private Dictionary<HousePartIndex, Tuple<PartResources, CurrentIndex>> housePartToResourcesAndCurrentIndex;

    private void Awake()
    {

        housePartToResourcesAndCurrentIndex = new Dictionary<HousePartIndex, Tuple<PartResources, CurrentIndex>>()
        {
            {  HousePartIndex.Wall,        Tuple.Create(partResources[(int)HousePartIndex.Wall],         new CurrentIndex()) },
            {  HousePartIndex.Base,        Tuple.Create(partResources[(int)HousePartIndex.Base],         new CurrentIndex()) },
            {  HousePartIndex.WallSupport, Tuple.Create(partResources[(int)HousePartIndex.WallSupport],  new CurrentIndex()) },
            {  HousePartIndex.Chimney,     Tuple.Create(partResources[(int)HousePartIndex.Chimney],      new CurrentIndex()) },
            {  HousePartIndex.Window,      Tuple.Create(partResources[(int)HousePartIndex.Window],       new CurrentIndex()) },
            {  HousePartIndex.Door,        Tuple.Create(partResources[(int)HousePartIndex.Door],         new CurrentIndex()) },
            {  HousePartIndex.Roof,        Tuple.Create(partResources[(int)HousePartIndex.Roof],         new CurrentIndex()) }
        };

        //Initialize icons and house sprites
        foreach (KeyValuePair<HousePartIndex, Tuple<PartResources, CurrentIndex>> pair in housePartToResourcesAndCurrentIndex) {
            SetHousePartSprite((HousePartIndex)pair.Key, pair.Value.Item1.spriteOptions[0].actualSprite);
            SetSlotIcon(pair.Key, pair.Value.Item1.spriteOptions[0].icon);
        }
    }

    //TODO merge with PreviousPartOption to make it 1 function cause there is alot of repetition here
    public void NextPartOption(int partIndex)
    {      
        Tuple<PartResources, CurrentIndex> option = housePartToResourcesAndCurrentIndex[(HousePartIndex)partIndex];
        CurrentIndex spriteIndex = option.Item2;
        spriteIndex.index++;
        if (spriteIndex.index > option.Item1.spriteOptions.Length - 1)
        {
            spriteIndex.index = 0;
        }
        SetHousePartSprite((HousePartIndex)partIndex, option.Item1.spriteOptions[spriteIndex.index].actualSprite);
        SetSlotIcon((HousePartIndex)partIndex, option.Item1.spriteOptions[spriteIndex.index].icon);
    }

    //TODO merge with NextPartOption to make it 1 function cause there is alot of repetition here
    public void PreviousPartOption(int partIndex)
    {
        Tuple<PartResources, CurrentIndex> option = housePartToResourcesAndCurrentIndex[(HousePartIndex)partIndex];
        CurrentIndex spriteIndex = option.Item2;
        spriteIndex.index--;
        if (spriteIndex.index < 0)
        {
            spriteIndex.index = option.Item1.spriteOptions.Length - 1;
        }
        SetHousePartSprite((HousePartIndex)partIndex, option.Item1.spriteOptions[spriteIndex.index].actualSprite);
        SetSlotIcon((HousePartIndex)partIndex, option.Item1.spriteOptions[spriteIndex.index].icon);
    }

    private void CreateRandomHouse()
    {
        for (int i = 0; i < partResources.Length; i++)
        {
            //TODO
            //int spriteCount = partResources[i].spriteOptions.Length;
            //int randomSpriteIndex = UnityEngine.Random.Range(0, spriteCount);
            //partResources[i].uiRenderer.sprite = partResources[i].spriteOptions[randomSpriteIndex].actualSprite;
            //jfakfas
        }
    }

    private void SetSlotIcon(HousePartIndex index, Sprite iconSprite)
    {
        Image slotImage = housePartToResourcesAndCurrentIndex[index].Item1.slotImage;
        //Slot needs to be disabled when sprite is null so that there is no white square displayed
        if (iconSprite == null)
        {
            slotImage.enabled = false;
        }
        else
        {
            slotImage.enabled = true;
            slotImage.sprite = iconSprite;
        }
    }

    private void SetHousePartSprite(HousePartIndex index, Sprite sprite)
    {
        //Set house part info
        houseCustomization.housePartToSprite[index] = sprite;

        //Refresh UI
        uiLoader.LoadCustomization(houseCustomization);
    }

    public void CreateButtonPressed()
    {
        UIPanelsManager.Instance.CloseCurrentPanel();
        PlayerStateMachine.Instance.TrySwitchState<CreateBuildingsState>(new object[] { houseStructure, houseCustomization });
    }
}
