using UnityEngine;

public class BaseScene : MonoBehaviour
{
    private void OnDestroy()
    {
        UIManager.Instance.CachedClear();
    }
}