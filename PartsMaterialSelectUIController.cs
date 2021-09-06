using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Manages the UI buttons for material set selection for parts and series
/// </summary>
public class PartsMaterialSelectUIController : BaseCanvasControlledIdentifier
{
    [SerializeField] private ResponsiveMaterialSetsGrid responsiveMaterialSetsGrid;
    [SerializeField] private PanelViewStates panelViewStates;

    [SerializeField] private Button materialSelectMode;
    [SerializeField] private Button brush;
    [SerializeField] private Button bucket;
    [SerializeField] private Button nextPart;
    [SerializeField] private Button previousPart;
    [SerializeField] private Button cancel;
    [SerializeField] private RectTransform materialSetButtonsParent;
    [SerializeField] private CanvasGroup materialsSetsCanvasGroup;
    [SerializeField] private GameObject materialSetButtonPrefab;

    private List<Button> materialSetButtonPool = new List<Button>();

    private List<Button> currentMaterialSetButtons = new List<Button>();

    private List<MaterialSetSO> currentMaterialSetSOs = new List<MaterialSetSO>();

    private ProductPrefabMaterialSetOperator currentMaterialSetOperator;

    private bool partMode = true;
    private bool hasManyParts = false;
    private bool materialSetButtonsInitialized = false;
    private bool showBrushAndBucket = false;


    private void OnEnable()
    {
        materialsSetsCanvasGroup.alpha = 0;
        brush.interactable = false;
        bucket.interactable = true;
        nextPart.enabled = false;
        nextPart.image.enabled = false;
        previousPart.enabled = false;
        previousPart.image.enabled = false;
        //The event that also tells us that we have a product with material switching enabled
        EventBus.Instance.OnSelectGO += Instance_OnSelectGO;
        EventBus.Instance.OnUpdateSelectableMaterialSets += UpdateUI;
        EventBus.Instance.OnDeselectGO += Instance_OnDeselectGO;
        materialSelectMode.onClick.AddListener(MaterialSelectModeResponse);
        brush.onClick.AddListener(BrushClickResponse);
        bucket.onClick.AddListener(BucketClickResponse);
        nextPart.onClick.AddListener(NextPartClickResponse);
        previousPart.onClick.AddListener(PreviousPartClickResponse);
        cancel.onClick.AddListener(CancelButtonResponse);
    }

    private void OnDisable()
    {
        EventBus.Instance.OnSelectGO -= Instance_OnSelectGO;
        EventBus.Instance.OnUpdateSelectableMaterialSets -= UpdateUI;
        EventBus.Instance.OnDeselectGO -= Instance_OnDeselectGO;
        materialSelectMode.onClick.RemoveListener(MaterialSelectModeResponse);
        brush.onClick.RemoveListener(BrushClickResponse);
        bucket.onClick.RemoveListener(BucketClickResponse);
        nextPart.onClick.RemoveListener(NextPartClickResponse);
        previousPart.onClick.RemoveListener(PreviousPartClickResponse);
        cancel.onClick.RemoveListener(CancelButtonResponse);

        ToggleMaterialSetsSpriteButtons(false);
    }

    //This sometimes runs after UpdateUI, may only run before
    private void Instance_OnSelectGO(GameObject go, ProductPrefabDataManager productPrefabDataManager)
    {
        //Debug.Log("partMode:" + partMode);
        if (productPrefabDataManager.ProductPrefabAvailableOperators == null || !productPrefabDataManager.ProductPrefabAvailableOperators.HasMaterialSetCapacity)
        {
            materialSelectMode.image.enabled = false;
            materialSelectMode.enabled = false;
        }
        else
        {
            materialSelectMode.image.enabled = true;
            materialSelectMode.enabled = true;
        }
    }

    private void Instance_OnDeselectGO(GameObject go)
    {
        materialsSetsCanvasGroup.alpha = 0;
        brush.interactable = false;
        bucket.interactable = true;

        nextPart.enabled = false;
        nextPart.image.enabled = false;
        previousPart.enabled = false;
        previousPart.image.enabled = false;
        EventBus.Instance.PartDeselected();
        ToggleMaterialSetsSpriteButtons(false);
    }

    private void ToggleMaterialSetsSpriteButtons(bool turnOn)
    {
        for (int i = 0; i < currentMaterialSetButtons.Count; i++)
        {
            currentMaterialSetButtons[i].image.enabled = turnOn;
            currentMaterialSetButtons[i].enabled = turnOn;
            currentMaterialSetButtons[i].interactable = turnOn;
        }
    }

    //private void UpdateMaterialUIData(List<MaterialSetSO> materialSets, ProductPrefabMaterialSetOperator materialSetSwitcher)
    //{
    //    currentMaterialSetSOs = materialSets;
    //    currentMaterialSetOperator = materialSetSwitcher;
    //}

    //private void InitializeUIUpdate(List<MaterialSetSO> materialSets, ProductPrefabMaterialSetOperator materialSetOperator)
    //{
    //    StopAllCoroutines();
    //    StartCoroutine(UIUpdateDelay(materialSets, materialSetOperator));
    //}

    //private IEnumerator UIUpdateDelay(List<MaterialSetSO> materialSets, ProductPrefabMaterialSetOperator materialSetOperator)
    //{
    //    yield return new WaitForSeconds(0.1f);
    //    UpdateUI(materialSets, materialSetOperator);
    //}

    private void UpdateUI(List<MaterialSetSO> materialSets, ProductPrefabMaterialSetOperator materialSetOperator)
    {
        if (materialSetOperator == null)
            return;

        materialSetButtonsInitialized = currentMaterialSetOperator != null && currentMaterialSetOperator == materialSetOperator;

        currentMaterialSetOperator = materialSetOperator;

        if (currentMaterialSetSOs != null && currentMaterialSetSOs == materialSets)
            return;

        currentMaterialSetSOs = materialSets;

        partMode = true;
        //Debug.Log("material prefab UpdateUI");

        //This is the same for part and series mode
        if (materialSetOperator.MaterialSwitchingParts == null || materialSetOperator.MaterialSwitchingParts.Count == 0)
        {
            //Debug.Log("materialSetSwitcher.MaterialSwitchingParts: " + materialSetOperator.MaterialSwitchingParts.Count);
            //Hide materialSelectMode button
            hasManyParts = false;
            materialSelectMode.image.enabled = false;
            materialSelectMode.enabled = false;
        }
        else if (materialSetOperator.MaterialSwitchingParts.Count == 1)
        {
            //Show materialSelectMode button (but not next/previous)
            materialSelectMode.image.enabled = true;
            materialSelectMode.enabled = true;
            //Debug.Log("materialSetSwitcher.MaterialSwitchingParts: " + materialSetOperator.MaterialSwitchingParts.Count);
            hasManyParts = false;
            if (!materialSetButtonsInitialized)
            {
                UpdateMaterialSetButtons(materialSets);
            }
        }
        else if (materialSetOperator.MaterialSwitchingParts.Count > 1)
        {
            //Show part materialSelectMode button (and forward back arrow buttons)
            materialSelectMode.image.enabled = true;
            materialSelectMode.enabled = true;
            //Debug.Log("materialSetSwitcher.MaterialSwitchingParts: " + materialSetOperator.MaterialSwitchingParts.Count);
            hasManyParts = true;
            if (!materialSetButtonsInitialized)
            {
                UpdateMaterialSetButtons(materialSets);
            }
        }
    }

    /// <summary>
    /// Updates the material sprite buttons
    /// Some basic object pooling is used
    /// </summary>
    /// <param name="materialSets"></param>
    private void UpdateMaterialSetButtons(List<MaterialSetSO> materialSets)
    {
        //Create more buttons if necessary (if result is negative we hide buttons)
        int missingButtonPrefabsCount = materialSets.Count - materialSetButtonPool.Count;
        currentMaterialSetButtons.Clear();
        //Check if more pooled buttons are needed
        if (missingButtonPrefabsCount > 0)
        {
            for (int i = 0; i < missingButtonPrefabsCount; i++)
            {
                GameObject newButton = Instantiate(materialSetButtonPrefab, materialSetButtonsParent);
                materialSetButtonPool.Add((Button)newButton.GetComponent(typeof(Button)));
            }
        }
        //float cellSizeX = materialSetButtonsParent.rect.width / materialSets.Count;
        //float cellHeight = materialSetButtonsParent.rect.height * 0.5f;
        //Turn on buttons to be used and turn off those that will not.
        List<RectTransform> visibleButtonsRectTransforms = new List<RectTransform>();
        for (int i = 0; i < materialSetButtonPool.Count; i++)
        {
            if (i > materialSets.Count - 1)
            {
                materialSetButtonPool[i].onClick.RemoveAllListeners();
                materialSetButtonPool[i].gameObject.SetActive(false);
            }
            else
            {
                materialSetButtonPool[i].gameObject.SetActive(true);
                materialSetButtonPool[i].image.sprite = materialSets[i].previewSprite;
                materialSetButtonPool[i].onClick.RemoveAllListeners();
                MaterialSetSO materialSetSO = materialSets[i];
                materialSetButtonPool[i].onClick.AddListener(delegate { ApplyMaterialSetSO(materialSetSO); });

                visibleButtonsRectTransforms.Add((RectTransform)materialSetButtonPool[i].gameObject.GetComponent(typeof(RectTransform)));
                //Should be updated to space with const pixelsize.x, regardless of number of material sets
                //float currentPosX = cellSizeX * 0.5f + (i * cellSizeX);
                //materialSetButtonPool[i].gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(currentPosX, 0, 1);
            }
        }

        responsiveMaterialSetsGrid.ExternalUpdateGridContent(visibleButtonsRectTransforms);
    }

    /// <summary>
    /// This is where the material set buttons sends the event with the 
    /// selected material set to be used for the part or series.
    /// </summary>
    /// <param name="materialSetSO"></param>
    private void ApplyMaterialSetSO(MaterialSetSO materialSetSO)
    {
        if(partMode)
        {
            EventBus.Instance.ChangePartMaterialSet(materialSetSO);
        }
        else
        {
            EventBus.Instance.ChangeSeriesMaterialSet(materialSetSO, currentMaterialSetOperator.ProductPrefabDataManager.Series);
        }
    }

    private void MaterialSelectModeResponse()
    {
        panelViewStates.SwitchViewState(ViewState.ARSwitchColor);
        ToggleMaterialSetsSpriteButtons(true);
        partMode = true;
        materialsSetsCanvasGroup.alpha = 1;

        brush.interactable = false;
        bucket.interactable = true;

        nextPart.interactable = true;
        nextPart.enabled = hasManyParts;
        nextPart.image.enabled = hasManyParts;
        previousPart.interactable = false;
        previousPart.enabled = hasManyParts;
        previousPart.image.enabled = hasManyParts;
        currentMaterialSetOperator.SelectFirstMaterialSwitchablePart();
    }

    private void BrushClickResponse()
    {
        EventBus.Instance.SeriesDeselect();
        partMode = true;
        //panelViewStates.SwitchViewState(ViewState.ARSwitchColor);
        //materialsSetsCanvasGroup.alpha = 1;

        brush.interactable = false;
        bucket.interactable = true;

        nextPart.interactable = true;
        nextPart.enabled = hasManyParts;
        nextPart.image.enabled = hasManyParts;
        previousPart.interactable = false;
        previousPart.enabled = hasManyParts;
        previousPart.image.enabled = hasManyParts;

        currentMaterialSetOperator.SelectFirstMaterialSwitchablePart();
    }
    private void BucketClickResponse()
    {
        EventBus.Instance.SeriesSelect(currentMaterialSetOperator.ProductPrefabDataManager.Series);
        EventBus.Instance.PartDeselected();
        partMode = false;
        //panelViewStates.SwitchViewState(ViewState.ARSwitchColor);

        bucket.interactable = false;
        brush.interactable = true;

        nextPart.enabled = false;
        nextPart.image.enabled = false;
        previousPart.enabled = false;
        previousPart.image.enabled = false;

        //materialsSetsCanvasGroup.alpha = 1;
        //Show all the products that are targeted with an indicator!!!!!!!!!!!!!!!!!!!!!
    }
    private void NextPartClickResponse()
    {
        if (!currentMaterialSetOperator.SelectNextMaterialSwitchablePart())
        {
            nextPart.interactable = false;
        }
        previousPart.interactable = true;
    }
    private void PreviousPartClickResponse()
    {
        if (!currentMaterialSetOperator.SelectPreviousMaterialSwitchablePart())
        {
            previousPart.interactable = false;
        }
        nextPart.interactable = true;
    }

    private void CancelButtonResponse()
    {
        EventBus.Instance.SeriesDeselect();
        ToggleMaterialSetsSpriteButtons(false);
        brush.interactable = false;
        bucket.interactable = true;
        materialsSetsCanvasGroup.alpha = 0;
        nextPart.enabled = false;
        nextPart.image.enabled = false;
        previousPart.enabled = false;
        previousPart.image.enabled = false;
        panelViewStates.SwitchViewState(ViewState.ARProductSelected);
        EventBus.Instance.PartDeselected();
    }
}
