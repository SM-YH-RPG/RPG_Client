using System;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerLevel
{
    private int currentLevel = 1;
    private int currentExp = 0;
    private int requireExp = 100;

    public event Action<int> OnUpdateLevel;
    public event Action<int, int> OnUpdateExp;

    public void GetExp(int _exp)
    {
        currentExp += _exp;
        if(IsLevelUp())
        {
            //..TODO :: SHOW POPUP
            currentExp -= requireExp;
            requireExp += 100;

            currentLevel++;
            OnUpdateLevel?.Invoke(currentLevel);
            OnUpdateExp?.Invoke(currentExp, requireExp);
        }
        else
        {
            OnUpdateExp?.Invoke(currentExp, requireExp);
        }
    }

    private bool IsLevelUp()
    {
        return currentExp >= requireExp;
    }

    public int GetLevel()
    {
        return currentLevel;
    }
}
