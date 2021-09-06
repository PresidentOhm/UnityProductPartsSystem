using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartsAnchorSelectUIController : MonoBehaviour
{
    [SerializeField] private PanelViewStates panelViewStates;
    [SerializeField] private ResponsiveMaterialSetsGrid responsiveMaterialSetsGrid;

    [SerializeField] private Button anchorSelectMode;

    [SerializeField] private Button anchorPart;
    [SerializeField] private Button anchorSeries;
    [SerializeField] private Button cancel;

    [SerializeField] private RectTransform anchorOptionsPanel;
    [SerializeField] private Button freeStanding;
    [SerializeField] private Button castInPlace;
    [SerializeField] private Button sunkenFoundation;
    [SerializeField] private Button looseFill;
    [SerializeField] private Button aboveGround;

    [SerializeField] private RectTransform freeStandingRectT;
    [SerializeField] private RectTransform castInPlaceRectT;
    [SerializeField] private RectTransform sunkenFoundationRectT;
    [SerializeField] private RectTransform looseFillRectT;
    [SerializeField] private RectTransform aboveGroundRectT;


    private BasePartDataManager currentAnchorpart;
    private bool partMode = true;

    private ProductPrefabDataManager currentPrefabManager;

    private List<AnchorType> currentAnchorTypes = new List<AnchorType>();
    private bool hasInitializedAnchorTypes = false;

    //Check ProductPrefabManager.cs and limit anchor filtration to AnchorPartDataManager parts
    //Selects the part that has anchor (linerenderer) or series
    //No part selection arrows
    //Won't show when there are no anchor options


    private void OnEnable()
    {
        EventBus.Instance.OnUpdateSelectableAnchorTypes += Instance_OnUpdateSelectableAnchorTypes;
        EventBus.Instance.OnSelectGO += Instance_OnSelectGO;

        anchorSelectMode.onClick.AddListener(ShowAnchorOptions);
        cancel.onClick.AddListener(CancelButtonResponse);
        anchorPart.onClick.AddListener(AnchorPartClickResponse);
        anchorSeries.onClick.AddListener(AnchorSeriesClickResponse);

        freeStanding.image.enabled = false;
        castInPlace.image.enabled = false;
        sunkenFoundation.image.enabled = false;
        looseFill.image.enabled = false;
        aboveGround.image.enabled = false;

        freeStanding.onClick.AddListener(delegate { SendAncorChangeEvent(AnchorType.FreeStanding); });
        castInPlace.onClick.AddListener(delegate { SendAncorChangeEvent(AnchorType.CastInPlace); });
        sunkenFoundation.onClick.AddListener(delegate { SendAncorChangeEvent(AnchorType.SunkenFoundation); });
        looseFill.onClick.AddListener(delegate { SendAncorChangeEvent(AnchorType.LooseFill); });
        aboveGround.onClick.AddListener(delegate { SendAncorChangeEvent(AnchorType.AboveGround); });
        anchorPart.interactable = false;
        anchorSeries.interactable = true;
    }

    private void OnDisable()
    {
        EventBus.Instance.OnUpdateSelectableAnchorTypes -= Instance_OnUpdateSelectableAnchorTypes;
        EventBus.Instance.OnSelectGO -= Instance_OnSelectGO;

        anchorSelectMode.onClick.RemoveAllListeners();
        cancel.onClick.RemoveAllListeners();
        anchorPart.onClick.RemoveAllListeners();
        anchorSeries.onClick.RemoveAllListeners();

        freeStanding.onClick.RemoveAllListeners();
        castInPlace.onClick.RemoveAllListeners();
        sunkenFoundation.onClick.RemoveAllListeners();
        looseFill.onClick.RemoveAllListeners();
        aboveGround.onClick.RemoveAllListeners();
    }

    private void Instance_OnSelectGO(GameObject go, ProductPrefabDataManager productPrefabDataManager)
    {
        if (productPrefabDataManager.ProductPrefabAvailableOperators == null || !productPrefabDataManager.ProductPrefabAvailableOperators.HasAnchorCapacity)
        {
            anchorSelectMode.image.enabled = false;
            anchorSelectMode.enabled = false;
        }
        else
        {
            anchorSelectMode.image.enabled = true;
            anchorSelectMode.enabled = true;
            anchorPart.interactable = false;
            anchorSeries.interactable = true;
            currentPrefabManager = productPrefabDataManager;
        }
    }

    private void Instance_OnUpdateSelectableAnchorTypes(List<AnchorType> anchorTypes, AnchorPartDataManager anchorPartDataManager)
    {
        //Debug.Log("anchorTypes count: " + anchorTypes.Count);

        if (!hasInitializedAnchorTypes || currentAnchorTypes != anchorTypes)
        {
            if (!hasInitializedAnchorTypes)
                hasInitializedAnchorTypes = true;

            currentAnchorTypes = anchorTypes;

            freeStanding.image.enabled = false;
            freeStanding.interactable = false;
            castInPlace.image.enabled = false;
            castInPlace.interactable = false;
            sunkenFoundation.image.enabled = false;
            sunkenFoundation.interactable = false;
            looseFill.image.enabled = false;
            looseFill.interactable = false;
            aboveGround.image.enabled = false;
            aboveGround.interactable = false;

            freeStanding.gameObject.SetActive(false);
            castInPlace.gameObject.SetActive(false);
            sunkenFoundation.gameObject.SetActive(false);
            looseFill.gameObject.SetActive(false);
            aboveGround.gameObject.SetActive(false);

            List<RectTransform> currentAnchorButtons = new List<RectTransform>();
            //Decide which anchor buttons images to show
            for (int i = 0; i < anchorTypes.Count; i++)
            {
                switch (anchorTypes[i])
                {
                    case AnchorType.FreeStanding:
                        freeStanding.image.enabled = true;
                        freeStanding.interactable = true;
                        currentAnchorButtons.Add(freeStandingRectT);
                        break;
                    case AnchorType.CastInPlace:
                        castInPlace.image.enabled = true;
                        castInPlace.interactable = true;
                        currentAnchorButtons.Add(castInPlaceRectT);
                        break;
                    case AnchorType.SunkenFoundation:
                        sunkenFoundation.image.enabled = true;
                        sunkenFoundation.interactable = true;
                        currentAnchorButtons.Add(sunkenFoundationRectT);
                        break;
                    case AnchorType.LooseFill:
                        looseFill.image.enabled = true;
                        looseFill.interactable = true;
                        currentAnchorButtons.Add(looseFillRectT);
                        break;
                    case AnchorType.AboveGround:
                        aboveGround.image.enabled = true;
                        aboveGround.interactable = true;
                        currentAnchorButtons.Add(aboveGroundRectT);
                        break;
                    default:
                        break;
                }
            }
            responsiveMaterialSetsGrid.ExternalUpdateGridContent(currentAnchorButtons);
        }
        currentAnchorpart = anchorPartDataManager;
    }

    private void SendAncorChangeEvent(AnchorType anchorType)
    {
        if (partMode)
            EventBus.Instance.ChangePrefabAnchor(anchorType);
        else
            EventBus.Instance.ChangeSeriesAnchor(anchorType, currentPrefabManager.Series);
    }

    private void ShowAnchorOptions()
    {
        partMode = true;
        panelViewStates.SwitchViewState(ViewState.ARSwitchAnchor);
        //materialsSetsCanvasGroup.alpha = 1;
        
        anchorPart.interactable = false;
        anchorSeries.interactable = true;

        cancel.enabled = true;
        anchorOptionsPanel.gameObject.SetActive(true);

        //EventBus.Instance.PartSelected(currentAnchorpart);
    }

    private void AnchorPartClickResponse()
    {
        EventBus.Instance.SeriesDeselect();
        partMode = true;

        anchorPart.interactable = false;
        anchorSeries.interactable = true;

        //EventBus.Instance.PartSelected(currentAnchorpart);
    }

    private void AnchorSeriesClickResponse()
    {
        EventBus.Instance.SeriesSelect(currentPrefabManager.Series);
        //EventBus.Instance.PartDeselected();
        partMode = false;

        anchorPart.interactable = true;
        anchorSeries.interactable = false;
    }

    private void CancelButtonResponse()
    {
        EventBus.Instance.SeriesDeselect();
        anchorPart.interactable = false;
        anchorSeries.interactable = true;

        panelViewStates.SwitchViewState(ViewState.ARProductSelected);
        //EventBus.Instance.PartDeselected();
    }

}
