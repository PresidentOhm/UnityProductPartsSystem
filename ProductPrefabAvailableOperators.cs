using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ProductPrefabDataManager))]
public class ProductPrefabAvailableOperators : MonoBehaviour
{
    [SerializeField] private bool hasMaterialSetCapacity;
    [SerializeField] private bool hasAnchorCapacity;
    [SerializeField] private bool hasAccessoryCapacity;
    [SerializeField] private bool hasGrowCapacity;
    [SerializeField] private bool hasSnapCapacity;

    public bool HasMaterialSetCapacity { get => hasMaterialSetCapacity; set => hasMaterialSetCapacity = value; }
    public bool HasAnchorCapacity { get => hasAnchorCapacity; set => hasAnchorCapacity = value; }
    public bool HasAccessoryCapacity { get => hasAccessoryCapacity; set => hasAccessoryCapacity = value; }
    public bool HasGrowCapacity { get => hasGrowCapacity; set => hasGrowCapacity = value; }
    public bool HasSnapCapacity { get => hasSnapCapacity; set => hasSnapCapacity = value; }



#if UNITY_EDITOR
    [ContextMenu(nameof(SetCapacities))]
    public void SetCapacities()
    {
        HasMaterialSetCapacity = TryGetComponent<ProductPrefabMaterialSetOperator>(out ProductPrefabMaterialSetOperator material);
        HasAnchorCapacity = TryGetComponent<ProductPrefabAnchorTypeOperator>(out ProductPrefabAnchorTypeOperator anchor);
        HasAccessoryCapacity = TryGetComponent<ProductPrefabAccessoryOperator>(out ProductPrefabAccessoryOperator accessory);
        HasGrowCapacity = TryGetComponent<SnapGrowBuilder>(out SnapGrowBuilder grow);
        HasSnapCapacity = TryGetComponent<SnapSystem>(out SnapSystem snap);
    }

#endif
}
