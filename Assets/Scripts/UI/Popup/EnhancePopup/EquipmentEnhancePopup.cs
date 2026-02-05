using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class EquipmentEnhancePopup : EnhancePopupBase
{    
    private List<EnhanceMaterialItemElement> _itemList = new List<EnhanceMaterialItemElement>();
    private EquipmentItem _equipmentItem;
    public void UpdateEquipmentData(EquipmentItem equip)
    {
        if (equip == null)
            return;

        _equipmentItem = equip;        

        ItemConfigData itemConfig = ItemDataManager.Instance.GetItemConfigData(equip.Category, equip.TemplateId);
        _itemPreview.sprite = itemConfig.Sprite;
        _itemName.text = equip.Name;

        RefreshUI();
    }

    private void RefreshUI()
    {
        if (_equipmentItem == null)
            return;

        int tier = _equipmentItem.Tier;
        bool isMaxTier = tier >= MAX_TIER;

        SetTierText(tier);
        UpdateStat(tier);
        CreateMaterialItems(_equipmentItem.Grade, tier);
        UpdateCurrencyValue();

        if (isMaxTier)
        {
            _costValue.text = "0";
            _enhanceButton.interactable = false;
        }
        else
        {
            var enhanceConfig = WeaponDataManager.Instance.WeaponEnhanceConfg.GetEnhanceData(_equipmentItem.Grade, tier);
            _costValue.text = enhanceConfig.Cost.ToString();
            _enhanceButton.interactable = CheckPossibleEnhance(_equipmentItem.Grade, tier);
        }
    }

    protected override void UpdateStat(int tier)
    {
        int statIndex = 0;
        if (tier >= MAX_TIER)
        {
            SetMaxTierStatData(statIndex++, _equipmentItem.RandomMainStatType, _equipmentItem.RandomMainStatValue);
            SetMaxTierStatData(statIndex++, _equipmentItem.MainStatType, _equipmentItem.MainStatValue);
            foreach (var (statType, value) in _equipmentItem.SubStatDict)
            {
                SetMaxTierStatData(statIndex++, statType, value);
            }            
        }
        else
        {
            SetStatData(statIndex++, _equipmentItem.RandomMainStatType, _equipmentItem.RandomMainStatValue);
            SetStatData(statIndex++, _equipmentItem.MainStatType, _equipmentItem.MainStatValue);

            foreach (var (statType, value) in _equipmentItem.SubStatDict)
            {
                SetStatData(statIndex++, statType, value);
            }
        }

        int statCount = _equipmentItem.SubStatDict.Count + 2; // Sub½ºÅÈ + main½ºÅÈ 2°³

        for (int i = statCount; i < _statArray.Length; i++)
        {
            _statArray[i].gameObject.SetActive(false);
        }
    }

    private void SetStatData(int index, EItemStatType type, float value)
    {
        EnhanceStatInfo stat = _statArray[index];
        var nextStat = EquipmentManager.Instance.GetNextTierStat(type, value, _equipmentItem);

        stat.UpdateEnhanceStat(type, value, nextStat);
        stat.gameObject.SetActive(true);
    }

    private void SetMaxTierStatData(int index, EItemStatType type, float value)
    {
        EnhanceStatInfo stat = _statArray[index];
        stat.UpdateMaxTierStat(type, value);
        stat.gameObject.SetActive(true);
    }

    protected override void CreateMaterialItems(EItemGrade grade, int tier)
    {
        if (tier >= MAX_TIER)
        {
            for (int i = 0; i < _itemList.Count; i++)
            {
                _itemList[i].gameObject.SetActive(false);
            }
            _enhanceButton.interactable = false;
            return;
        }
        else
        {
            var materialItemList = EquipmentDataManager.Instance.EquipmentEnhanceConfig.GetEnhanceData(grade, tier).MaterialsArray;
            int itemCount = materialItemList.Length;
            while (_itemList.Count < itemCount)
            {
                GameObject elementObject = Instantiate(_materialPrefab, _createRoot);
                if (elementObject.TryGetComponent(out EnhanceMaterialItemElement item))
                {
                    _itemList.Add(item);
                }
            }

            for (int i = 0; i < _itemList.Count; i++)
            {
                bool isActive = i < itemCount;
                _itemList[i].gameObject.SetActive(isActive);
                if (isActive)
                {
                    var itemConfig = ItemDataManager.Instance.GetItemConfigData(materialItemList[i].Category, materialItemList[i].Index);
                    _itemList[i].InitMaterialItem(itemConfig, materialItemList[i].Amount);
                }
            }           
        }        
    }

    protected override bool CheckPossibleEnhance(EItemGrade grade, int tier)
    {
        var enhanceConfig = EquipmentDataManager.Instance.EquipmentEnhanceConfig.GetEnhanceData(grade, tier);
        if (PlayerManager.Instance.CurrentCurrencyValue < enhanceConfig.Cost)
            return false;

        var materialDatas = enhanceConfig.MaterialsArray;
        for (int i = 0; i < materialDatas.Length; i++)
        {
            int currentCount = InventoryManager.Instance.GetItemCount(EItemCategory.Material, materialDatas[i].Index);
            int needCount = materialDatas[i].Amount;
            int calculatorCount = currentCount / needCount;
            if (calculatorCount == 0)
            {
                return false;
            }
        }
        
        return true;
    }    

    protected override void OnClickEnhanceButton()
    {
        if (_equipmentItem == null)
            return;

        _enhanceButton.interactable = false;
        var enhanceConfig = EquipmentDataManager.Instance.EquipmentEnhanceConfig.GetEnhanceData(_equipmentItem.Grade, _equipmentItem.Tier);
        
        if (EquipmentManager.Instance.TryEquipmentEnhance(_equipmentItem.InstanceId, enhanceConfig, out _equipmentItem))
        {
            SuccessEnhanceDataUpdate();
        }
        else
        {
            FaildEnhanceDataUpdate();
        }        
    }

    protected override async void SuccessEnhanceDataUpdate()
    {
        RefreshUI();

        var popup = await UIManager.Instance.Show<ToastMessagePopup>();
        popup.PlayToast("¼º°ø");
    }

    protected override async void FaildEnhanceDataUpdate()
    {
        RefreshUI();

        var popup = await UIManager.Instance.Show<ToastMessagePopup>();
        popup.PlayToast("½ÇÆÐ");
    }
}
