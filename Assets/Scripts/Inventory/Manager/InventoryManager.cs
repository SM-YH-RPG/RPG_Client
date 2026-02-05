using System;
using System.Collections.Generic;

public interface IEquipmentItem
{
    int Tier { get; set; }
}

public enum EItemCategory
{    
    Weapon,     //.. non-staclable, Equiped
    Equipment,  //.. non-staclable, Equiped
    Consumable,     //.. staclable, consumed
    Material,   //.. stackable, used for crafting
    End
}

public enum EItemGrade  
{
    Common,
    Rare,
    Epic,
    Legend,
    God,
    End
}


public class InventoryManager : LazySingleton<InventoryManager>, IInventoryManagerService
{
    private const int DEFAULT_TIER = 1;

    private Dictionary<EItemCategory, Dictionary<long, BaseInventoryItem>> _inventoryByCategory = new Dictionary<EItemCategory, Dictionary<long, BaseInventoryItem>>();

    private long _nextInstanceId = 1;

    #region Action
    public event Action OnInventoryChanged;
    public event Action<EItemCategory> OnCategorySelected;
    #endregion

    public void Initialize(Dictionary<EItemCategory, Dictionary<long, BaseInventoryItem>> inventoryItems)
    {
        // 인벤토리 아이템 셋팅
        if (inventoryItems == null)
        {
            return;
        }

        _inventoryByCategory = inventoryItems;
    }    

    public Dictionary<EItemCategory, Dictionary<long, BaseInventoryItem>> GetSaveInventoryData()
    {
        return _inventoryByCategory;
    }

    private Dictionary<long, BaseInventoryItem> GetCategoryDict(EItemCategory category, bool createIfMissing = false)
    {
        if (_inventoryByCategory.TryGetValue(category, out Dictionary<long, BaseInventoryItem> dict) == false)
        {
            if (createIfMissing)
            {
                dict = new Dictionary<long, BaseInventoryItem>();
                _inventoryByCategory.Add(category, dict);
            }
            else
            {
                return new Dictionary<long, BaseInventoryItem>();
            }
        }

        return dict;
    }

    #region Add
    /// <summary>
    /// InventoryItem Create And(or) Add
    /// </summary>
    /// <param name="templateId">TemplateId</param>
    /// <param name="category">Category</param>
    /// <param name="grade">Grade</param>
    /// <param name="name">ItemName</param>
    /// <param name="amountToAdd">Add Amount</param>
    /// <param name="tier">ItemTier</param>
    /// <param name="instanceId">ItemInstanceId</param>
    /// <param name="originItem">Weapon,Equipment Origin Item</param>
    public void AddItem(int templateId, EItemCategory category, EItemGrade grade, string name, int amountToAdd = 1, int tier = 1, long instanceId = 0)
    {
        if (amountToAdd <= 0)
            return;

        InventoryProcessor.AddItem(
               _inventoryByCategory,
               templateId,
               category,
               grade,
               name,
               amountToAdd,
               tier,
               ref _nextInstanceId);

        OnInventoryChanged?.Invoke();
    }
    #endregion

    #region Remove (합성, 강화, 폐기 등에 사용)
    public bool RemoveItem(EItemCategory category, int templateId, int amountToRemove = 1)
    {
        if (amountToRemove <= 0)
        {
            return true;
        }

        InventoryProcessor.RemoveItem(
            _inventoryByCategory,
            category, 
            templateId,
            amountToRemove);

        OnInventoryChanged?.Invoke();

        return true;
    }
    #endregion

    #region Get
    public int GetItemCount(EItemCategory category, int templateId)
    {
        Dictionary<long, BaseInventoryItem> categoryDict = GetCategoryDict(category);
        int totalCount = 0;
        foreach (var item in categoryDict.Values)
        {
            if (item.TemplateId == templateId)
            {
                totalCount += item.Amount;
            }
        }
        return totalCount;
    }

    private bool TryGetInventoryItemInstance(long instanceId, out BaseInventoryItem item)
    {
        item = null;
        foreach (var categoryKvp in _inventoryByCategory)
        {
            Dictionary<long, BaseInventoryItem> categoryDict = categoryKvp.Value;

            if (categoryDict.TryGetValue(instanceId, out BaseInventoryItem foundItem))
            {
                item = foundItem;
                return true;
            }
        }

        return false;
    }

    public BaseInventoryItem GetInventoryItemInstance(long instanceId)
    {
        if (TryGetInventoryItemInstance(instanceId, out BaseInventoryItem item))
        {
            return item;
        }

        return null;
    }

    public EItemCategory GetItemCategoryInventoryItemInstance(long instanceId)
    {
        foreach (var categoryKvp in _inventoryByCategory)
        {
            Dictionary<long, BaseInventoryItem> categoryDict = categoryKvp.Value;

            if (categoryDict.ContainsKey(instanceId))
            {
                return categoryKvp.Key;
            }
        }
        return EItemCategory.End;
    }

    public IReadOnlyList<BaseInventoryItem> GetInvenItemDataList(EItemCategory type)
    {
        List<BaseInventoryItem> filteredList = new List<BaseInventoryItem>();
        Dictionary<long, BaseInventoryItem> itemsInCategory = GetCategoryDict(type);

        foreach (var item in itemsInCategory.Values)
        {
            filteredList.Add(item);
        }

        return filteredList.AsReadOnly();
    }
    #endregion

    //.. FIXME?? :: 고민좀 해보자
    public void SelectCategory(EItemCategory category)
    {
        OnCategorySelected?.Invoke(category);
    }

    public long GetCurrentInventoryInstanceID()
    {
        return _nextInstanceId;
    }

    public void SetInventoryInstanceID(long instanceId)
    {
        _nextInstanceId = instanceId;
    }
}
