using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultPopup : BasePopup
{
    #region Inspector
    [SerializeField]
    private Image _itemIcon;

    [SerializeField]
    private TextMeshProUGUI _getText;

    [SerializeField]
    private TextMeshProUGUI _itemCountText;

    [SerializeField]
    private Button _closeButton;
    #endregion

    private IInventoryManagerService _inventoryManagerService => PlayerManager.Instance.Inventory;

    protected override void Awake()
    {
        base.Awake();
        _closeButton.onClick.AddListener(OnClickCloseButton);
    }

    public void InitResultItemData(int categoryIndex, int itemIndex, int buyCount, EShopCategory type)
    {
        var inventoryItemList = _inventoryManagerService.GetInvenItemDataList((EItemCategory)categoryIndex);        
        if (inventoryItemList != null)
        {
            foreach (var item in inventoryItemList)
            {
                if (itemIndex == item.TemplateId)
                {                    
                    ItemConfigData itemConfig = ItemDataManager.Instance.GetItemConfigData(item.Category, item.TemplateId);
                    _itemIcon.sprite = itemConfig.Sprite;
                    _itemCountText.text = $"x{buyCount}";
                    break;
                }
            }
        }
        string resultText = string.Empty;
        switch (type)
        {
            case EShopCategory.Consumption:
            case EShopCategory.Weapon:
            case EShopCategory.Equipment:
            case EShopCategory.Enhancement:
                resultText = "확인";
                break;
            case EShopCategory.Mixer:
                resultText = "합성 완료";
                break;
        }
        _getText.text = resultText;
    }

    private void OnClickCloseButton()
    {
        UIManager.Instance.Hide();
    }
}
