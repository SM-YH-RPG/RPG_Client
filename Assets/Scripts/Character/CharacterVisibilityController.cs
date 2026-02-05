using UnityEngine;

public class CharacterVisibilityController : MonoBehaviour
{
    private SkinnedMeshRenderer[] _skinRenderers;
    private MeshRenderer[] _meshRenderers;
    private Animator _animator;

    private void Awake()
    {
        TryGetComponent(out _animator);
        _skinRenderers = GetComponentsInChildren<SkinnedMeshRenderer>(true);
        _meshRenderers = GetComponentsInChildren<MeshRenderer>(true);
        InGameManager.Instance.OnActiveControlInShop += OnActiveInteractShopState;
    }

    private void OnDestroy()
    {
        InGameManager.Instance.OnActiveControlInShop -= OnActiveInteractShopState;
    }

    private void OnActiveInteractShopState(bool isActive)
    {
        int count = _skinRenderers.Length;
        for (int i = 0; i < count; i++)
        {
            _skinRenderers[i].enabled = isActive;
        }

        count = _meshRenderers.Length;
        for (int i = 0; i < count; i++)
        {
            _meshRenderers[i].enabled = isActive;
        }

        _animator.enabled = isActive;
    }
}
