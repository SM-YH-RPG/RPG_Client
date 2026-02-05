using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class SkillTable : LazySingleton<SkillTable>
{
    private Dictionary<int, SkillData> _dictSkillData = new Dictionary<int, SkillData>();
    public async UniTask LoadTable()
    {

        List<SkillData> skillDatas = await DataLoader.LoadJson<List<SkillData>>("SkillTable");
        foreach (var data in skillDatas)
        {
            if (!_dictSkillData.ContainsKey(data.index))
            {
                _dictSkillData.Add(data.index, data);
            }
        }
    }

    public Dictionary<int, SkillData> GetSkillDatas()
    {
        return _dictSkillData;
    }

    public SkillData GetSkillData(int index)
    {
        if (_dictSkillData.TryGetValue(index, out var skillData))
        {
            return skillData;
        }
        else
        {
            return default;
        }
    }
}
