using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Used by parts that have only one product number associated with the part
/// </summary>
public class SimplePartDataManager : BasePartDataManager
{
    public override void SetPartByMaterialSet(MaterialSetSO updateToMaterialset)
    {
        //ActiveProduct = new SubpartsProductDataSOPair(AllPartDatas[0].ProductSubParts, AllPartDatas[0].MeshFilters, AllPartDatas[0].MeshRenderers, AllPartDatas[0].ProductDatas[0]);
    }

    public override void SetPartByProductNumber(int productNumber)
    {
        ActiveProduct = new SubpartsProductDataSOPair(AllPartDatas[0].ProductSubParts, AllPartDatas[0].MeshFilters, AllPartDatas[0].MeshRenderers, AllPartDatas[0].ProductDatas[0]);
        UpdateSubPartsVisibility(ActiveProduct.ProductSubParts);
    }

    public override void SetPartBySubparts(List<GameObject> subparts)
    {
        //ActiveProduct = new SubpartsProductDataSOPair(AllPartDatas[0].ProductSubParts, AllPartDatas[0].MeshFilters, AllPartDatas[0].MeshRenderers, AllPartDatas[0].ProductDatas[0]);
    }

    public override void UpdateMaterials(MaterialSetSO useMaterialSet)
    {
        
    }


    public override void UpdateSubPartsVisibility(List<GameObject> ActivateSubParts)
    {
        if (allSubParts == null || allSubParts.Count == 0)
            return;

        List<GameObject> turnOffSubparts = allSubParts.Except(ActivateSubParts).ToList();

        if(turnOffSubparts != null && turnOffSubparts.Count != 0)
        {
            for (int i = 0; i < turnOffSubparts.Count(); i++)
            {
                turnOffSubparts[i].SetActive(false);
            }
        }
        for (int i = 0; i < ActivateSubParts.Count; i++)
        {
            ActivateSubParts[i].SetActive(true);
        }
    }
}
