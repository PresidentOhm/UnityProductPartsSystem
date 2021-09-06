using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProductPrefabMaterialSetOperator : MonoBehaviour
{
    [SerializeField] private ProductPrefabDataManager productPrefabDataManager;
    public ProductPrefabDataManager ProductPrefabDataManager { get => productPrefabDataManager; set => productPrefabDataManager = value; }
    private List<BasePartDataManager> materialSwitchingParts = new List<BasePartDataManager>();
    public List<BasePartDataManager> MaterialSwitchingParts { get => materialSwitchingParts; set => materialSwitchingParts = value; }

    private int currentPartIndex = 0;

    private BasePartDataManager currentPart;

    bool hasPartSelect = false;


    private void OnEnable()
    {
        productPrefabDataManager.OnPrefabInitialized += GetInitialReferences;
        EventBus.Instance.OnSelectGO += SelectResponse;
        EventBus.Instance.OnDeselectGO += DeselectResponse;
        EventBus.Instance.OnChangeSeriesMaterialSet += ChangeSeriesMaterial;
    }

    private void OnDisable()
    {
        productPrefabDataManager.OnPrefabInitialized -= GetInitialReferences;
        EventBus.Instance.OnSelectGO -= SelectResponse;
        EventBus.Instance.OnDeselectGO -= DeselectResponse;
        EventBus.Instance.OnChangeSeriesMaterialSet -= ChangeSeriesMaterial;
        EventBus.Instance.OnChangePartMaterialSet -= ChangePartMaterial;

        productPrefabDataManager.OnPartAdded -= PartAddedResponse;
        productPrefabDataManager.OnPartRemoved -= PartRemovedResponse;
    }

    private void SelectResponse(GameObject go, ProductPrefabDataManager productPrefabDataManager)
    {
        EventBus.Instance.OnChangePartMaterialSet -= ChangePartMaterial;
        //Debug.Log("this.gameObject.transform.parent: " + this.gameObject.transform.parent);
        //Debug.Log("go.transform.parent: " + go.transform.parent);
        if (go != null && this.gameObject.transform.parent == go.transform.parent)
        {
            currentPartIndex = 0;
            //partmode turns on
            GetPrefabMaterialSetData();
            productPrefabDataManager.OnPartAdded += PartAddedResponse;
            productPrefabDataManager.OnPartRemoved += PartRemovedResponse;
            EventBus.Instance.OnChangePartMaterialSet += ChangePartMaterial;

        }
    }

    private void DeselectResponse(GameObject go)
    {
        if (go != null && this.gameObject.transform.parent == go.transform.parent)
        {
            productPrefabDataManager.OnPartAdded -= PartAddedResponse;
            productPrefabDataManager.OnPartRemoved -= PartRemovedResponse;
            EventBus.Instance.OnChangePartMaterialSet -= ChangePartMaterial;
        }
    }

    private void PartAddedResponse(BasePartDataManager partPrefab)
    {
        for (int i = 0; i < partPrefab.ActiveProduct.ProductSubParts.Count; i++)
        {
            if ((MeshMaterialSetter)partPrefab.ActiveProduct.ProductSubParts[i].GetComponent(typeof(MeshMaterialSetter)))
            {
                MaterialSwitchingParts.Add(partPrefab);
                return;
            }
        }

        if (MaterialSwitchingParts.Count > 0)
        {
            //Update the Material select UI
            EventBus.Instance.UpdateSelectableMaterialSets(productPrefabDataManager.SeriesMaterialSets, this);
        }
    }

    private void PartRemovedResponse(BasePartDataManager partPrefab)
    {
        for (int i = 0; i < partPrefab.ActiveProduct.ProductSubParts.Count; i++)
        {
            //here we can just compare with our materialParts list instead
            if ((MeshMaterialSetter)partPrefab.ActiveProduct.ProductSubParts[i].GetComponent(typeof(MeshMaterialSetter)))
            {
                MaterialSwitchingParts.Remove(partPrefab);
                return;
            }
        }
        //Update the Material select UI
        EventBus.Instance.UpdateSelectableMaterialSets(productPrefabDataManager.SeriesMaterialSets, this);
    }

    private void GetPrefabMaterialSetData()
    {
        if (MaterialSwitchingParts.Count > 0)
            EventBus.Instance.UpdateSelectableMaterialSets(productPrefabDataManager.SeriesMaterialSets, this);
    }

    public void DeselectMaterialSwitchablePart()
    {
        currentPartIndex = 0;
        currentPart = null;
        EventBus.Instance.PartDeselected();
    }

    //Allways begins at material switchable part with lowest index
    public void SelectFirstMaterialSwitchablePart()
    {
        //Select first index material part
        currentPartIndex = 0;
        currentPart = MaterialSwitchingParts[currentPartIndex];
        if (MaterialSwitchingParts.Count > 1)
        {
            EventBus.Instance.PartSelected(currentPart);
        }
    }

    //Returning bool tells UI if there is a next part.
    public bool SelectNextMaterialSwitchablePart()
    {
        //Which parts have active subparts with MeshMaterialSetters?
        //Needs to be up to date (include added extensions or accessories etc)
        //Go to next one
        if (currentPartIndex + 1 > MaterialSwitchingParts.Count - 1)
            return false;
        ++currentPartIndex;
        currentPart = MaterialSwitchingParts[currentPartIndex];
        EventBus.Instance.PartSelected(currentPart);
        //Debug.Log("next part event fired!");
        if (currentPartIndex + 1 > MaterialSwitchingParts.Count - 1)
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
        SelectNextMaterialSwitchablePart();
    }

    [ContextMenu(nameof(TestSwitchPrevious))]
    public void TestSwitchPrevious()
    {
        SelectPreviousMaterialSwitchablePart();
    }

    //Returning bool tells UI if there is a previous part.
    public bool SelectPreviousMaterialSwitchablePart()
    {
        //Which parts have active subparts with MeshMaterialSetters?
        //Needs to be up to date (include added extensions or accessories etc)
        //Go to next one
        if (currentPartIndex - 1 < 0)
            return false;
        --currentPartIndex;
        currentPart = MaterialSwitchingParts[currentPartIndex];
        EventBus.Instance.PartSelected(currentPart);
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

    private void ChangePartMaterial(MaterialSetSO materialSet)
    {
        currentPart.SetPartByMaterialSet(materialSet);
    }

    private void ChangeSeriesMaterial(MaterialSetSO materialSet, string series)
    {
        if (productPrefabDataManager.Series.Equals(series))
        {
            productPrefabDataManager.SetPrefabByMaterialSet(materialSet);
        }
    }

    /// <summary>
    /// Used after a prefab has been intialized to reference the specific material set enabled parts
    /// </summary>
    public void GetInitialReferences()
    {
        MaterialSwitchingParts.Clear();
        //This has to collect CurrentState data AFTER the ProductPrefabDataManager has been set to its initial active state
        for (int i = 0; i < productPrefabDataManager.CurrentState.Count; i++)
        {
            for (int j = 0; j < productPrefabDataManager.CurrentState[i].Part.ActiveProduct.ProductSubParts.Count; j++)
            {
                if (productPrefabDataManager.CurrentState[i].Part.ActiveProduct.ProductSubParts[j].TryGetComponent(typeof(MeshMaterialSetter), out Component meshMaterialSetter))
                {
                    MaterialSwitchingParts.Add(productPrefabDataManager.CurrentState[i].Part);
                    break;
                }
            }
        }
    }

#if UNITY_EDITOR


#endif
}