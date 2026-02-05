using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(CanvasScaler))]
[RequireComponent(typeof(GraphicRaycaster))]
public class BasePopup : MonoBehaviour
{
    [Header("Base")]
    [SerializeField]
    private Canvas canvas;

    [SerializeField]
    private CanvasScaler scaler;

    [SerializeField]
    private GraphicRaycaster raycaster;

    protected virtual void Awake()
    {
        TryGetComponent(out canvas);
        TryGetComponent(out scaler);
        TryGetComponent(out raycaster);

        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
#if UNITY_STANDALONE
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0f;
#else
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 1f;
#endif
    }

    public virtual void Show()
    {
        canvas.enabled = true;
        raycaster.enabled = true;
    }

    public virtual void Hide()
    {
        canvas.enabled = false;
        raycaster.enabled = false;
    }

    public void SetSortingOrderLayer(int _index)
    {
        canvas.sortingOrder = _index;
    }
}