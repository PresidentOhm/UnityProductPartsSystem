using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


/// <summary>
/// All subparts should be children of this.gameobject.
/// Anchors should not be child to this.gameobject.
/// </summary>
public abstract class BasePartDataManager : MonoBehaviour
{
    [Header("All subparts should be children to this.", order=0)]
    [Header("Anchors should not be children to this.", order=1)]
    [Space(order=2)]
    [SerializeField] private List<SubpartsProductDataSOPairs> allPartDatas = new List<SubpartsProductDataSOPairs>();
    public List<SubpartsProductDataSOPairs> AllPartDatas { get => allPartDatas; set => allPartDatas = value; }

    [ReadOnly][SerializeField] protected List<GameObject> allSubParts = new List<GameObject>();

    [ReadOnly][SerializeField] protected List<MeshMaterialSetter> meshMaterialSetters = new List<MeshMaterialSetter>();

    private SubpartsProductDataSOPair activeProduct;
    public SubpartsProductDataSOPair ActiveProduct { get => activeProduct; set => activeProduct = value; }


    public abstract void SetPartByMaterialSet(MaterialSetSO updateToMaterialset);

    public abstract void SetPartByProductNumber(int productNumber);

    public abstract void SetPartBySubparts(List<GameObject> subparts);

    public abstract void UpdateMaterials(MaterialSetSO useMaterialSet);

    public abstract void UpdateSubPartsVisibility(List<GameObject> ActivateSubParts);

#if UNITY_EDITOR
    //References all MeshMaterialSetters and fixes their MeshRenderer reference (should be children of this).
    //References all subparts (should be children of this).
    [ContextMenu(nameof(ReferenceAndGetAllMeshMaterialSetters))]
    public void ReferenceAndGetAllMeshMaterialSetters()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }

        meshMaterialSetters.Clear();
        meshMaterialSetters = GetComponentsInChildren<MeshMaterialSetter>().ToList();
        for (int i = 0; i < meshMaterialSetters.Count; i++)
        {
            meshMaterialSetters[i].MeshRenderer = meshMaterialSetters[i].gameObject.GetComponent<MeshRenderer>();
        }
        allSubParts.Clear();


        MeshRenderer[] subpartRenderers = GetComponentsInChildren<MeshRenderer>();
        foreach (var subpartsubpartRenderer in subpartRenderers)
        {
            allSubParts.Add(subpartsubpartRenderer.gameObject);
            subpartsubpartRenderer.gameObject.SetActive(false);
        }
    }
#endif
}
