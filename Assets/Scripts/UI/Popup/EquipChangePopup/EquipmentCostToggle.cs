using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class EquipmentCostToggle : MonoBehaviour
{
    [SerializeField] private Toggle _toggle;
    [SerializeField] private Image _selectLine;
    [SerializeField] private TextMeshProUGUI _costText;

    private EEquipCost _currentCost;
    private Action<EEquipCost> _OnChangeCostFilterCallback;    

    private void Awake()
    {
        _selectLine.enabled = false;
        _toggle.onValueChanged.AddListener(OnSelectToggle);
    }

    public void InitCostData(EEquipCost cost, Action<EEquipCost> _callback)
    {
        _currentCost = cost;
        _OnChangeCostFilterCallback = _callback;        
    }

    public void OnSelectToggle(bool _isOn)
    {
        _selectLine.enabled = _isOn;
        _toggle.isOn = _isOn;
        _toggle.interactable = !_isOn;
        if (_isOn)
        {            
            _OnChangeCostFilterCallback?.Invoke(_currentCost);
        }
    }
}
