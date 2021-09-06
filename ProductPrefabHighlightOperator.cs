#if UNITY_EDITOR
using System.Collections.Generic;
#endif
using UnityEngine;

/// <summary>
/// Used with <see cref="DynamicSelectionHighlighter"/> to set highlight position.
/// </summary>
public class ProductPrefabHighlightOperator : MonoBehaviour
{
    [SerializeField] private float groundHighlighterDiameterOffset;
    [SerializeField] private float topHighlighterYAxisOffset;

    private ProductPrefabBoundsCalculator productPrefabBoundsCalculator;

    private void Awake()
    {
        productPrefabBoundsCalculator = (ProductPrefabBoundsCalculator)GetComponent(typeof(ProductPrefabBoundsCalculator));
    }

    private void OnEnable()
    {
        productPrefabBoundsCalculator.OnUpdatePartsBounds += SetHiglighter;        
    }

    private void OnDisable()
    {
        productPrefabBoundsCalculator.OnUpdatePartsBounds -= SetHiglighter;
    }

    /// <summary>
    /// Make this public or use an OnSetHighlighter event
    /// </summary>
    private void SetHiglighter(GameObject go, Bounds bounds)
    {
        EventBus.Instance.UpdateHighlight(transform, bounds, groundHighlighterDiameterOffset, topHighlighterYAxisOffset);
    }

#if UNITY_EDITOR
    [ContextMenu(nameof(TestHighlightSettings))]
    public void TestHighlightSettings()
    {
        DynamicPartEncapsulatingBox dynamicPartEncapsulatingBox = new DynamicPartEncapsulatingBox();
        GameObject highlighter = GameObject.Find("DynamicSelectionHighlighter");
        BasePartDataManager[] parts = GetComponentsInChildren<BasePartDataManager>();
        List<BasePartDataManager> partsList = new List<BasePartDataManager>();
        foreach (var item in parts)
        {
            partsList.Add(item);
        }
        Bounds bounds = dynamicPartEncapsulatingBox.GetPartsRenderersBoundingBox(partsList);
        Vector3 diameter = bounds.size.x > bounds.size.z
            ? new Vector3(bounds.size.x + groundHighlighterDiameterOffset, bounds.size.x + groundHighlighterDiameterOffset, bounds.size.x + groundHighlighterDiameterOffset)
            : new Vector3(bounds.size.z + groundHighlighterDiameterOffset, bounds.size.z + groundHighlighterDiameterOffset, bounds.size.z + groundHighlighterDiameterOffset);

        highlighter.transform.position = transform.position;
        highlighter.transform.localScale = diameter;
    }
#endif
}
