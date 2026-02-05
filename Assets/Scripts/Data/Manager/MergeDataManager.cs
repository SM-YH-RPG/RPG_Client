using System;
using UnityEngine;

public class MergeDataManager : LazySingleton<MergeDataManager>
{
    private MergeItemConfig _mergeItemConfig;
    public MergeItemConfig MergeItemConfig => _mergeItemConfig;   

    public void Initialize(MergeItemConfig mergeConfig)
    {
        _mergeItemConfig = mergeConfig;
    }       
}
