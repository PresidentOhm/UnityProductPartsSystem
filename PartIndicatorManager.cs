using UnityEngine;

//Used to manage what part is hichlighted by the LineRendererBoxDrawer
public class PartIndicatorManager : MonoBehaviour
{
    [SerializeField] private Transform partIndicator;
    [SerializeField] private LineRendererBoxDrawer lineRendererBoxDrawer;
    private DynamicPartEncapsulatingBox dynamicEncapsulatingBox = new DynamicPartEncapsulatingBox();

    private void Awake()
    {
        partIndicator.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        EventBus.Instance.OnPartSelected += TurnOnIndicator;
    }

    private void OnDisable()
    {
        EventBus.Instance.OnPartSelected -= TurnOnIndicator;
        EventBus.Instance.OnPartDeselected -= TurnOffIndicator;
        EventBus.Instance.OnDeselectGO -= TurnOffIndicator;
    }

    /// <summary>
    /// Turns on the indicator and sets it to the first part available for editing
    /// </summary>
    /// <param name="basePartDataManager">This should be Parts[0] of materialset or accessories Parts list</param>
    public void TurnOnIndicator(BasePartDataManager basePartDataManager)
    {
        UpdateIndicatorPos(basePartDataManager);
        partIndicator.gameObject.SetActive(true);
        EventBus.Instance.OnPartDeselected += TurnOffIndicator;
        EventBus.Instance.OnDeselectGO += TurnOffIndicator;
    }

    public void TurnOffIndicator(GameObject go)
    {
        partIndicator.gameObject.SetActive(false);
        partIndicator.parent = this.transform;
        //This might cause problems as this might happen twice
        EventBus.Instance.OnPartDeselected -= TurnOffIndicator;
        EventBus.Instance.OnDeselectGO -= TurnOffIndicator;
    }

    //Triggered by brush / accessories button/event
    public void TurnOffIndicator()
    {
        partIndicator.gameObject.SetActive(false);
        partIndicator.parent = this.transform;
        //This might cause problems as this might happen twice
        EventBus.Instance.OnPartDeselected -= TurnOffIndicator;
        EventBus.Instance.OnDeselectGO -= TurnOffIndicator;
    }

    public void UpdateIndicatorPos(BasePartDataManager basePartDataManager)
    {
        Bounds newBounds = dynamicEncapsulatingBox.GetPartMeshFilterBoundingBox(basePartDataManager);
        newBounds.size = new Vector3(newBounds.size.x + 0.025f, newBounds.size.y + 0.025f, newBounds.size.z + 0.025f);
        lineRendererBoxDrawer.DrawTwelveLineBox(newBounds);
        partIndicator.parent = basePartDataManager.gameObject.transform;
        partIndicator.localPosition = Vector3.zero;
        partIndicator.localRotation = Quaternion.identity;
        //partIndicator.position = dynamicEncapsulatingBoxCollider.FindCenterOfPart(basePartDataManager);
    }
}
