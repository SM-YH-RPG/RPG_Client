

using Cysharp.Threading.Tasks;

public class StoreTable : LazySingleton<StoreTable>
{
    private StoreItemData[] storeItemDatas;

    public async UniTask LoadTable()
    {
        storeItemDatas = await DataLoader.LoadJson<StoreItemData[]>("StoreItemTable");
    }

    public StoreItemData GetStoreItemData(int index)
    {
        return storeItemDatas[index];
    }

    public StoreItemData[] GetStoreItemDatas()
    {
        return storeItemDatas;
    }
}