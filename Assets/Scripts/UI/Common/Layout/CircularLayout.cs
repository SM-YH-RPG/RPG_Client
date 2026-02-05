using UnityEngine;

[ExecuteInEditMode]
public class CircularLayout : MonoBehaviour
{
    [SerializeField]
    private float radius = 2.0f;

    [SerializeField]
    private float angleOffset = 0f;

    [SerializeField]
    private float totalAngle = 360f;

    private void Awake()
    {
        UpdateChildrenPosition();
    }

    private void UpdateChildrenPosition()
    {
        int childCount = transform.childCount;
        if (childCount == 0)
            return;


        float angleStep = totalAngle / childCount;

        for (int i = 0; i < childCount; i++)
        {
            var child = transform.GetChild(i);
            var pos = child.localPosition;

            float angle = angleStep * i + angleOffset;
            pos.x = radius * Mathf.Cos(Mathf.Deg2Rad * angle);
            pos.y = radius * Mathf.Sin(Mathf.Deg2Rad * angle);
            child.localPosition = pos;
        }
    }

    private void OnValidate()
    {
        UpdateChildrenPosition();
    }
}
