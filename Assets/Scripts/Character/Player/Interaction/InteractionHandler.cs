using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class InteractionHandler : MonoBehaviour
{
    [SerializeField]
    private PlayerController _playerCtrl;
    
    private InteractionDetector _interactionDetactor;
    private InteractionUI _interactionUI;

    private List<IInteractable> _currentDetectedTargets = new List<IInteractable>();
    private int _selectedIndex = -1;

    public void Init(InteractionUI interactionUI)
    {
        _interactionDetactor = _playerCtrl.InteractionDetector;
        _interactionUI = interactionUI;

        if (_interactionDetactor != null)
        {
            _interactionDetactor.OnTargetsDetected += HandleTargetsDetected;
        }
    }

    private void OnEnable()
    {
        if (_playerCtrl == null)
            return;

        var playerInput = _playerCtrl.GetInput().PlayerActions;
        playerInput.ShortInteract.Enable();
        playerInput.Navigate.Enable();

        playerInput.ShortInteract.performed += OnInteractPerformed;
        playerInput.Navigate.performed += OnScrollPerformed;
        playerInput.Zoom.Disable();
    }

    private void OnDisable()
    {
        if (_playerCtrl == null)
            return;

        var playerInput = _playerCtrl.GetInput().PlayerActions;

        playerInput.ShortInteract.performed -= OnInteractPerformed;
        playerInput.Navigate.performed -= OnScrollPerformed;

        playerInput.ShortInteract.Disable();
        playerInput.Navigate.Disable();
        playerInput.Zoom.Enable();
    }

    private void UpdateInputStateBy(int targetCount)
    {
        enabled = targetCount > 0;
    }

    private void HandleTargetsDetected(List<IInteractable> targets)
    {
        _currentDetectedTargets = targets;
        _interactionUI.HandleTargetsDetected(_currentDetectedTargets);
        if (_currentDetectedTargets == null || _currentDetectedTargets.Count < 1)
        {
            _selectedIndex = -1;
            _interactionUI?.HighlightItem(-1);
        }
        else
        {
            _selectedIndex = 0;
            _interactionUI?.HighlightItem(_selectedIndex);
        }

        UpdateInputStateBy(targets.Count);
    }

    private void OnScrollPerformed(InputAction.CallbackContext context)
    {
        if (_currentDetectedTargets.Count <= 1)
            return;

        float scrollValue = context.ReadValue<Vector2>().y;
        if (scrollValue > 0)
        {
            _selectedIndex--;
        }
        else if (scrollValue < 0)
        {
            _selectedIndex++;
        }

        if (_selectedIndex < 0)
        {
            _selectedIndex = _currentDetectedTargets.Count - 1;
        }
        else if (_selectedIndex >= _currentDetectedTargets.Count)
        {
            _selectedIndex = 0;
        }

        _interactionUI?.HighlightItem(_selectedIndex);
    }

    private void OnInteractPerformed(InputAction.CallbackContext context)
    {
        if(context.interaction is HoldInteraction)
        {

        }
        else
        {
            if (_currentDetectedTargets == null || _currentDetectedTargets.Count == 0)
                return;

            if (_currentDetectedTargets.Count == 1)
            {
                _currentDetectedTargets[0].Interact();
                return;
            }

            if (_selectedIndex >= 0 && _selectedIndex < _currentDetectedTargets.Count)
            {
                _currentDetectedTargets[_selectedIndex].Interact();
            }
        }
    }
}