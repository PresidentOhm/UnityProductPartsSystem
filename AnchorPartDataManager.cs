using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


/// <summary>
/// Used by parts that have many multiple anchoring options types
/// </summary>
public class AnchorPartDataManager : BasePartDataManager
{
    [SerializeField] private List<AnchorTypeGameObjectPair> anchorGOPairs = new List<AnchorTypeGameObjectPair>();

    public List<AnchorTypeGameObjectPair> AnchorGOPairs { get => anchorGOPairs; set => anchorGOPairs = value; }

    public void SetPartByAnchor(AnchorType anchorType)
    {
        for (int i = 0; i < AllPartDatas.Count; i++)
        {
            for (int j = 0; j < AllPartDatas[i].ProductDatas.Count; j++)
            {
                if (AllPartDatas[i].ProductDatas[j].ProductsDatas.AnchorType == anchorType)
                {
                    //Debug.Log("found anchor match");
                    //Comparing the string by content not reference
                    if (AllPartDatas[i].ProductDatas[j].ProductsDatas.ProductMaterialSet.SetName.Equals(ActiveProduct.ProductData.ProductsDatas.ProductMaterialSet.SetName))
                    {
                        //Debug.Log("found material match");
                        //Comparing the list by content not reference
                        if (AllPartDatas[i].ProductSubParts == null || Enumerable.SequenceEqual(AllPartDatas[i].ProductSubParts, ActiveProduct.ProductSubParts))
                        {
                            //Debug.Log("found sub parts match");
                            //AllPartDatas[i].ProductSubParts can be null, it should be OK
                            ActiveProduct = new SubpartsProductDataSOPair(AllPartDatas[i].ProductSubParts, AllPartDatas[i].MeshFilters, AllPartDatas[i].MeshRenderers, AllPartDatas[i].ProductDatas[j]);
                            UpdateAnchor(anchorType, AllPartDatas[i].ProductDatas[j].ProductsDatas.ProductMaterialSet);
                            //UpdateSubPartsVisibility(ActiveProduct.ProductSubParts);
                            UpdateMaterials(AllPartDatas[i].ProductDatas[j].ProductsDatas.ProductMaterialSet);
                            //Debug.Log(ActiveProduct.ProductData.ProductsDatas.ProductNumber);
                            return;
                        }
                    }
                }
            }
        }
    }

    public override void SetPartByMaterialSet(MaterialSetSO updateToMaterialset)
    {
        for (int i = 0; i < AllPartDatas.Count; i++)
        {
            for (int j = 0; j < AllPartDatas[i].ProductDatas.Count; j++)
            {
                if (AllPartDatas[i].ProductDatas[j].ProductsDatas.ProductMaterialSet.SetName.Equals(updateToMaterialset.SetName))
                {
                    //Debug.Log("found material match");
                    if (AllPartDatas[i].ProductDatas[j].ProductsDatas.AnchorType == ActiveProduct.ProductData.ProductsDatas.AnchorType)
                    {
                        //Debug.Log("found anchor match");
                        if (AllPartDatas[i].ProductSubParts == null || Enumerable.SequenceEqual(AllPartDatas[i].ProductSubParts, ActiveProduct.ProductSubParts))
                        {
                            //Debug.Log("found sub parts match");
                            //AllPartDatas[i].ProductSubParts can be null, it should be OK
                            ActiveProduct = new SubpartsProductDataSOPair(AllPartDatas[i].ProductSubParts, AllPartDatas[i].MeshFilters, AllPartDatas[i].MeshRenderers, AllPartDatas[i].ProductDatas[j]);
                            //Changing a material set should not affect anchor or subparts
                            UpdateAnchor(ActiveProduct.ProductData.ProductsDatas.AnchorType, ActiveProduct.ProductData.ProductsDatas.ProductMaterialSet);
                            UpdateMaterials(AllPartDatas[i].ProductDatas[j].ProductsDatas.ProductMaterialSet);
                            //Debug.Log(ActiveProduct.ProductData.ProductsDatas.ProductNumber);
                            return;
                        }
                    }
                }
            }
        }
    }

    public override void SetPartByProductNumber(int productNumber)
    {
        for (int i = 0; i < AllPartDatas.Count; i++)
        {
            for (int j = 0; j < AllPartDatas[i].ProductDatas.Count; j++)
            {
                if (AllPartDatas[i].ProductDatas[j].ProductsDatas.ProductNumber == productNumber)
                {
                    //AllPartDatas[i].ProductSubParts can be null, it should be OK
                    ActiveProduct = new SubpartsProductDataSOPair(AllPartDatas[i].ProductSubParts, AllPartDatas[i].MeshFilters, AllPartDatas[i].MeshRenderers, AllPartDatas[i].ProductDatas[j]);

                    UpdateAnchor(ActiveProduct.ProductData.ProductsDatas.AnchorType, ActiveProduct.ProductData.ProductsDatas.ProductMaterialSet);
                    UpdateMaterials(ActiveProduct.ProductData.ProductsDatas.ProductMaterialSet);
                    UpdateSubPartsVisibility(ActiveProduct.ProductSubParts);
                    return;
                }
            }
        }
    }

    public override void SetPartBySubparts(List<GameObject> subparts)
    {
        
    }

    //Also used after anchor/subpart switch as the old anchor/subpart may have the old base color.
    public override void UpdateMaterials(MaterialSetSO useMaterialSet)
    {
        if (meshMaterialSetters == null || !meshMaterialSetters.Any())
            return;

        for (int i = 0; i < meshMaterialSetters.Count; i++)
        {
            if (meshMaterialSetters[i].enabled == true)
            {
                meshMaterialSetters[i].SetMaterial(useMaterialSet);
            }
        }
    }

    public override void UpdateSubPartsVisibility(List<GameObject> ActivateSubParts)
    {
        if (allSubParts == null || !allSubParts.Any())
            return;

        List<GameObject> turnOffSubparts = allSubParts.Except(ActivateSubParts).ToList();
       
        for (int i = 0; i < turnOffSubparts.Count(); i++)
        {
            turnOffSubparts[i].SetActive(false);
        }
        for (int i = 0; i < ActivateSubParts.Count; i++)
        {
            ActivateSubParts[i].SetActive(true);
        }
    }

    private void UpdateAnchor(AnchorType anchorType, MaterialSetSO useMaterialSet)
    {
        if (anchorGOPairs == null || !anchorGOPairs.Any())
            return;

        for (int i = 0; i < anchorGOPairs.Count; i++)
        {
            if (anchorGOPairs[i] == null)
                return;
            if (anchorGOPairs[i].AnchorTypeEnum == anchorType)
            {
                anchorGOPairs[i].AnchorGameObject.SetActive(true);
                if (anchorGOPairs[i].MeshMaterialSetter != null)
                {
                    anchorGOPairs[i].MeshMaterialSetter.SetMaterial(useMaterialSet);
                }

            }
            else
            {
                anchorGOPairs[i].AnchorGameObject.SetActive(false);
            }
        }
    }
}

[System.Serializable]
public class AnchorTypeGameObjectPair
{
    public AnchorType AnchorTypeEnum;
    [Header("Optional, can be left empty.")]
    public MeshMaterialSetter MeshMaterialSetter;
    public GameObject AnchorGameObject;
}
