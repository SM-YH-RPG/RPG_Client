using System;
using System.Collections.Generic;

public class EquipmentManager : BaseEquipManager<EquipmentManager>
{
    #region Const    
    private const int SLOT_COUNT = 5;
    private const int MAX_COST_BUDGET = 12;
    private const int EMPTY_SLOT_ID = -1;
    private const long EMPTY_ITEM_ID = -1;
    #endregion

    private Dictionary<int, long[]> _characterLoadouts = new Dictionary<int, long[]>();
    private int _characterIndex;
    private int _oldCharacterIndex;
    private int _slotIndex;
    private long _instanceId;

    #region Action
    public event Action OnEquipmentChanged;
    public event Action<Exception> OnEquipError;
    public event Action<EquipmentItem> OnEquipmentItemDataChanged;
    #endregion

    #region Public
    public void Initialize()
    {
        var characters = PlayerManager.Instance.CharacterService.HaveCharacterDict;

        foreach (var character in characters.Values)
        {
            var equippedDict = character.EquippedItems;
            foreach (var (slotIndex, item) in equippedDict)
            {
                if (item.Category == EItemCategory.Equipment)
                {
                    SetCharacetrLoadoutToSlotIndex(character.TemplateId, slotIndex - 1, item.InstanceId);
                }
            }
        }
    }

    public void RegisterCharacterLoadout(int characterIndex)
    {
        if (_characterLoadouts.ContainsKey(characterIndex) == false)
        {
            _characterLoadouts.Add(characterIndex, new long[SLOT_COUNT] { -1, -1, -1, -1, -1 });
        }
    }

    public void SetCharacetrLoadoutToSlotIndex(int characterIndex, int slotIndex, long instanceId)
    {
        if (_characterLoadouts.ContainsKey(characterIndex))
        {
            _characterLoadouts[characterIndex][slotIndex] = instanceId;
        }
    }

    public override bool EquipItem(int characterIndex, long instanceId, int slotIndex)
    {
        if (IsItemValidAndOfType(instanceId, out EquipmentItem item))
        {
            if (item.IsEquipped)
            {
                TryUnequipWeapon(instanceId, item.EquippedCharacterIndex);
            }
        }

        _characterIndex = characterIndex;
        _instanceId = instanceId;
        _slotIndex = slotIndex;

        if (IsItemValidAndOfType(instanceId, out EquipmentItem newItem) == false)
        {
            OnEquipError?.Invoke(new InvalidOperationException("선택된 아이템은 유효한 장비 아이템이 아닙니다."));
            return false;
        }

        if (_characterLoadouts.ContainsKey(characterIndex) == false)
        {
            OnEquipError?.Invoke(new InvalidOperationException($"캐릭터 인덱스 {characterIndex}에 대한 로드아웃이 설정되지 않았습니다."));
            return false;
        }
        if (slotIndex < 0 || slotIndex >= SLOT_COUNT)
        {
            OnEquipError?.Invoke(new ArgumentOutOfRangeException(nameof(slotIndex), "잘못된 장비 슬롯 인덱스입니다."));
            return false;
        }

        int potentialTotalCost = CalculateTotalEquipCost(characterIndex, slotIndexToExclude: slotIndex) + newItem.EquipCost;
        if (potentialTotalCost > MAX_COST_BUDGET)
        {
            OnEquipError?.Invoke(new InvalidOperationException($"장착 비용 ({potentialTotalCost})이 최대치 ({MAX_COST_BUDGET})를 초과합니다."));
            return false;
        }

        ApplyEquipItem();
        return true;
    }

    public override bool TryUnequipWeapon(long instanceId, int ownerCharacterIndex)
    {
        _oldCharacterIndex = ownerCharacterIndex;
        _instanceId = instanceId;        

        return TryUnEquipItem();
    }

    public long GetSlotEquippedIntanceID(int characterIndex, int slotIndex)
    {
        if (_characterLoadouts.ContainsKey(characterIndex) == false)
        {
            return EMPTY_ITEM_ID;
        }
        return _characterLoadouts[characterIndex][slotIndex];
    }

    

    public bool TryEquipmentEnhance(long instanceId, EnhanceData enhanceData, out EquipmentItem currentEquipment)
    {
        if (CanEnhance(enhanceData) == false)
        {
            currentEquipment = null;
            return false;
        }

        bool isSuccess = EquipmentItemProcessor.TryEquipmentEnhance(instanceId, enhanceData, out currentEquipment);
        if (isSuccess)
        {
            OnEquipmentItemDataChanged?.Invoke(currentEquipment);
            if (currentEquipment.EquippedCharacterIndex != EMPTY_SLOT_ID)
            {
                long characterInstanceId = PlayerManager.Instance.CharacterService.GetCharacterUniqueIndex(currentEquipment.EquippedCharacterIndex);
                var characterEquippeDict = PlayerManager.Instance.CharacterService.GetRunTimeCharacterBy(characterInstanceId).EquippedItems;
                for (int i = 0; i < characterEquippeDict.Count; i++)
                {
                    if (characterEquippeDict[i].InstanceId == currentEquipment.InstanceId)
                    {
                        characterEquippeDict[i] = currentEquipment;
                        break;
                    }
                }
                NotifyCharacterStatUpdate(currentEquipment.EquippedCharacterIndex);
            }
        }

        return isSuccess;
    }

    public float GetNextTierStat(EItemStatType type, float value, EquipmentItem equip)
    {        
        return EquipmentItemProcessor.CalculateNewStat(type, value, equip);
    }
    #endregion

    #region Private
    private int CalculateTotalEquipCost(int characterIndex, int slotIndexToExclude = -1)
    {
        int currentTotalCost = 0;
        if (_characterLoadouts.TryGetValue(characterIndex, out long[] currentSlots) == false)
        {
            return 0;
        }

        for (int i = 0; i < currentSlots.Length; i++)
        {
            if (i == slotIndexToExclude)
                continue;

            long id = currentSlots[i];
            if (id != EMPTY_ITEM_ID)
            {
                BaseInventoryItem itemInSlot = _inventoryManagerService.GetInventoryItemInstance(id);
                if (itemInSlot is EquipmentItem equipItem)
                {
                    currentTotalCost += equipItem.EquipCost;
                }
            }
        }

        return currentTotalCost;
    }

    private void ApplyEquipItem()
    {
        EquipmentItemProcessor.ExecuteEquipItem(_characterLoadouts, _characterIndex, _slotIndex, _instanceId);

        OnEquipmentChanged?.Invoke();
        NotifyCharacterStatUpdate(_characterIndex);
    }

    private bool TryUnEquipItem()
    {
        if (_characterLoadouts.TryGetValue(_oldCharacterIndex, out long[] ownerSlots))
        {
            for (int i = 0; i < ownerSlots.Length; i++)
            {
                if (ownerSlots[i] == _instanceId)
                {
                    EquipmentItemProcessor.ExecuteUnEquipItem(ownerSlots, i, _oldCharacterIndex, _instanceId);
                    OnEquipmentChanged?.Invoke();
                    NotifyCharacterStatUpdate(_oldCharacterIndex);
                    return true;
                }
            }
        }
        return false;
    }
    #endregion   
}