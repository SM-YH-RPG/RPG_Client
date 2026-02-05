using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaleShopItemElement : MonoBehaviour
{
    #region Inspector
    [SerializeField] private Toggle _toggle;
    [SerializeField] private Image _selectImage;
    [SerializeField] private Image _itemImage;
    [SerializeField] private Image _itemGradeLine;
    [SerializeField] private TextMeshProUGUI _itemName;
    #endregion

    private ShopItemData _data;    
    private Action<ShopItemData> _onChangeSelectItemNonServerCallback;    

    private void Awake()
    {
        _toggle.onValueChanged.AddListener(OnSelectItem);
    }

    public void InitItemElement(ShopItemData data, ToggleGroup toggleGroup, Action<ShopItemData> callback)
    {
        _data = data;        
        _toggle.group = toggleGroup;
        ItemConfigData config = ItemDataManager.Instance.GetItemConfigData((EItemCategory)data.CategoryIndex, data.Index);
        _itemImage.sprite = config.Sprite;
        _itemName.text = config.Name;
        _itemGradeLine.color = ItemDataManager.Instance.GetGradeColor((int)config.template.Grade);
        _onChangeSelectItemNonServerCallback = callback;
    }

    public void SetToggleSelect()
    {
        _toggle.isOn = true;
        _selectImage.enabled = true;
    }

    private void OnSelectItem(bool isOn)
    {
        _selectImage.enabled = isOn;
        if (isOn)
        {
            _onChangeSelectItemNonServerCallback?.Invoke(_data);
        }
    }
}
