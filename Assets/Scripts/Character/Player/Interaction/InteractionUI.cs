using System.Collections.Generic;
using UnityEngine;


public class InteractionUI : MonoBehaviour
{
    private Queue<InteractionElement> _elementPool = new Queue<InteractionElement>();
    private List<InteractionElement> _activeElements = new List<InteractionElement>();

    [SerializeField]
    private GameObject _uiPanel;

    [SerializeField]
    private Transform _root;

    [SerializeField]
    private GameObject _prefab;

    [SerializeField]
    private int _initialPoolSize = 4;

    private InteractionElement _selectedItem = null;

    public void Init()
    {
        _uiPanel.SetActive(false);

        InitializePool();
    }

    private void InitializePool()
    {
        for (int i = 0; i < _initialPoolSize; i++)
        {
            GameObject elementObject = Instantiate(_prefab, _root);
            if (elementObject.TryGetComponent(out InteractionElement element))
            {
                element.gameObject.SetActive(false);
                _elementPool.Enqueue(element);
            }
            else
            {
                Destroy(elementObject);
            }
        }
    }

    public void HandleTargetsDetected(List<IInteractable> detectedTargets)
    {
        ReturnActiveItemsToPool();

        if (detectedTargets == null || detectedTargets.Count <= 0)
        {
            _uiPanel.SetActive(false);
            return;
        }

        _uiPanel.SetActive(true);

        foreach (var interactable in detectedTargets)
        {
            InteractionElement element = GetElementFromPool();
            if (element != null)
            {
                element.Setup(interactable);
                element.gameObject.SetActive(true);
                _activeElements.Add(element);
            }
        }
    }

    public void HighlightItem(int index)
    {
        if(_selectedItem != null)
        {
            _selectedItem.OnDefault();
        }

        if (index >= 0 && index < _activeElements.Count)
        {
            _selectedItem = _activeElements[index];
            _selectedItem.OnHighlight();
        }
    }

    private InteractionElement GetElementFromPool()
    {
        if (_elementPool.Count > 0)
        {
            return _elementPool.Dequeue();
        }
        else
        {
            return null;
        }
    }

    private void ReturnActiveItemsToPool()
    {
        foreach (var element in _activeElements)
        {
            element.OnDefault();
            element.gameObject.SetActive(false);
            _elementPool.Enqueue(element);
        }

        _activeElements.Clear();
    }
}