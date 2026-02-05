using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnhanceMaterialItemElement : MonoBehaviour
{
    [SerializeField] private Image _itemIcon;
    [SerializeField] private Image _gradeLine;
    [SerializeField] private TextMeshProUGUI _amountText;

    private ItemConfigData _itemConfig;
    private int _needAmount;

    private void Awake()
    {
        PlayerManager.Instance.Inventory.OnInventoryChanged += OnInventoryChanged;
    }

    private void OnDestroy()
    {
        PlayerManager.Instance.Inventory.OnInventoryChanged -= OnInventoryChanged;
    }

    public void InitMaterialItem(ItemConfigData config, int needAmount)
    {
        _itemConfig = config;
        _needAmount = needAmount;
        _itemIcon.sprite = config.Sprite;
        _gradeLine.color = ItemDataManager.Instance.GetGradeColor((int)config.template.Grade);
        OnInventoryChanged();
    }

    private void OnInventoryChanged()
    {
        _amountText.text = $"{InventoryManager.Instance.GetItemCount(_itemConfig.template.Category, _itemConfig.template.Index)}/{_needAmount}";
    }
}
