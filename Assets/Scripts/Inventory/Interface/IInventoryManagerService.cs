
using System;
using System.Collections.Generic;

public interface IInventoryManagerService
{
    event Action OnInventoryChanged;
    event Action<EItemCategory> OnCategorySelected;
    
    void Initialize(Dictionary<EItemCategory, Dictionary<long, BaseInventoryItem>> inventoryItems);

    Dictionary<EItemCategory, Dictionary<long, BaseInventoryItem>> GetSaveInventoryData();
    
    void AddItem(int templateIndex, EItemCategory category, EItemGrade grade, string name, int amountToAdd = 1, int tier = 1, long instanceId = 0);    
    bool RemoveItem(EItemCategory category, int itemIndex, int amountToRemove = 1);
    BaseInventoryItem GetInventoryItemInstance(long instanceId);
    EItemCategory GetItemCategoryInventoryItemInstance(long instanceId);
    IReadOnlyList<BaseInventoryItem> GetInvenItemDataList(EItemCategory type);
    void SelectCategory(EItemCategory category);
    long GetCurrentInventoryInstanceID();
    void SetInventoryInstanceID(long instanceId);
}