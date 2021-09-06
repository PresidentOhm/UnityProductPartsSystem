using UnityEngine;

public class MeshMaterialSetter : MonoBehaviour
{
    /// <summary>
    /// The indexes of a part series <see cref="MaterialSet.Materials"/> this renderer will use when
    /// responding to a color switching event.
    /// The <see cref="materialSetIndexes"/> length must match the <see cref="meshRenderer"/> shared materials length
    /// or this will return without changing any materials.
    /// </summary>
    [SerializeField] private int[] materialSetIndexes;

    [SerializeField] private MeshRenderer meshRenderer;

    public MeshRenderer MeshRenderer { get => meshRenderer; set => meshRenderer = value; }


    public void SetMaterial(MaterialSetSO materialSet)
    {
        if (materialSetIndexes.Length != meshRenderer.sharedMaterials.Length)
        {
            return;
        }

        if (materialSetIndexes.Length == 1)
        {
            meshRenderer.sharedMaterial = materialSet.Materials[materialSetIndexes[0]];
        }
        else if (materialSetIndexes.Length > 1)
        {
            Material[] materials = new Material[materialSetIndexes.Length];
            int addedMaterialsCounter = 0;
            for (int i = 0; i < materialSetIndexes.Length; i++)
            {
                materials[addedMaterialsCounter] = materialSet.Materials[materialSetIndexes[i]];
                ++addedMaterialsCounter;
            }

            meshRenderer.sharedMaterials = materials;
        }
    }
}