using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class MergeItemElement : MonoBehaviour
{
    [SerializeField] private Toggle _toggle;
    [SerializeField] private Image _itemIcon;
    [SerializeField] private Image _gradeLine;
    [SerializeField] private Image _selectLine;
    [SerializeField] private TextMeshProUGUI _itemName;

    private Action<MergeItemConfigData> onSelectToggleDetailViewUpdata;
    private MergeItemConfigData _mergeConfig;

    private void Awake()
    {        
        _toggle.onValueChanged.AddListener(OnSelectToggle);
    }

    public void InitMergeItemElement(MergeItemConfigData mergeConfig, ItemConfigData itemConfig, ToggleGroup group, Action<MergeItemConfigData> callback)
    {
        _mergeConfig = mergeConfig;

        _selectLine.enabled = false;
        _toggle.group = group;
        _itemIcon.sprite = itemConfig.Sprite;
        _gradeLine.color = ItemDataManager.Instance.GetGradeColor((int)itemConfig.template.Grade);
        _itemName.text = itemConfig.Name;
        onSelectToggleDetailViewUpdata = callback;
    }

    public void OnSelectToggle(bool isOn)
    {
        _selectLine.enabled = isOn;
        _toggle.isOn = isOn;
        if (isOn)
        {
            onSelectToggleDetailViewUpdata?.Invoke(_mergeConfig);
        }
    }
}
