using System;
using System.Collections.Generic;

public class RewardItemManager : LazySingleton<RewardItemManager>
{
    private RewardItemConfig _rewardItemConfig = null;

    public event Action OnAddRewardItemEffectStart;

    private Queue<RewardItemTemplate> _RewardItemQueue = new Queue<RewardItemTemplate>();

    private int _rewardItemConfigIndex;
    public int RewardItemConfigIndex => _rewardItemConfigIndex;

    public void InitializeData(RewardItemConfig config)
    {
        _rewardItemConfig = config;
        _RewardItemQueue.Clear();
    }

    public void SetRewardItemConfigIndex(int index)
    {
        _rewardItemConfigIndex = index;
    }

    public RewardItemConfigData GetRewardItemConfigData()
    {
        RewardItemConfigData data = _rewardItemConfig.GetRewardConfigData(_rewardItemConfigIndex);
        return data;
    }

    public void RewardDropItem(int index)
    {
        if (_rewardItemConfig == null)
        {
            return;
        }

        RewardItemConfigData data = _rewardItemConfig.GetRewardConfigData(index);
        EnqueueRewardItem(data);        
    }

    private void EnqueueRewardItem(RewardItemConfigData data)
    {        
        int count = data.RewardItemArray.Length;
        for (int i = 0; i < count; i++)
        {
            if (data.RewardItemArray[i].Amount != 0) // 어떤 아이템을 획득한거라면..
            {
                _RewardItemQueue.Enqueue(data.RewardItemArray[i]);                
            }
        }

        OnAddRewardItemEffectStart?.Invoke();
    }

    public RewardItemTemplate GetRewardItemData()
    {
        if (_RewardItemQueue.Count != 0)
        {
            return _RewardItemQueue.Dequeue();
        }
        return default;
    }

    public int GetRewardItemCount()
    {
        return _RewardItemQueue.Count;
    }
}
