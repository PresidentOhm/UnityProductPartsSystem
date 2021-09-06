using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Used by parts that have multiple material sets
/// </summary>
public class MaterialPartDataManager : BasePartDataManager
{
    //Receives material and anchor events data to update its data
    public override void SetPartByMaterialSet(MaterialSetSO updateToMaterialset)
    {
        for (int i = 0; i < AllPartDatas.Count; i++)
        {
            for (int j = 0; j < AllPartDatas[i].ProductDatas.Count; j++)
            {
                if (AllPartDatas[i].ProductDatas[j].ProductsDatas.ProductMaterialSet.SetName.Equals(updateToMaterialset.SetName))
                {
                    //Debug.Log("found material match");
                    //Can be uncommented if neede later
                    //if (AllPartDatas[i].ProductDatas[j].ProductsDatas.AnchorType == ActiveProduct.ProductData.ProductsDatas.AnchorType)
                    //{
                    //    Debug.Log("found anchor match");
                        if (AllPartDatas[i].ProductSubParts == null || Enumerable.SequenceEqual(AllPartDatas[i].ProductSubParts, ActiveProduct.ProductSubParts))
                        {
                            //Debug.Log("found sub parts match");
                            //AllPartDatas[i].ProductSubParts can be null, it should be OK
                            ActiveProduct = new SubpartsProductDataSOPair(AllPartDatas[i].ProductSubParts, AllPartDatas[i].MeshFilters, AllPartDatas[i].MeshRenderers, AllPartDatas[i].ProductDatas[j]);
                            //Changing a material set should not affect anchor or subparts
                            UpdateMaterials(AllPartDatas[i].ProductDatas[j].ProductsDatas.ProductMaterialSet);
                            //Debug.Log(ActiveProduct.ProductData.ProductsDatas.ProductNumber);
                            return;
                        }
                    //}
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
}
