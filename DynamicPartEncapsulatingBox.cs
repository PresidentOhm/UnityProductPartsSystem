using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used to find the center or bounds of a part
/// Also used to add an encapsulating box collider to a gameobject with many meshes
/// </summary>
public class DynamicPartEncapsulatingBox
{
    //Returns the local position of a parts center
    public Vector3 FindCenterOfPart(BasePartDataManager basePartDataManager)
    {
        return GetPartMeshFilterBoundingBox(basePartDataManager).center;
    }

    public Bounds GetPartMeshFilterBoundingBox(BasePartDataManager basePartDataManager)
    {
        Bounds wholePartBounds = new Bounds();
        for (int j = 0; j < basePartDataManager.ActiveProduct.MeshFilters.Count; j++)
        {
            if (j == 0)
            {
                wholePartBounds = basePartDataManager.ActiveProduct.MeshFilters[j].mesh.bounds;
            }
            else
            {
                wholePartBounds.Encapsulate(basePartDataManager.ActiveProduct.MeshFilters[j].mesh.bounds);
            }
        }
        return wholePartBounds;
    }

    public Bounds GetPartsRenderersBoundingBox(List<BasePartDataManager> basePartDataManagers)
    {
        Bounds wholePartBounds = new Bounds();
        for (int i = 0; i < basePartDataManagers.Count; i++)
        {
            if (basePartDataManagers[i].ActiveProduct == null)
                continue;
            for (int j = 0; j < basePartDataManagers[i].ActiveProduct.MeshRenderers.Count; j++)
            {
                if (i == 0 && j == 0)
                {
                    //Bounds tempBounds = basePartDataManagers[i].ActiveProduct.MeshFilters[j].mesh.bounds;
                    //Vector3 centerOfMesh = new Vector3(basePartDataManagers[i].ActiveProduct.MeshFilters[j].transform.position.x, basePartDataManagers[i].ActiveProduct.MeshFilters[j].transform.position.y + basePartDataManagers[i].ActiveProduct.MeshFilters[j].transform.localScale.y * 0.25f, basePartDataManagers[i].ActiveProduct.MeshFilters[j].transform.position.z);
                    //tempBounds.center = centerOfMesh;
                    wholePartBounds = basePartDataManagers[i].ActiveProduct.MeshRenderers[j].bounds;
                }
                else
                {
                    //Bounds tempBounds = basePartDataManagers[i].ActiveProduct.MeshFilters[j].mesh.bounds;
                    //Vector3 centerOfMesh = new Vector3(basePartDataManagers[i].ActiveProduct.MeshFilters[j].transform.position.x, basePartDataManagers[i].ActiveProduct.MeshFilters[j].transform.position.y + basePartDataManagers[i].ActiveProduct.MeshFilters[j].transform.localScale.y * 0.25f, basePartDataManagers[i].ActiveProduct.MeshFilters[j].transform.position.z);
                    //tempBounds.center = centerOfMesh;
                    wholePartBounds.Encapsulate(basePartDataManagers[i].ActiveProduct.MeshRenderers[j].bounds);
                }
            }
        }

        //float partHeight = basePartDataManagers[1].transform.localScale.y;
        //Vector3 highpoint = new Vector3(basePartDataManagers[1].transform.localPosition.x, basePartDataManagers[1].transform.localPosition.y + partHeight * 0.25f, basePartDataManagers[1].transform.localPosition.z);
        //wholePartBounds.Encapsulate(highpoint);

        return wholePartBounds;
    }
}
