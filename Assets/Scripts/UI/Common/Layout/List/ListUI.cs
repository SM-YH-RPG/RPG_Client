using Unity.VisualScripting;
using UnityEngine;

public enum EAlignDirection
{
    UP,
    DOWN,
    LEFT,
    RIGHT
}

[ExecuteInEditMode]
public class ListUI : MonoBehaviour
{
    [SerializeField]
    private EAlignDirection _eAlignDirection = EAlignDirection.LEFT;

    [SerializeField]
    private float _space;

    private Transform _list = null;

    private RectTransform[] _childList = null;

    private void Awake()
    {
        Init();
        UpdateAlignDirection();
    }

    public void Init()
    {
        TryGetComponent(out _list);
        int childCount = _list.childCount;
        _childList = new RectTransform[childCount];

        for (int i = 0; i < childCount; i++)
        {
            var child = _list.GetChild(i);
            child.TryGetComponent(out _childList[i]);
        }
    }

    private void UpdateAlignDirection()
    {
        if (_childList == null || _childList.Length < 2)
            return;

        switch (_eAlignDirection)
        {
            case EAlignDirection.UP:
                UpdateAlignDirectionY(1);
                break;
            case EAlignDirection.DOWN:
                UpdateAlignDirectionY(-1);
                break;
            case EAlignDirection.RIGHT:
                UpdateAlignDirectionX(1);
                break;
            case EAlignDirection.LEFT:
                UpdateAlignDirectionX(-1);
                break;
        }
    }

    private void UpdateAlignDirectionX(int direction)
    {
        _childList[0].anchoredPosition = Vector3.zero;
        for (int i = 1; i < _childList.Length; i++)
        {
            var prev = _childList[i - 1];
            var current = _childList[i];

            var pos = current.anchoredPosition;
            pos.x = prev.anchoredPosition.x + ((prev.rect.width + _space) * direction);
            pos.y = 0f;
            current.anchoredPosition = pos;
        }
    }

    private void UpdateAlignDirectionY(int direction)
    {
        _childList[0].anchoredPosition = Vector2.zero;
        for (int i = 1; i < _childList.Length; i++)
        {
            var prev = _childList[i - 1];
            var current = _childList[i];

            var pos = current.anchoredPosition;
            pos.x = 0f;
            pos.y = prev.anchoredPosition.y + ((prev.rect.height + _space) * direction);
            current.anchoredPosition = pos;
        }
    }

    private void OnValidate()
    {
        Init();
        UpdateAlignDirection();
    }
}
