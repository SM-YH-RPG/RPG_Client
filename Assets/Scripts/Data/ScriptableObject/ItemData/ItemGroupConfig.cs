using UnityEngine;
[CreateAssetMenu(fileName = "ItemGroupConfig", menuName = "Scriptable Objects/ItemGroupConfig")]
public class ItemGroupConfig : ScriptableObject
{
    [field: SerializeField]
    public ItemListConfig[] ItemListConfigArray { get; private set; }


    public int GetMaxStackCount(EItemCategory category)
    {
        for(int i = 0; i < ItemListConfigArray.Length; i++)
        {
            if(ItemListConfigArray[i].Category == category)
            {
                return ItemListConfigArray[i].MaxStackCount;
            }
        }

        return 1;
    }
}
