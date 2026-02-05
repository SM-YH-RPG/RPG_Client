using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public struct LevelConfigData
{
    public int Level;
    public int RequireExp;
}

[CreateAssetMenu(fileName = "CharacterLevelConfig", menuName = "Scriptable Objects/CharacterLevelConfig")]
public class CharacterLevelConfig : ScriptableObject
{
    public GameObject LevelUpEffect;

    [field: SerializeField]
    private LevelConfigData[] _levelConfigDataArray;
    public IReadOnlyList<LevelConfigData> LevelConfigDatas => _levelConfigDataArray;

    private int _maxLevel = 10;
    public int MaxLevel => _maxLevel;

    private Dictionary<int, LevelConfigData> _lookupCache;

    public LevelConfigData GetLevelConfigData(int level)
    {
        if (_lookupCache == null)
        {
            _lookupCache = new Dictionary<int, LevelConfigData>();
            foreach (var data in _levelConfigDataArray)
            {
                if (_lookupCache.ContainsKey(data.Level) == false)
                {
                    _lookupCache.Add(data.Level, data);
                }
            }
        }

        if (_lookupCache.TryGetValue(level, out LevelConfigData result))
            return result;

        return default;
    }
}
