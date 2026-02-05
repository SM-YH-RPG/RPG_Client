using System.Collections.Generic;

public static class StatGenerationHelper
{
    public static T GetRandomMainStatType<T>(T[] availableStats, int cost)
    {
        if (availableStats == null || availableStats.Length == 0)
        {
            return default(T);
        }
        int randomMin = 0;
        int randomMax = 0;
        if (cost < 3) // 3코스트 미만..체력,방어력,공격력 퍼센트..
        {
            randomMin = 0;
            randomMax = 3;
        }
        else // 3코스트 이상..공격력,공명,크리티컬데미지 퍼센트..
        {
            randomMin = 2;
            randomMax = 5;
        }
        int randomIndex = UnityEngine.Random.Range(randomMin, randomMax);
        return availableStats[randomIndex];
    }


    public static float GenerateRandomStatValue(EItemStatType statType, EItemGrade grade)
    {
        //.. grade data 필요
        return UnityEngine.Random.Range(10f, 15f);
    }

    public static float GenerateRandomSubStat(EItemGrade grade, EItemStatType subStatType)
    {
        //.. grade data 필요
        return UnityEngine.Random.Range(10f, 15f);
    }

    public static Dictionary<EItemStatType, float> GenerateRandomSubStats(EItemGrade grade, EItemStatType[] availableSubStat)
    {
        var dict = new Dictionary<EItemStatType, float>();
        for (int i = 0; i < availableSubStat.Length; i++)
        {
            if (dict.ContainsKey(availableSubStat[i]) == false)
            {
                float value = UnityEngine.Random.Range(1f, 5f);
                dict.Add(availableSubStat[i], value);
            }
        }

        return dict;
    }
}
