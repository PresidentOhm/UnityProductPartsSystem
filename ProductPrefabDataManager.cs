using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// This is the top level object in the Parts system.
/// A part is defined by a <see cref="BasePartDataManager"/> and is equivalent to a product number.
/// A part can contain many subparts that together represent a product number.
/// </summary>
public class ProductPrefabDataManager : MonoBehaviour
{
    [Header("For prefab without operators this can be left empty")]
    [SerializeField] private ProductPrefabAvailableOperators productPrefabAvailableOperators;
    public ProductPrefabAvailableOperators ProductPrefabAvailableOperators { get => productPrefabAvailableOperators; set => productPrefabAvailableOperators = value; }
    //set by getcomponent in editor
    [ReadOnly] [SerializeField] private string series;
    public string Series { get => series; set => series = value; }
    //set by getcomponent in editor
    [Space(order = 0)]
    [Header("Index order matters and base/anchor part index should be 0.", order = 1)]
    [Header("All references for this script are set by a contextmenu method.", order =2)]
    [Space(order = 3)]
    [SerializeField] private List<BasePartDataManager> parts = new List<BasePartDataManager>();
    public List<BasePartDataManager> Parts { get => parts; set => parts = value; }
    //set by editor method
    [ReadOnly] [SerializeField] private List<PartState> partsByProductNumbersSerialized = new List<PartState>();
    //this is set in awake by partsByProductNumbersSerialized;
    private Dictionary<int, BasePartDataManager> partsByProductNumbers = new Dictionary<int, BasePartDataManager>();

    //Current state. All parts gameobjects current ProductData
    private List<PartState> currentState = new List<PartState>();
    public List<PartState> CurrentState { get => currentState; set => currentState = value; }
    private List<PartState> tempState = new List<PartState>();

    //All the products in this prefab that are active (in the AR session)
    private List<ProductData> activeProductsData = new List<ProductData>();
    public List<ProductData> ActiveProductsData
    {
        get {
            activeProductsData.Clear();
            for (int i = 0; i < currentState.Count; i++)
            {
                activeProductsData.Add(currentState[i].Part.ActiveProduct.ProductData.ProductsDatas);
            }
            return activeProductsData;
        }
        set => activeProductsData = value;
    }

    // Series ColorMaterialSets auto set in editor
    [ReadOnly] [SerializeField] private List<MaterialSetSO> seriesMaterialSets = new List<MaterialSetSO>();
    public List<MaterialSetSO> SeriesMaterialSets { get => seriesMaterialSets; set => seriesMaterialSets = value; }

    ///This should be updated by <see cref="SetPrefabByMaterialSet"/> and material events, both series and part events.
    private MaterialSetSO lastUsedMaterialSet;
    // Series AnchorTypes set in editor
    [ReadOnly] [SerializeField] private List<AnchorType> seriesAnchorTypes = new List<AnchorType>();
    public List<AnchorType> SeriesAnchorTypes { get => seriesAnchorTypes; set => seriesAnchorTypes = value; }

    public delegate void PrefabInitializedCallback();
    public event PrefabInitializedCallback OnPrefabInitialized;
    public delegate void PartListUpdateCallback(BasePartDataManager partPrefab);
    public event PartListUpdateCallback OnPartAdded;
    public event PartListUpdateCallback OnPartRemoved;

#if UNITY_EDITOR
    [SerializeField] private int[] ProductNumberToLoadFromContextMenu;
    //Only intended to run once after prefab has been instantiated (placement button has been pressed)
    [ContextMenu(nameof(SetByNumberTest))]
    public void SetByNumberTest()
    {
        int[] productNumbers = ProductNumberToLoadFromContextMenu;
        InitializePrefabByProductNumbers(productNumbers);
    }
#endif
    //This is only used for prefab initialization!
    //This can only function if each part has exclusive product numbers.
    //No parts can contain the same product numbers when initializing.
    public void InitializePrefabByProductNumbers(int[] productNumbers)
    {
        bool foundNewState = false;
        tempState.Clear();

        if (!partsByProductNumbers.Any())
            partsByProductNumbers = partsByProductNumbersSerialized.ToDictionary(k => k.ProductNumber, v => v.Part);
        for (int i = 0; i < productNumbers.Length; i++)
        {
            if (partsByProductNumbers.ContainsKey(productNumbers[i]))
            {
                partsByProductNumbers[productNumbers[i]].SetPartByProductNumber(productNumbers[i]);
                foundNewState = true;
                tempState.Add(new PartState(productNumbers[i], partsByProductNumbers[productNumbers[i]]));
            }
        }
        if (foundNewState)
        {
            currentState = tempState;
            lastUsedMaterialSet = parts[0].ActiveProduct.ProductData.ProductsDatas.ProductMaterialSet;
            OnPrefabInitialized?.Invoke();
        }
    }

    //Not intended to be used for now
    //public void SetActiveParts(List<BasePartDataManager> activeParts)
    //{
    //    for (int i = 0; i < parts.Count; i++)
    //    {
    //        for (int j = 0; j < activeParts.Count; j++)
    //        {
    //            if (parts[i] == activeParts[j])
    //            {
    //                parts[i].gameObject.SetActive(true);
    //            }
    //            else
    //            {
    //                parts[i].gameObject.SetActive(false);
    //            }

    //        }
    //    }
    //}

    public void SetPrefabByMaterialSet(MaterialSetSO materialSet)
    {
        tempState.Clear();
        for (int i = 0; i < parts.Count; i++)
        {
            parts[i].SetPartByMaterialSet(materialSet);

            //Does this need an action callback from SetPartByMaterialSet before it runs? No.
            tempState.Add(new PartState(parts[i].ActiveProduct.ProductData.ProductsDatas.ProductNumber, parts[i]));
        }
        lastUsedMaterialSet = materialSet;
        currentState = tempState;
    }

    public void SetPrefabByAnchor(AnchorType anchorType)
    {
        bool foundNewState = false;
        tempState.Clear();
        for (int i = 0; i < parts.Count; i++)
        {
            if (parts[i].GetType() == typeof(AnchorPartDataManager))
            {
                //this is an anchor part
                foundNewState = true;
                AnchorPartDataManager anchorPart = parts[i] as AnchorPartDataManager;
                anchorPart.SetPartByAnchor(anchorType);
            }
            //Does this need an action callback from SetPartByAnchor before it runs?
            tempState.Add(new PartState(parts[i].ActiveProduct.ProductData.ProductsDatas.ProductNumber, parts[i]));
        }
        if (foundNewState)
        {
            currentState = tempState;
        }
    }
    

    //Adds a BasePartDataManager part (of simple type with no subparts) to the Parts
    //For extensions, and perhaps other accessories
    //Material should be the last selected one
    public void AddPartData(BasePartDataManager partPrefab)
    {
        partPrefab.SetPartByMaterialSet(lastUsedMaterialSet);
        currentState.Add(new PartState(partPrefab.ActiveProduct.ProductData.ProductsDatas.ProductNumber, partPrefab));
        parts.Add(partPrefab);
        OnPartAdded?.Invoke(partPrefab);
    }

    public void RemovePartData(BasePartDataManager partPrefab)
    {
        PartState deletePartState = new PartState(partPrefab.ActiveProduct.ProductData.ProductsDatas.ProductNumber, partPrefab);
        for (int i = 0; i < currentState.Count; i++)
        {
            if(currentState[i].Part == partPrefab)
            {
                currentState.Remove(currentState[i]);
            }
        }

        parts.Remove(partPrefab);
        OnPartRemoved?.Invoke(partPrefab);
    }

#if UNITY_EDITOR
    //Create method that populates the prefab manager from childrens SO product data
    [ContextMenu(nameof(SetPrefabReferences))]
    public void SetPrefabReferences()
    {
        //Get all parts of this prefab
        parts.Clear();
        parts = GetComponentsInChildren<BasePartDataManager>().ToList();
        //Get all product numbers and their corresponding part, for quick lookups
        partsByProductNumbersSerialized.Clear();
        partsByProductNumbers.Clear();
        for (int i = 0; i < parts.Count; i++)
        {
            for (int j = 0; j < parts[i].AllPartDatas.Count; j++)
            {
                for (int h = 0; h < parts[i].AllPartDatas[j].ProductDatas.Count; h++)
                {
                    if (!partsByProductNumbers.ContainsKey(parts[i].AllPartDatas[j].ProductDatas[h].ProductsDatas.ProductNumber))
                    {
                        partsByProductNumbers.Add(parts[i].AllPartDatas[j].ProductDatas[h].ProductsDatas.ProductNumber, parts[i]);
                    }
                }
            }
            parts[i].ReferenceAndGetAllMeshMaterialSetters();
        }
        foreach (var item in partsByProductNumbers)
        {
            partsByProductNumbersSerialized.Add(new PartState(item.Key, item.Value));
        }
        //Get series material sets from Part[0]
        seriesMaterialSets.Clear();
        //Used to only add unique MaterialSets
        Dictionary<string, MaterialSetSO> tempSets = new Dictionary<string, MaterialSetSO>();
        for (int i = 0; i < parts[0].AllPartDatas.Count; i++)
        {
            for (int j = 0; j < parts[0].AllPartDatas[i].ProductDatas.Count; j++)
            {
                if (!tempSets.ContainsKey(parts[0].AllPartDatas[i].ProductDatas[j].ProductsDatas.ProductMaterialSet.SetName))
                    tempSets.Add(parts[0].AllPartDatas[i].ProductDatas[j].ProductsDatas.ProductMaterialSet.SetName, parts[0].AllPartDatas[i].ProductDatas[j].ProductsDatas.ProductMaterialSet);
            }
        }
        foreach (var item in tempSets)
        {
            seriesMaterialSets.Add(item.Value);
        }
        //Get series anchor types from AnchorPartDataManager
        seriesAnchorTypes.Clear();
        for (int i = 0; i < parts.Count; i++)
        {
            if (parts[i].GetType() == typeof(AnchorPartDataManager))
            {
                //this is an anchor part
                AnchorPartDataManager anchorPart = parts[i] as AnchorPartDataManager;
                for (int j = 0; j < anchorPart.AllPartDatas.Count; j++)
                {
                    for (int h = 0; h < anchorPart.AllPartDatas[j].ProductDatas.Count; h++)
                    {
                        if (!seriesAnchorTypes.Contains(anchorPart.AllPartDatas[j].ProductDatas[h].ProductsDatas.AnchorType))
                        {
                            seriesAnchorTypes.Add(anchorPart.AllPartDatas[j].ProductDatas[h].ProductsDatas.AnchorType);
                        }
                    }
                }
                if (anchorPart.AnchorGOPairs != null && anchorPart.AnchorGOPairs.Any())
                {
                    for (int k = 0; k < anchorPart.AnchorGOPairs.Count; k++)
                    {
                        anchorPart.AnchorGOPairs[k].AnchorGameObject.SetActive(false);
                    }
                }
                
                break;
            }
        }
        for (int i = 0; i < parts.Count; i++)
        {
            for (int j = 0; j < parts[i].AllPartDatas.Count; j++)
            {
                parts[i].AllPartDatas[j].MeshFilters.Clear();
                parts[i].AllPartDatas[j].MeshRenderers.Clear();
                for (int h = 0; h < parts[i].AllPartDatas[j].ProductSubParts.Count; h++)
                {
                    parts[i].AllPartDatas[j].MeshFilters.Add(parts[i].AllPartDatas[j].ProductSubParts[h].GetComponent<MeshFilter>());
                    parts[i].AllPartDatas[j].MeshRenderers.Add(parts[i].AllPartDatas[j].ProductSubParts[h].GetComponent<MeshRenderer>());
                }
            }
        }
        //series should be a string that tests for ProductCategories.Undefined and sets
        if (parts[0].AllPartDatas[0].ProductDatas[0].ProductsDatas.ProductCategories == ProductCategories.Undefined)
        {
            series = this.gameObject.name;
        }
        else
        {
            series = parts[0].AllPartDatas[0].ProductDatas[0].ProductsDatas.ProductCategories.ToString();
        }
        TryGetComponent<ProductPrefabAvailableOperators>(out ProductPrefabAvailableOperators foundProductPrefabAvailableOperators);
        productPrefabAvailableOperators = foundProductPrefabAvailableOperators;
        if (productPrefabAvailableOperators)
            productPrefabAvailableOperators.SetCapacities();
    }
#endif
}