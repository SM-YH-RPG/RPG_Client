using UnityEngine;

public class MergeManager : LazySingleton<MergeManager>
{
    public void AddMergeItem(MergeItemConfigData mergeConfig, ItemConfigData itemConfig, int amount)
    {
        PlayerManager.Instance.Inventory.AddItem(mergeConfig.ItemIndex, mergeConfig.Category, itemConfig.template.Grade, itemConfig.Name, amount);
        ConsumeMaterialsItem(mergeConfig, amount);
    }

    private void ConsumeMaterialsItem(MergeItemConfigData mergeConfig, int amount)
    {
        for (int i = 0; i < mergeConfig.NeedItemArray.Length; i++)
        {
            PlayerManager.Instance.Inventory.RemoveItem(mergeConfig.NeedItemArray[i].Category, mergeConfig.NeedItemArray[i].ItemIndex, mergeConfig.NeedItemArray[i].NeedAmount * amount);
        }
    }
}
