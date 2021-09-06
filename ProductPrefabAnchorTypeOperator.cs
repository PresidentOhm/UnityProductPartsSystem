using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductPrefabAnchorTypeOperator : MonoBehaviour
{
    [SerializeField] private ProductPrefabDataManager productPrefabDataManager;
    public ProductPrefabDataManager ProductPrefabDataManager { get => productPrefabDataManager; set => productPrefabDataManager = value; }

    [SerializeField] private AnchorPartDataManager anchorPartDataManager;
    public AnchorPartDataManager AnchorPartDataManager { get => anchorPartDataManager; set => anchorPartDataManager = value; }


    private void OnEnable()
    {
        EventBus.Instance.OnSelectGO += SelectResponse;
        EventBus.Instance.OnDeselectGO += DeselectResponse;
        EventBus.Instance.OnChangeSeriesAnchor += ChangeSeriesAnchor;
    }

    private void OnDisable()
    {
        EventBus.Instance.OnSelectGO -= SelectResponse;
        EventBus.Instance.OnDeselectGO -= DeselectResponse;
        EventBus.Instance.OnChangeSeriesAnchor -= ChangeSeriesAnchor;
        EventBus.Instance.OnChangePrefabAnchor -= ChangePrefabAnchor;
    }

    private void SelectResponse(GameObject go, ProductPrefabDataManager productPrefabDataManager)
    {
        EventBus.Instance.OnChangePrefabAnchor -= ChangePrefabAnchor;
        if (go != null && this.gameObject.transform.parent == go.transform.parent)
        {
            GetPrefabAnchorData();
            EventBus.Instance.OnChangePrefabAnchor += ChangePrefabAnchor;
        }
    }

    private void DeselectResponse(GameObject go)
    {
        if (go != null && this.gameObject.transform.parent == go.transform.parent)
            EventBus.Instance.OnChangePrefabAnchor -= ChangePrefabAnchor;
    }

    private void GetPrefabAnchorData()
    {
        if (productPrefabDataManager.SeriesAnchorTypes.Count > 0)
            EventBus.Instance.UpdateSelectableAnchorTypes(productPrefabDataManager.SeriesAnchorTypes, AnchorPartDataManager);
    }      

    private void ChangePrefabAnchor(AnchorType anchortype)
    {
        productPrefabDataManager.SetPrefabByAnchor(anchortype);
    }

    private void ChangeSeriesAnchor(AnchorType anchortype, string series)
    {
        if (productPrefabDataManager.Series.Equals(series))        
            productPrefabDataManager.SetPrefabByAnchor(anchortype);
    }

}
