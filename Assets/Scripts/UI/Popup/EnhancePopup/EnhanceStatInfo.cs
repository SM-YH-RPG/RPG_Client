using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnhanceStatInfo : MonoBehaviour
{
    [SerializeField] private Image _statImage;
    [SerializeField] private Image _arrow;
    [SerializeField] private TextMeshProUGUI _statName;
    [SerializeField] private TextMeshProUGUI _currentStatValue;
    [SerializeField] private TextMeshProUGUI _nextStatValue;
    [SerializeField] private Sprite[] _equipItemStatSpriteArray;

    public void UpdateEnhanceStat(EItemStatType type, float currentStatValue, float nextStatValue)
    {
        _statImage.sprite = _equipItemStatSpriteArray[(int)type];
        _arrow.enabled = true;
        SetEquipItemStatName(type);
        _currentStatValue.text = GetEquipItemStatDataString(currentStatValue, type);
        _nextStatValue.text = GetEquipItemStatDataString(nextStatValue, type);
    }

    public void UpdateMaxTierStat(EItemStatType type, float currentStatValue)
    {
        _statImage.sprite = _equipItemStatSpriteArray[(int)type];
        _arrow.enabled = false;
        SetEquipItemStatName(type);
        _currentStatValue.text = GetEquipItemStatDataString(currentStatValue, type);
        _nextStatValue.text = string.Empty;
    }

    public string GetEquipItemStatDataString(float value, EItemStatType type)
    {
        SetEquipItemStatName(type);
        switch (type)
        {
            case EItemStatType.HP:
                return $"{Math.Round(value)}";                
            case EItemStatType.HPRate:
                return $"{Math.Round(value, 1)}%";                
            case EItemStatType.AttackPower:
                return $"{Math.Round(value)}";                
            case EItemStatType.AttackPowerRate:
                return $"{Math.Round(value, 1)}%";                
            case EItemStatType.Defense:
                return $"{Math.Round(value)}";                
            case EItemStatType.DefenseRate:
            case EItemStatType.ResoundRate:
            case EItemStatType.CriticalRate:
            case EItemStatType.CriticalDamageRate:
                return $"{Math.Round(value, 1)}%";
            default:
                return string.Empty;
        }        
    }

    private void SetEquipItemStatName(EItemStatType type)
    {
        switch (type)
        {
            case EItemStatType.HP:
            case EItemStatType.HPRate:
                _statName.text = "체력";
                break;
            case EItemStatType.AttackPower:
            case EItemStatType.AttackPowerRate:
                _statName.text = "공격력";
                break;
            case EItemStatType.Defense:
            case EItemStatType.DefenseRate:
                _statName.text = "방어력";
                break;
            case EItemStatType.ResoundRate:
                _statName.text = "공명 효율";
                break;
            case EItemStatType.CriticalRate:
                _statName.text = "크리티컬";
                break;
            case EItemStatType.CriticalDamageRate:
                _statName.text = "크리티컬 피해";
                break;
        }
    }
}
