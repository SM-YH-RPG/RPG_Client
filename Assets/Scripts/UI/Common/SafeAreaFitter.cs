using UnityEngine;

public class SafeAreaFitter : MonoBehaviour
{
    [Header("Apply Safe Area")]
    [SerializeField] private bool _applyLeft = true;
    [SerializeField] private bool _applyRight = true;
    [SerializeField] private bool _applyTop = true;
    [SerializeField] private bool _applyBottom = true;

    private RectTransform _rectTransform;
    private Rect _lastSafeArea = new Rect(0, 0, 0, 0);
    private Vector2 _lastScreenSize = new Vector2(0, 0);
    private ScreenOrientation _lastOrientation = ScreenOrientation.AutoRotation;

    private void Awake()
    {
        TryGetComponent(out _rectTransform);
        ApplySafeArea();
    }

    private void OnEnable()
    {
        ApplySafeArea();
    }

    private void Update()
    {
        // 회전/해상도 변경/노치 상태 변화 대응
        if (_lastSafeArea != Screen.safeArea ||
            _lastScreenSize.x != Screen.width ||
            _lastScreenSize.y != Screen.height ||
            _lastOrientation != Screen.orientation)
        {
            ApplySafeArea();
        }
    }

    private void ApplySafeArea()
    {
        Rect safe = Screen.safeArea;
        _lastSafeArea = safe;
        _lastScreenSize = new Vector2Int(Screen.width, Screen.height);
        _lastOrientation = Screen.orientation;

        Vector2 anchorMin = safe.position;
        Vector2 anchorMax = safe.position + safe.size;

        // 0~1 정규화
        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        // 특정 방향만 적용 옵션
        Vector2 finalMin = _rectTransform.anchorMin;
        Vector2 finalMax = _rectTransform.anchorMax;

        if (_applyLeft) finalMin.x = anchorMin.x;
        if (_applyBottom) finalMin.y = anchorMin.y;
        if (_applyRight) finalMax.x = anchorMax.x;
        if (_applyTop) finalMax.y = anchorMax.y;

        _rectTransform.anchorMin = finalMin;
        _rectTransform.anchorMax = finalMax;

        // 오프셋은 0으로 고정(앵커 기반 맞춤)
        _rectTransform.offsetMin = Vector2.zero;
        _rectTransform.offsetMax = Vector2.zero;
    }
}
