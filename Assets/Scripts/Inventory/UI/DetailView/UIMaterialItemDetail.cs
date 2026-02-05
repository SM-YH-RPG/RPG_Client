using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIMaterialItemDetail : UIItemDetailBase
{
    [SerializeField] private TextMeshProUGUI _categoryText;
    [SerializeField] private TextMeshProUGUI _currentCount;    
    [SerializeField] private TextMeshProUGUI _itemDescText;
    [SerializeField] private Image _itemGradeLine;
    [SerializeField] private Image _itemCategoryImage;

    public override void UpdateSelectView(BaseInventoryItem data)
    {
        if (data == null)
        {
            EmptyItemData();
            return;
        }

        ItemConfigData config = ItemDataManager.Instance.GetItemConfigData(EItemCategory.Material, data.TemplateId);
        _itemImage.enabled = true;
        _itemCategoryImage.enabled = true;
        _itemImage.sprite = config.Sprite;
        _categoryText.text = "재료";
        _currentCount.text = $"보유{InventoryManager.Instance.GetItemCount(EItemCategory.Material, data.TemplateId)}";
        _itemName.text = config.Name;
        _itemDescText.text = config.Description;
        _itemGradeLine.color = ItemDataManager.Instance.GetGradeColor((int)data.Grade);
    }

    private void EmptyItemData()
    {
        _itemImage.enabled = false;
        _itemCategoryImage.enabled = false;
        _categoryText.text = string.Empty;
        _currentCount.text = string.Empty;
        _itemName.text = string.Empty;
        _itemDescText.text = string.Empty;
        _itemGradeLine.color = Color.white;
    }
}
