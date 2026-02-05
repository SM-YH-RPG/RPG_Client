using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PresetToggle : MonoBehaviour
{
    [SerializeField]
    private Toggle _toggle;

    [SerializeField]
    private TextMeshProUGUI _toggleText;

    private int _presetIndex;

    private Action<int> _OnChangePresetIndex;

    private void Awake()
    {
        _toggle.onValueChanged.AddListener(OnValueChnageToggleTextColor);
        _toggle.onValueChanged.AddListener(OnPrestNumberChange);

        OnValueChnageToggleTextColor(_toggle.isOn);
    }

    public void InitTogglePreset(int index, Action<int> callback)
    {
        _presetIndex = index;
        _OnChangePresetIndex = callback;
    }

    public void SetToggleText(int _index)
    {
        _toggleText.text = $"{_index}";
    }

    public void SetToggleIsOn(bool _isOn)
    {
        _toggle.isOn = _isOn;
    }

    private void OnValueChnageToggleTextColor(bool isOn)
    {
        _toggleText.color = isOn ? Color.yellow : Color.white;
    }

    private void OnPrestNumberChange(bool isOn)
    {        
        if (isOn)
        {
            _OnChangePresetIndex?.Invoke(_presetIndex);
        }
    }
}
