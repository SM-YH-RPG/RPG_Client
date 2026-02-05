using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MergeCraftNeedItemElement : MonoBehaviour
{
    [SerializeField] private Image _itemIcon;
    [SerializeField] private Image _gradeLine;
    [SerializeField] private TextMeshProUGUI _amountText;

    private NeedItemTemplate template;

    private void Awake()
    {
        PlayerManager.Instance.Inventory.OnInventoryChanged += OnInventoryChanged;
    }

    private void OnDestroy()
    {
        PlayerManager.Instance.Inventory.OnInventoryChanged -= OnInventoryChanged;
    }

    public void InitNeedElement(NeedItemTemplate needItem, ItemConfigData itemConfig)
    {
        template = needItem;
        _itemIcon.sprite = itemConfig.Sprite;
        _gradeLine.color = ItemDataManager.Instance.GetGradeColor((int)itemConfig.template.Grade);
        OnInventoryChanged();
    }

    private void OnInventoryChanged()
    {
        _amountText.text = $"{InventoryManager.Instance.GetItemCount(template.Category, template.ItemIndex)}/{template.NeedAmount}";
    }
}
