using UnityEngine;

public abstract class InteractionObject : ClientPlacementObjectBase, IInteractable
{
    [SerializeField]
    private string _interactionTitle = "Default Interaction";
    public string InteractionTitle => _interactionTitle;

    [SerializeField]
    protected Renderer _renderer = null;

    [SerializeField]
    protected Collider _collider = null;

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public virtual void Interact()
    {

    }
}