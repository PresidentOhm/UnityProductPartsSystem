using UnityEngine;

/// <summary>
/// Used to add an encapsulating box collider to a gameobject with many meshes
/// Parent prefab should have same transform position as part.
/// </summary>
public class DynamicPartsPrefabBoxCollider : MonoBehaviour
{
    private ProductPrefabBoundsCalculator productPrefabBoundsCalculator;
    private BoxCollider dynamicBoxCollider;
    private bool initialized = false;

    private void Awake()
    {
        productPrefabBoundsCalculator = (ProductPrefabBoundsCalculator)GetComponent(typeof(ProductPrefabBoundsCalculator));
    }

    private void OnEnable()
    {
        productPrefabBoundsCalculator.OnUpdatePartsBounds += Instance_OnUpdatePartsBounds;
    }

    private void OnDisable()
    {
        productPrefabBoundsCalculator.OnUpdatePartsBounds -= Instance_OnUpdatePartsBounds;
    }

    private void Instance_OnUpdatePartsBounds(GameObject go, Bounds bounds)
    {
        if(!initialized)
        {
            initialized = true;
            AddPartEncapsulatingBoxCollider(bounds);
        }
        else
        {
            UpdateBoxColliderSize(bounds);
        }
    }
        
    public void AddPartEncapsulatingBoxCollider(Bounds bounds)
    {
        dynamicBoxCollider = gameObject.AddComponent(typeof(BoxCollider)) as BoxCollider;
        UpdateBoxColliderSize(bounds);
    }

    public void UpdateBoxColliderSize(Bounds bounds)
    {
        if (dynamicBoxCollider.size == bounds.size && dynamicBoxCollider.center == new Vector3(0, bounds.size.y * 0.5f, 0))
            return;

        dynamicBoxCollider.center = new Vector3(0, bounds.size.y * 0.5f, 0);
        dynamicBoxCollider.size = bounds.size;
    }
}
