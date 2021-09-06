using UnityEngine;

/// <summary>
/// Calculates the bounds of all parts in a prefab.
/// Raises event carrying Bounds.
/// </summary>
public class ProductPrefabBoundsCalculator : MonoBehaviour
{
    private ProductPrefabDataManager productPrefabDataManager;
    private DynamicPartEncapsulatingBox dynamicPartEncapsulatingBox = new DynamicPartEncapsulatingBox();

    private Bounds currentBounds;
    private bool initialized;

    public delegate void UpdatePartsBoundsCallback(GameObject go, Bounds bounds);
    public event UpdatePartsBoundsCallback OnUpdatePartsBounds;

    private void Awake()
    {
        productPrefabDataManager = (ProductPrefabDataManager)GetComponent(typeof(ProductPrefabDataManager));
        EventBus.Instance.OnSelectGO += Instance_OnSelectGO;
        productPrefabDataManager.OnPrefabInitialized += UpdatePartsBounds;
        productPrefabDataManager.OnPartAdded += PartsAltered;
        productPrefabDataManager.OnPartRemoved += PartsAltered;
    }

    private void OnDisable()
    {
        EventBus.Instance.OnSelectGO -= Instance_OnSelectGO;
        productPrefabDataManager.OnPrefabInitialized -= UpdatePartsBounds;
        productPrefabDataManager.OnPartAdded -= PartsAltered;
        productPrefabDataManager.OnPartRemoved -= PartsAltered;
    }

    private void Instance_OnSelectGO(GameObject go, ProductPrefabDataManager prefabDataManager)
    {
        if (go.transform.parent == this.transform.parent)
        {
            if (!initialized)
            {
                initialized = true;
            }
            else
            {
                OnUpdatePartsBounds?.Invoke(gameObject, currentBounds);
            }
        }
    }

    private void PartsAltered(BasePartDataManager partPrefab)
    {
        UpdatePartsBounds();
    }

    private void UpdatePartsBounds()
    {
        currentBounds = dynamicPartEncapsulatingBox.GetPartsRenderersBoundingBox(productPrefabDataManager.Parts);
        OnUpdatePartsBounds?.Invoke(gameObject, currentBounds);
    }
}
