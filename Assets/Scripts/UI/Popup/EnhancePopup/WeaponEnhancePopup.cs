using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponEnhancePopup : EnhancePopupBase
{
    private List<EnhanceMaterialItemElement> _itemList = new List<EnhanceMaterialItemElement>();
    private WeaponItem _weaponItem;

    public void UpdateWeaponData(WeaponItem weapon)
    {
        if (weapon == null)
            return;

        _weaponItem = weapon;

        var itemConfig = ItemDataManager.Instance.GetItemConfigData(weapon.Category, weapon.TemplateId);
        _itemPreview.sprite = itemConfig.Sprite;
        _itemName.text = weapon.Name;

        //.. 코드 분할 및 통합
        RefreshUI();
    }

    private void RefreshUI()
    {
        if (_weaponItem == null)
            return;

        int tier = _weaponItem.Tier;
        bool isMaxTier = tier >= MAX_TIER;

        SetTierText(tier);
        UpdateStat(tier);
        CreateMaterialItems(_weaponItem.Grade, tier);
        UpdateCurrencyValue();

        if (isMaxTier)
        {
            _costValue.text = "0";
            _enhanceButton.interactable = false;
        }
        else
        {
            var enhanceConfig = WeaponDataManager.Instance.WeaponEnhanceConfg.GetEnhanceData(_weaponItem.Grade, tier);
            _costValue.text = enhanceConfig.Cost.ToString();
            _enhanceButton.interactable = CheckPossibleEnhance(_weaponItem.Grade, tier);
        }
    }

    protected override void UpdateStat(int tier)
    {
        if (tier >= MAX_TIER)
        {
            _statArray[0].UpdateMaxTierStat(_weaponItem.MainStatType, _weaponItem.MainStatValue);
            _statArray[1].UpdateMaxTierStat(_weaponItem.SubStatType, _weaponItem.SubStatValue);
        }
        else
        {
            var nextStat = WeaponManager.Instance.GetNextTierStats(_weaponItem);

            _statArray[0].UpdateEnhanceStat(_weaponItem.MainStatType, _weaponItem.MainStatValue, nextStat.main);
            _statArray[1].UpdateEnhanceStat(_weaponItem.SubStatType, _weaponItem.SubStatValue, nextStat.sub);
        }
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
            var materialItemList = WeaponDataManager.Instance.WeaponEnhanceConfg.GetEnhanceData(grade, tier).MaterialsArray;
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
        var enhanceConfig = WeaponDataManager.Instance.WeaponEnhanceConfg.GetEnhanceData(grade, tier);
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
        if (_weaponItem == null)
            return;

        _enhanceButton.interactable = false;
        var enhanceConfig = WeaponDataManager.Instance.WeaponEnhanceConfg.GetEnhanceData(_weaponItem.Grade, _weaponItem.Tier);
                
        if (WeaponManager.Instance.TryEnhanceWeaponItem(_weaponItem.InstanceId, enhanceConfig, out _weaponItem))
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
        //.. 중복코드 제거
        RefreshUI();

        var popup = await UIManager.Instance.Show<ToastMessagePopup>();
        popup.PlayToast("성공");
    }

    protected override async void FaildEnhanceDataUpdate()
    {
        //.. 중복코드 제거
        RefreshUI();

        var popup = await UIManager.Instance.Show<ToastMessagePopup>();
        popup.PlayToast("실패");
    }
}
