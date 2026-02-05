using System.Collections.Generic;
using UnityEngine;

public class ItemDataManager : LazySingleton<ItemDataManager>
{
    #region Const
    private const int SHIFT_VALUE = 32;
    #endregion

    private string[] _gradeHexCode;

    private Color[] _gradeColor;

    private ItemGroupConfig _itemGroupConfig;

    private ConsumeableItemConfigData _consumableItemConfig;
    public ConsumeableItemConfigData ConsumeableItemConfig => _consumableItemConfig;

    private Dictionary<EItemCategory, ItemListConfig> _categoryConfigCache = new Dictionary<EItemCategory, ItemListConfig>();

    private Dictionary<long, ItemConfigData> _itemConfigDataCache = new Dictionary<long, ItemConfigData>();

    public void InitializeData(ItemGroupConfig config)
    {
        _itemGroupConfig = config;
        if (_itemGroupConfig == null)
        {
            return;
        }

        foreach (var listConfig in _itemGroupConfig.ItemListConfigArray)
        {
            if (_categoryConfigCache.ContainsKey(listConfig.Category) == false)
            {
                _categoryConfigCache.Add(listConfig.Category, listConfig);
            }

            foreach (var itemData in listConfig.ItemDatas)
            {
                long uniqueKey = GenerateUniqueKey(itemData.template.Category, itemData.template.Index);
                if (_itemConfigDataCache.ContainsKey(uniqueKey) == false)
                {
                    _itemConfigDataCache.Add(uniqueKey, itemData);
                }
            }
        }
        GradeColorCache();
    }

    public void SetConsumeableItemConfig(ConsumeableItemConfigData config)
    {
        _consumableItemConfig = config;
    }

    private void GradeColorCache()
    {
        _gradeHexCode = new string[]
        {
            "#C2C2C2",
            "#93DCA3",
            "#AEF6FF",
            "#FFBFFC",
            "#CFC78B"
        };

        _gradeColor = new Color[_gradeHexCode.Length];

        for (int i = 0; i < _gradeHexCode.Length; i++)
        {
            if (!ColorUtility.TryParseHtmlString(_gradeHexCode[i], out _gradeColor[i]))
                _gradeColor[i] = Color.white;
        }
    }

    public Color GetGradeColor(int grade)
    {
        if (grade < 0 || grade >= _gradeColor.Length)
            return Color.white;

        return _gradeColor[grade];
    }

    private long GenerateUniqueKey(EItemCategory category, int index)
    {
        return ((long)category << SHIFT_VALUE) | (uint)index;
    }

    public ItemConfigData GetItemConfigData(EItemCategory category, int itemIndex)
    {
        long uniqueKey = GenerateUniqueKey(category, itemIndex);
        if (_itemConfigDataCache.TryGetValue(uniqueKey, out ItemConfigData data))
        {
            return data;
        }

        return default;
    }

    public int GetMaxStackSize(EItemCategory category)
    {
        if (_categoryConfigCache.TryGetValue(category, out ItemListConfig config))
        {
            return config.MaxStackCount;
        }

        return 1;
    }

    public int GetCategoryMaxSlotCount(EItemCategory category)
    {
        if (_categoryConfigCache.TryGetValue(category, out ItemListConfig config))
        {
            return config.CategoryMaxSlotCount;
        }

        return 0;
    }
}
