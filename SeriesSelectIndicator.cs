using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used to control the series indicator of a prefab.
/// Place this on the same GameObject as the <see cref="productPrefabDataManager"/>.
/// </summary>
public class SeriesSelectIndicator : MonoBehaviour
{
    [SerializeField] private GameObject seriesIndicator;
    [SerializeField] private ProductPrefabDataManager productPrefabDataManager;
    private ProductPrefabBoundsCalculator productPrefabBoundsCalculator;

    private Bounds currentBounds;

    private void Awake()
    {
        seriesIndicator.SetActive(false);
        productPrefabBoundsCalculator = (ProductPrefabBoundsCalculator)GetComponent(typeof(ProductPrefabBoundsCalculator));
    }

    private void OnEnable()
    {
        EventBus.Instance.OnDeselectGO += Instance_OnDeselectGO; ;
        EventBus.Instance.OnSeriesSelect += Instance_OnSeriesSelect;
        EventBus.Instance.OnSeriesDeselect += TurnOffIndicator;
        productPrefabBoundsCalculator.OnUpdatePartsBounds += ProductPrefabBoundsCalculator_OnUpdatePartsBounds;
    }

    private void OnDisable()
    {
        EventBus.Instance.OnDeselectGO -= Instance_OnDeselectGO;
        EventBus.Instance.OnSeriesSelect -= Instance_OnSeriesSelect;
        EventBus.Instance.OnSeriesDeselect -= TurnOffIndicator;
        productPrefabBoundsCalculator.OnUpdatePartsBounds -= ProductPrefabBoundsCalculator_OnUpdatePartsBounds;
    }

    private void Instance_OnDeselectGO(GameObject go)
    {
        TurnOffIndicator();
    }

    private void TurnOffIndicator()
    {
        seriesIndicator.SetActive(false);
    }

    private void Instance_OnSeriesSelect(string series)
    {
        //check if prefab series matches event series
        if(productPrefabDataManager.Series == series)
        {
            seriesIndicator.SetActive(true);
        }
        else
        {
            seriesIndicator.SetActive(false);
        }
    }

    private void ProductPrefabBoundsCalculator_OnUpdatePartsBounds(GameObject go, Bounds bounds)
    {
        //Debug.Log(bounds.center);
        if (currentBounds.center == bounds.center)
            return;
        currentBounds.center = bounds.center;

        //If correct this should only move relatively to the parent == small adjustment.
        seriesIndicator.transform.position = seriesIndicator.transform.InverseTransformVector(bounds.center);
    }
}
