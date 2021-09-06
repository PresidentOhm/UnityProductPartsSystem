using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Implementation that moves an object between two transforms
/// </summary>
public class AccessoryPartStraightLineGuide : BaseAccessoryPartGuide
{
    [SerializeField] private BasePartDataManager part;
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;

    [SerializeField] private float sliderValue = 0;
    private float latestSliderValue;
    [SerializeField] private int sliderMinValue = 0;
    [SerializeField] private int sliderMaxValue = 1;
    [SerializeField] private bool sliderWholeNumbersMode = false;

    private float lineLength;
    private Vector3 direction;



    private void Awake()
    {
        lineLength = Vector3.Distance(pointA.position, pointB.position);
        direction = (pointB.position - pointA.position).normalized;

        latestSliderValue = sliderValue;
        MoveAccessory(sliderValue);
    }

    //How and when events are subscribed to needs testing later
    private void OnEnable()
    {
        EventBus.Instance.OnPartSelected += PartSelectedResponse;
        EventBus.Instance.OnAccessoryPartSelected += Instance_OnAccessoryPartSelected;
    }

    private void OnDisable()
    {
        EventBus.Instance.OnAccessorySliderValueChange -= MoveAccessory;
        EventBus.Instance.OnPartSelected -= PartSelectedResponse;
        EventBus.Instance.OnPartDeselected -= PartDeselectedResponse;
    }

    private void Instance_OnAccessoryPartSelected(BasePartDataManager basePartDataManager)
    {
        //Should probably make a getcomponent for this from the operator
        if (part == basePartDataManager)
        {

        }
    }

    private void SetInitialPosition()
    {

    }

    private void PartSelectedResponse(BasePartDataManager basePartDataManager)
    {
        EventBus.Instance.OnAccessorySliderValueChange -= MoveAccessory;
        EventBus.Instance.OnPartDeselected -= PartDeselectedResponse;
        if (part == basePartDataManager)
        {
            //This should be subscribed to inside operator instead
            EventBus.Instance.OnAccessorySliderValueChange += MoveAccessory;
            EventBus.Instance.OnPartDeselected += PartDeselectedResponse;
        }
    }

    private void PartDeselectedResponse()
    {

        EventBus.Instance.OnAccessorySliderValueChange -= MoveAccessory;
        EventBus.Instance.OnPartDeselected -= PartDeselectedResponse;
    }

    //Used to set the UI slider
    public override SliderData AccessorySelected()
    {
        //Check if first run and choose one between initial or latest
        sliderValue = SetSliderValue();
        return new SliderData(sliderValue,
                                sliderMinValue,
                                sliderMaxValue,
                                sliderWholeNumbersMode,
                                sliderReservedValueSpan);

    }

    //This is called from within the accessory operator class 
    public override void MoveAccessory(float sliderValue)
    {
        latestSliderValue = sliderValue;
        Vector3 newPos = lineLength * sliderValue/sliderMaxValue * direction;
        transform.position = newPos;
    }

    public override float SetSliderValue()
    {
        float prefabDistanceFromPointA = Vector3.Distance(transform.position, pointB.position);
        return prefabDistanceFromPointA / lineLength; 
    }
}

/// <summary>
/// Defines how a vertical slider (value 0-1) moves an accessory.
/// Placed on the accessory part gameobject.
/// </summary>
public abstract class BaseAccessoryPartGuide : MonoBehaviour
{
    [SerializeField] protected Transform startTransform;
    [SerializeField] protected float sliderReservedValueSpan;
    public float SliderReservedValueSpan { get => sliderReservedValueSpan; set => sliderReservedValueSpan = value; }

    public Transform StartTransform { get => startTransform; set => startTransform = value; }

    public abstract SliderData AccessorySelected();
    public abstract void MoveAccessory(float sliderValue);

    public abstract float SetSliderValue();
}

[System.Serializable]
public class AccessoryPartDataAndGuide
{
    public BasePartDataManager Part;
    public BaseAccessoryPartGuide Guide;

    public AccessoryPartDataAndGuide(BasePartDataManager Part, BaseAccessoryPartGuide Guide)
    {
        this.Part = Part;
        this.Guide = Guide;
    }
}

[System.Serializable]
public class AccessoryPartType
{
    [ReadOnly] public bool HasFreeSlots = true;
    public Sprite AccessoryPreview;
    public GameObject AccessoryPrefab;
}

[System.Serializable]
public class SliderData
{
    public float SliderValue;
    public int SliderMinValue;
    public int SliderMaxValue;
    public bool SliderWholeNumbersMode;
    public float SliderReservedValueSpan;

    public SliderData(float SliderValue, int SliderMinValue, int SliderMaxValue, bool SliderWholeNumbersMode, float SliderReservedValueSpan)
    {
        this.SliderValue = SliderValue;
        this.SliderMinValue = SliderMinValue;
        this.SliderMaxValue = SliderMaxValue;
        this.SliderWholeNumbersMode = SliderWholeNumbersMode;
        this.SliderReservedValueSpan = SliderReservedValueSpan;
    }
}