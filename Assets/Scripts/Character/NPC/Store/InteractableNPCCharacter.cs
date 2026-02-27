using UnityEngine;

public abstract class InteractableNPCCharacter : InteractionObject
{
    #region Inspector    
    [SerializeField]
    private float _lookSpeed;

    [SerializeField]
    private EShopCategory _shopCategory;
    #endregion

    private Transform _target;

    protected NPCInteractionHandler _interactionHandler;

    private IInteractionService _interactionService => PlayerManager.Instance.InteractionService;

    public EShopCategory ShopCategory => _shopCategory;

    private void Awake()
    {
        TryGetComponent(out _interactionHandler);

        _interactionService.OnInteractionSourceChanged += HandleTransformChanged;

    }

    protected virtual void OnDestroy()
    {
        _interactionService.OnInteractionSourceChanged -= HandleTransformChanged;
    }

    public override void Interact()
    {
        if (_interactionHandler != null)
        {
            _interactionHandler.StartLookingAtPlayer(_target);
        }

        _interactionService.NotifyInteractionTargetChanged(transform);
    }

    private void HandleTransformChanged(Transform transform)
    {
        _target = transform;
    }
}
