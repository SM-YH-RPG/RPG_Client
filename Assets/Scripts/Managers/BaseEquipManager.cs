using UnityEngine;

public abstract class BaseEquipManager<T> : LazySingleton<T> where T : BaseEquipManager<T>, new()
{
    protected const long NO_ITEM_ID = -1;

    protected IInventoryManagerService _inventoryManagerService => PlayerManager.Instance.Inventory;

    public abstract bool EquipItem(int characterIndex, long instanceId, int slotIndex = 0);
    public abstract bool TryUnequipWeapon(long instanceId, int ownerCharacterIndex);

    protected bool IsItemValidAndOfType<TItem>(long instanceId, out TItem item) where TItem : EquipableItem
    {
        BaseInventoryItem baseItem = _inventoryManagerService.GetInventoryItemInstance(instanceId);
        if (baseItem == null)
        {
            Debug.LogWarning($"아이템 ID {instanceId}를 인벤토리에서 찾을 수 없습니다.");
            item = null;
            return false;
        }

        if ((baseItem is TItem specificItem) == false)
        {
            Debug.LogWarning($"아이템 ID {instanceId}는 {typeof(TItem).Name} 타입이 아닙니다. 실제 타입: {baseItem.GetType().Name}");
            item = null;
            return false;
        }

        item = specificItem;
        return true;
    }

    protected void NotifyCharacterStatUpdate(int charIndex)
    {
        var statData = InGameManager.Instance.GetPlayerController(charIndex)?.CharacterData.StatData;
        if (statData != null)
        {
            var character = CharacterManager.Instance.HaveCharacterDict[CharacterManager.Instance.GetCharacterUniqueIndex(charIndex)];
            PlayerManager.Instance.CharacterService.UpdateCharacterStatData(character.InstanceId, statData);

            PlayerManager.Instance.CharacterService.UpdateCurrentCharacterHp(character.CurrentHP, character.InstanceId);
        }
    }

    protected bool CanEnhance(EnhanceData enhanceData)
    {
        if (PlayerManager.Instance.CurrentCurrencyValue < enhanceData.Cost)
            return false;

        var materials = enhanceData.MaterialsArray;
        for (int i = 0; i < materials.Length; i++)
        {
            var requiredCount = materials[i].Amount;
            var ownedCount = InventoryManager.Instance.GetItemCount(materials[i].Category, materials[i].Index);
            if (ownedCount < requiredCount)
                return false;
        }

        return true;
    }
}