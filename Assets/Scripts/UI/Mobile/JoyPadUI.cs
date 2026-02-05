using UnityEngine;
using UnityEngine.EventSystems;

public class JoyPadUI : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    private RectTransform padRect;

    [SerializeField]
    private RectTransform stickRect;

    private IMoveTarget target;

    public void UpdateMoveTarget(IMoveTarget _target)
    {
        target = _target;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(padRect, eventData.position, eventData.pressEventCamera, out pos);
        pos = Vector2.ClampMagnitude(pos, padRect.sizeDelta.x * 0.5f);
        stickRect.anchoredPosition = pos;

        Vector2 normalized = pos / (padRect.sizeDelta.x * 0.5f);
#if !UNITY_STANDALONE
        target.SetDirection(normalized);
#endif
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        stickRect.anchoredPosition = Vector2.zero;

#if !UNITY_STANDALONE
        target.SetDirection(Vector2.zero);
#endif
    }
}
