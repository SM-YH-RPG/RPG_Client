using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatInfo : MonoBehaviour
{
    [SerializeField] private Image _statImage;
    [SerializeField] private TextMeshProUGUI _statName;
    [SerializeField] private TextMeshProUGUI _statValue;
    [SerializeField] private Sprite[] _characterStatSpriteArray;
    [SerializeField] private Sprite[] _equipitemStatSpriteArray;

    public void SetCharacterStatData(int value, ECharacterStatType type)
    {        
        SetCharacterStatName(type);
        _statValue.text = value.ToString();
        SetChararcterStatImage(type);
    }

    public void SetCharacterStatData(float value, ECharacterStatType type)
    {        
        SetCharacterStatName(type);
        _statValue.text = $"{Math.Round(value, 1)}%";
        SetChararcterStatImage(type);
    }

    private void SetCharacterStatName(ECharacterStatType type)
    {
        switch (type)
        {
            case ECharacterStatType.HP:
                _statName.text = "체력";
                break;
            case ECharacterStatType.AttackPower:
                _statName.text = "공격력";
                break;
            case ECharacterStatType.Defense:
                _statName.text = "방어력";
                break;
            case ECharacterStatType.Resound:
                _statName.text = "공명 효율";
                break;
            case ECharacterStatType.CriticalRate:
                _statName.text = "크리티컬";
                break;
            case ECharacterStatType.CriticalDamage:
                _statName.text = "크리티컬 피해";
                break;
        }
    }

    public void SetEquipItemStatData(float value, EItemStatType type)
    {        
        SetEquipItemStatName(type);
        switch (type)
        {
            case EItemStatType.HP:
                _statValue.text = $"{Math.Round(value)}";
                break;
            case EItemStatType.HPRate:
                _statValue.text = $"{Math.Round(value, 1)}%";
                break;
            case EItemStatType.AttackPower:
                _statValue.text = $"{Math.Round(value)}";
                break;
            case EItemStatType.AttackPowerRate:
                _statValue.text = $"{Math.Round(value, 1)}%";
                break;
            case EItemStatType.Defense:
                _statValue.text = $"{Math.Round(value)}";
                break;
            case EItemStatType.DefenseRate:                                
            case EItemStatType.ResoundRate:                                
            case EItemStatType.CriticalRate:                
            case EItemStatType.CriticalDamageRate:
                _statValue.text = $"{Math.Round(value, 1)}%";
                break;            
        }        
        SetEquipItemStatImage(type);
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

    private void SetChararcterStatImage(ECharacterStatType type)
    {        
        _statImage.sprite = _characterStatSpriteArray[(int)type];
    }

    private void SetEquipItemStatImage(EItemStatType type)
    {
        _statImage.sprite = _equipitemStatSpriteArray[(int)type];
    }
}
