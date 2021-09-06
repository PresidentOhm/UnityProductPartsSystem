using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used to manage the accessory data and gameobject instatiation & placement of accessory parts.
/// This script assumes no accessories are instantiated when a new product is placed but
/// we could use helper scripts if any such case is relevant.
/// This is placed on the same gameobject as the <see cref="ProductPrefabDataManager"/>
/// </summary>
public class ProductPrefabAccessoryOperator : MonoBehaviour
{
    [SerializeField] private ProductPrefabDataManager productPrefabDataManager;
    public ProductPrefabDataManager ProductPrefabDataManager { get => productPrefabDataManager; set => productPrefabDataManager = value; }

    [SerializeField] private List<AccessoryPartType> accessoryPartTypes = new List<AccessoryPartType>();
    public List<AccessoryPartType> AccessoryPartTypes { get => accessoryPartTypes; set => accessoryPartTypes = value; }

    public Dictionary<GameObject, List<AccessoryPartDataAndGuide>> AccessoryPartsAndGuideByPrefabType = new Dictionary<GameObject, List<AccessoryPartDataAndGuide>>();
    //public Dictionary<GameObject, List<BaseAccessoryPartGuide>> AccessoryGuidesByPrefabType = new Dictionary<GameObject, List<BaseAccessoryPartGuide>>();

    private GameObject selectedAccessoryPrefabType;
    public GameObject SelectedAccessoryPrefabType { get => selectedAccessoryPrefabType; set => selectedAccessoryPrefabType = value; }

    private int currentPartIndex = 0;

    private AccessoryPartDataAndGuide currentAccessory;
    public AccessoryPartDataAndGuide CurrentAccessory { get => currentAccessory; set => currentAccessory = value; }

    bool hasPartSelect = false;

    private void OnEnable()
    {
        currentPartIndex = 0;
        //Questionable if needed
        SelectedAccessoryPrefabType = AccessoryPartTypes[0].AccessoryPrefab;

        productPrefabDataManager.OnPrefabInitialized += GetInitialReferences;
        EventBus.Instance.OnSelectGO += SelectResponse;
        EventBus.Instance.OnDeselectGO += DeselectResponse;
    }

    private void SelectResponse(GameObject go, ProductPrefabDataManager productPrefabDataManager)
    {
        if (this.gameObject.transform.parent == go.transform.parent)
        {
            currentPartIndex = 0;

            //Raise accessory event
            EventBus.Instance.AccessorySelected(this);
        }
    }

    private void DeselectResponse(GameObject go)
    {
        if (this.gameObject.transform.parent == go.transform.parent)
        {

        }
    }

    private void GetInitialReferences()
    {
        AccessoryPartsAndGuideByPrefabType.Clear();
    }

    //public void SetSelectedAccessoryPrefab(GameObject accessoryPrefab)
    //{
    //    selectedAccessoryPrefabType = accessoryPrefab;
    //}


    //This new go should only be made visible AFTER its position has been set by SetInitialPosition();
    public void AddAccessoryPart()
    {
        GameObject accessory = Instantiate(SelectedAccessoryPrefabType, transform);
        //Set the initial position, here we can use interfaces per part with different slider/positioning types (maybe another type of BasePartDataManager?)
        BasePartDataManager accessoryPartData = (BasePartDataManager)accessory.GetComponent(typeof(BasePartDataManager));
        BaseAccessoryPartGuide accessoryGuide = (BaseAccessoryPartGuide)accessory.GetComponent(typeof(BaseAccessoryPartGuide));
        productPrefabDataManager.AddPartData(accessoryPartData);
        CurrentAccessory = new AccessoryPartDataAndGuide(accessoryPartData, accessoryGuide);
        AccessoryPartsAndGuideByPrefabType[SelectedAccessoryPrefabType].Add(CurrentAccessory);
        currentPartIndex = AccessoryPartsAndGuideByPrefabType[SelectedAccessoryPrefabType].IndexOf(CurrentAccessory);
        //position the prefab
        //accessory.transform.rotation = currentAccessory.Guide.StartTransform.rotation;
        //accessory.transform.position = currentAccessory.Guide.StartTransform.position;
        SetInitialPosition();
        //Send slider data to slider
    }

    //If an nth accessory of type is placed adjust
    //initial position to not be inside an already placed prefabs position.
    //Based on slider value.
    //Could be used to limit amount of accessories per part.
    //Slider default value should always be zero.
    private void SetInitialPosition()
    {
        if (AccessoryPartsAndGuideByPrefabType[SelectedAccessoryPrefabType].Count == 1)
            AccessoryPartsAndGuideByPrefabType[SelectedAccessoryPrefabType][0].Guide.MoveAccessory(0);

        List<FloatRangePair> sliderPositions = new List<FloatRangePair>();
        for (int i = 0; i < AccessoryPartsAndGuideByPrefabType[SelectedAccessoryPrefabType].Count; i++)
        {
            if (CurrentAccessory == AccessoryPartsAndGuideByPrefabType[SelectedAccessoryPrefabType][i])
                continue;

            float sliderPos = AccessoryPartsAndGuideByPrefabType[SelectedAccessoryPrefabType][i].Guide.SetSliderValue();
            float reservedValueOffset = AccessoryPartsAndGuideByPrefabType[SelectedAccessoryPrefabType][i].Guide.SliderReservedValueSpan / 2;
            FloatRangePair newRange = new FloatRangePair(sliderPos - reservedValueOffset, sliderPos + reservedValueOffset);
            sliderPositions.Add(newRange);
        }
        SliderData sliderData = CurrentAccessory.Guide.AccessorySelected();
        //convert scale to 0-1 mathematically (newvalue= (newMax- newMin)/(max-min)*(value-max)+ newMax)
        List<float> allSliderPositions = new List<float>();
        float currentSliderPos = sliderData.SliderMinValue;
        float valueSpan = AccessoryPartsAndGuideByPrefabType[SelectedAccessoryPrefabType][0].Guide.SliderReservedValueSpan;
        while (currentSliderPos <= sliderData.SliderMaxValue)
        {
            allSliderPositions.Add(currentSliderPos);
            currentSliderPos += valueSpan;
        }

        bool foundFreeSlot = false;
        //bool foundCurrentFreeSlot = false;
        for (int i = 0; i < allSliderPositions.Count; i++)
        {
            foundFreeSlot = true;
            for (int j = 0; j < AccessoryPartsAndGuideByPrefabType[SelectedAccessoryPrefabType].Count; j++)
            {
                if (CurrentAccessory == AccessoryPartsAndGuideByPrefabType[SelectedAccessoryPrefabType][i])
                    continue;
                //If slider range inside an allready occupied range
                if (AccessoryPartsAndGuideByPrefabType[SelectedAccessoryPrefabType][j].Guide.SetSliderValue() > allSliderPositions[i] - valueSpan
                    && AccessoryPartsAndGuideByPrefabType[SelectedAccessoryPrefabType][j].Guide.SetSliderValue() < allSliderPositions[i] + valueSpan)
                {
                    foundFreeSlot = false;
                }
            }

            if (foundFreeSlot)
            {
                //foundCurrentFreeSlot = true;
                int index = accessoryPartTypes.FindIndex(a => a.AccessoryPrefab == SelectedAccessoryPrefabType);
                accessoryPartTypes[index].HasFreeSlots = true;
                CurrentAccessory.Guide.MoveAccessory(allSliderPositions[i]);
                EventBus.Instance.PartSelected(CurrentAccessory.Part);
                EventBus.Instance.AfterAddedAccessoryFreeSlotStatus(true);
                return;
            }
        }

        if(!foundFreeSlot)
        {
            //This was the last free slot => trigger event to hide the + button
            int index = accessoryPartTypes.FindIndex(a => a.AccessoryPrefab == SelectedAccessoryPrefabType);
            accessoryPartTypes[index].HasFreeSlots = false;
            EventBus.Instance.PartSelected(CurrentAccessory.Part);
            EventBus.Instance.AfterAddedAccessoryFreeSlotStatus(false);
            return;
        }
    }

    private void RemoveAccessoryPart()
    {
        productPrefabDataManager.RemovePartData(AccessoryPartsAndGuideByPrefabType[SelectedAccessoryPrefabType][currentPartIndex].Part);
        AccessoryPartsAndGuideByPrefabType[SelectedAccessoryPrefabType].Remove(CurrentAccessory);
        DestroyImmediate(CurrentAccessory.Part.gameObject);
        //Check if any other accessories of this type are active and update selectedAccessoryPrefabType accordingly

        //Check if any other of this type of accessory and select that one for currentPart

    }

    //Sends event with the current accessorys UI data.
    private void SendUpdateToAccessoryUI()
    {
        //Send slider data

        //Send what buttons +, -, <=, => in what state

    }

    private void SetCurrentAccessory(AccessoryPartDataAndGuide accessoryPartDataAndGuide)
    {
        //Select the right part and update UI
        currentPartIndex = AccessoryPartsAndGuideByPrefabType[SelectedAccessoryPrefabType].IndexOf(accessoryPartDataAndGuide);
        CurrentAccessory = AccessoryPartsAndGuideByPrefabType[SelectedAccessoryPrefabType][currentPartIndex];
    }

    //Triggered by clicking a specific accessory button
    public void SpecificAccessoryButtonResponse(GameObject prefabType)
    {
        SelectedAccessoryPrefabType = prefabType;

        if (AccessoryPartsAndGuideByPrefabType == null || AccessoryPartsAndGuideByPrefabType[prefabType] == null || AccessoryPartsAndGuideByPrefabType[prefabType][0] == null)
        {
            //Add first accessory of this type
            AddAccessoryPart();
        }
        else
        {
            SelectFirstAccessoryPart();
        }
    }

    
    public void SelectFirstAccessoryPart()
    {
        //Select first index material part
        currentPartIndex = 0;
        CurrentAccessory = AccessoryPartsAndGuideByPrefabType[SelectedAccessoryPrefabType][currentPartIndex];
        if (AccessoryPartsAndGuideByPrefabType[SelectedAccessoryPrefabType].Count > 1)
        {
            EventBus.Instance.PartSelected(CurrentAccessory.Part);
        }
    }

    public bool SelectNextAccessoryPart()
    {
        //Which parts have active subparts with MeshMaterialSetters?
        //Needs to be up to date (include added extensions or accessories etc)
        //Go to next one
        if (currentPartIndex + 1 > AccessoryPartsAndGuideByPrefabType[SelectedAccessoryPrefabType].Count - 1)
            return false;
        ++currentPartIndex;
        CurrentAccessory = AccessoryPartsAndGuideByPrefabType[SelectedAccessoryPrefabType][currentPartIndex];
        EventBus.Instance.PartSelected(CurrentAccessory.Part);
        //Debug.Log("next part event fired!");
        if (currentPartIndex + 1 > AccessoryPartsAndGuideByPrefabType[SelectedAccessoryPrefabType].Count - 1)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    [ContextMenu(nameof(TestSwitchNext))]
    public void TestSwitchNext()
    {
        SelectNextAccessoryPart();
    }

    [ContextMenu(nameof(TestSwitchPrevious))]
    public void TestSwitchPrevious()
    {
        SelectPreviousAccessoryPart();
    }

    //Triggered by event from part back arrow
    //Returning bool tells UI if there is a previous part.
    public bool SelectPreviousAccessoryPart()
    {
        //Which parts have active subparts with MeshMaterialSetters?
        //Needs to be up to date (include added extensions or accessories etc)
        //Go to next one
        if (currentPartIndex - 1 < 0)
            return false;
        --currentPartIndex;
        CurrentAccessory = AccessoryPartsAndGuideByPrefabType[SelectedAccessoryPrefabType][currentPartIndex];
        EventBus.Instance.PartSelected(CurrentAccessory.Part);
        //Debug.Log("previous part event fired!");
        if (currentPartIndex - 1 < 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}

public class FloatRangePair
{
    float StartValue;
    float EndValue;

    public FloatRangePair(float StartValue, float EndValue)
    {
        this.StartValue = StartValue;
        this.EndValue = EndValue;
    }
}
