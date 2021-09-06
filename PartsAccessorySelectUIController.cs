using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartsAccessorySelectUIController : MonoBehaviour
{
    [SerializeField] private PanelViewStates panelViewStates;
    [SerializeField] private Slider slider;
    [SerializeField] private List<Button> accessoryTypeButtons = new List<Button>();
    [SerializeField] private Button nextPart;
    [SerializeField] private Button previousPart;
    [SerializeField] private Button cancel;
    [SerializeField] private Button addAccessory;
    [SerializeField] private Button removeAccessory;


    private SliderData currentSliderData;
    private ProductPrefabAccessoryOperator currentAccessoryOperator;

    private void OnEnable()
    {
        EventBus.Instance.OnSelectGO += Instance_OnSelectGO;
        EventBus.Instance.OnDeselectGO += Instance_OnDeselectGO;
        EventBus.Instance.OnAccessorySelected += Instance_OnAccessoryPrefabSelected;
        EventBus.Instance.OnAfterAddedAccessoryFreeSlotStatus += Instance_OnAfterAddedAccessoryFreeSlotStatus;
    }


    private void OnDisable()
    {
        EventBus.Instance.OnSelectGO -= Instance_OnSelectGO;
        EventBus.Instance.OnDeselectGO -= Instance_OnDeselectGO;
        EventBus.Instance.OnAccessorySelected -= Instance_OnAccessoryPrefabSelected;
    }

    private void Instance_OnSelectGO(GameObject go, ProductPrefabDataManager productPrefabDataManager)
    {
        if (productPrefabDataManager.ProductPrefabAvailableOperators == null || !productPrefabDataManager.ProductPrefabAvailableOperators.HasAccessoryCapacity)
        {
            //not an accessory prefab => hide accessory prefab buttons
            for (int i = 0; i < accessoryTypeButtons.Count; i++)
            {
                accessoryTypeButtons[i].gameObject.SetActive(false);
            }
        }
        else
        {
            //is an accessory prefab => update UI => show accessory buttons from preview sprites
            if (SelectionHandler.latestSelectedGO != null && gameObject.name.Equals(SelectionHandler.latestSelectedGO.name))
            {
                //not a new type selected, do nothing?

            }
            else
            {
                //new type selected => hide UI now then operator event will refresh type buttons later
                for (int i = 0; i < accessoryTypeButtons.Count; i++)
                {
                    accessoryTypeButtons[i].gameObject.SetActive(false);
                }
            }
        }
    }

    //Triggered by operator
    private void Instance_OnAccessoryPrefabSelected(ProductPrefabAccessoryOperator accessoryOperator)
    {
        //Update buttons and current
        currentAccessoryOperator = accessoryOperator;
        ShowAccessoryPrefabButtons();
    }

    private void Instance_OnDeselectGO(GameObject go)
    {
        //Hide any accessory related ui that is persistent beyond the accessory panel, if any such exists
    }

    //Triggered by event from accessory operator (=> Then switch to accessory panel)
    private void UpdateAccessoryPanelUI(SliderData sliderData)
    {
        //Update UI


        //Update slider
        UpdateSlider(sliderData);

        //Update previous next


        //Update plus minus

    }

    private void ShowAccessoryPrefabButtons()
    {
        //Update the prefab buttons sprites and onclick delegates
        for (int i = 0; i < accessoryTypeButtons.Count; i++)
        {
            accessoryTypeButtons[i].onClick.RemoveAllListeners();

            if(currentAccessoryOperator.AccessoryPartTypes[i] != null)
            {
                accessoryTypeButtons[i].image.sprite = currentAccessoryOperator.AccessoryPartTypes[i].AccessoryPreview;
                accessoryTypeButtons[i].onClick.AddListener(delegate { AccessoryPrefabTypeButtonResponse(currentAccessoryOperator.AccessoryPartTypes[i].AccessoryPrefab); });
                accessoryTypeButtons[i].gameObject.SetActive(true);
            }
            else
            {
                accessoryTypeButtons[i].gameObject.SetActive(false);
            }
        }
    }
        
    private void AccessoryPrefabTypeButtonResponse(GameObject prefabType)
    {
        currentAccessoryOperator.SpecificAccessoryButtonResponse(prefabType);

        UpdateSlider(currentAccessoryOperator.CurrentAccessory.Guide.AccessorySelected());
        
        //If more than one accessory of this type is available => show prev and next
        if (currentAccessoryOperator.AccessoryPartsAndGuideByPrefabType[prefabType].Count > 1)
        {
            nextPart.enabled = true;
            nextPart.interactable = true;
            nextPart.image.enabled = true;
            previousPart.enabled = true;
            previousPart.interactable = false;
            previousPart.image.enabled = true;
        }
        else
        {
            nextPart.enabled = false;
            nextPart.image.enabled = false;
            previousPart.enabled = false;
            previousPart.image.enabled = false;
        }

        addAccessory.enabled = true;
        removeAccessory.enabled = true;

        //Show the accessory panel
        panelViewStates.SwitchViewState(ViewState.ARAccessoryParts);
    }

    private void UpdateSlider(SliderData sliderData)
    {
        slider.value = sliderData.SliderValue;
        slider.minValue = sliderData.SliderMinValue;
        slider.maxValue = sliderData.SliderMaxValue;
        slider.wholeNumbers = sliderData.SliderWholeNumbersMode;
    }

    //Triggered by the + button
    private void AddAccessory()
    {
        //Add new accessory
        addAccessory.interactable = false;
        //Show the correct UI buttons
        if (currentAccessoryOperator.AccessoryPartsAndGuideByPrefabType[currentAccessoryOperator.SelectedAccessoryPrefabType].Count > 0)
        {
            previousPart.enabled = true;
            previousPart.interactable = true;
            previousPart.image.enabled = true;
        }
        else
        {
            previousPart.enabled = false;
            previousPart.interactable = false;
            previousPart.image.enabled = false;
        }
        currentAccessoryOperator.AddAccessoryPart();

        nextPart.enabled = false;
        nextPart.interactable = false;
        nextPart.image.enabled = false;
    }

    

    //After add of accessory the bool hasFreeSlot tells us if + button should be visible
    private void Instance_OnAfterAddedAccessoryFreeSlotStatus(bool hasFreeSlot)
    {
        UpdateSlider(currentAccessoryOperator.CurrentAccessory.Guide.AccessorySelected());

        if (hasFreeSlot)
        {
            addAccessory.interactable = true;
        }
        else
        {
            addAccessory.interactable = false;
        }
    }

    //Triggered by the remove button
    private void RemoveAccessory()
    {
        removeAccessory.interactable = false;
        addAccessory.interactable = true;
    }

    //Only used to control buttons interactive state
    private void NextPartClickResponse()
    {
        if (!currentAccessoryOperator.SelectNextAccessoryPart())
        {
            nextPart.interactable = false;
        }
        previousPart.interactable = true;
    }

    //Only used to control buttons interactive state
    private void PreviousPartClickResponse()
    {
        if (!currentAccessoryOperator.SelectPreviousAccessoryPart())
        {
            previousPart.interactable = false;
        }
        nextPart.interactable = true;
    }

    private void CancelButtonResponse()
    {
        nextPart.enabled = false;
        nextPart.image.enabled = false;
        previousPart.enabled = false;
        previousPart.image.enabled = false;
        panelViewStates.SwitchViewState(ViewState.ARProductSelected);
        EventBus.Instance.PartDeselected();
    }
}
