using System;
using System.Collections.Generic;
using UnityEngine;

public enum ECharacterStatType
{
    HP,
    AttackPower,
    Defense,
    Resound,
    CriticalRate,
    CriticalDamage,
    End
}

public class CharacterManager : LazySingleton<CharacterManager>, ICharacterService
{    
    private const int WEAPON_SLOT_INDEX = 0;
    private const int DEFAULT_CHARACTER_TEMPLATE_INDEX = 1001;
    
    public event Action<long, RuntimeCharacter> OnCharacterStatUpdated;
    public event Action<int, int> OnUpdateCharacterCurrentHp;
    public event Action<RuntimeCharacter> OnUpdateCharacterStatData;
    public event Action<int, Dictionary<ECharacterStatType, float>> OnUpdateCharacterStatForInfoPage;

    private Dictionary<long, RuntimeCharacter> _haveCharacterDict;
    public IReadOnlyDictionary<long, RuntimeCharacter> HaveCharacterDict => _haveCharacterDict;

    private Dictionary<int, Dictionary<ECharacterStatType, float>> _characterBaseStatDict;
    public IReadOnlyDictionary<int, Dictionary<ECharacterStatType, float>> CharacterBaseStatDict => _characterBaseStatDict;

    private Dictionary<int, Dictionary<ECharacterStatType, float>> _plusAddDict;
    public IReadOnlyDictionary<int, Dictionary<ECharacterStatType, float>> PlusAddDict => _plusAddDict;

    private Dictionary<int, Dictionary<ECharacterStatType, float>> _multipleAddDict;
    public IReadOnlyDictionary<int, Dictionary<ECharacterStatType, float>> MultipleAddDict => _multipleAddDict;

    private Dictionary<int, Dictionary<ECharacterStatType, float>> _finalStatDict;
    public IReadOnlyDictionary<int, Dictionary<ECharacterStatType, float>> FinalStatDict => _finalStatDict;

    private IPartyService _partyService;

    public void Initialize(Dictionary<long, RuntimeCharacter> characterDict)
    {
        _partyService = PlayerManager.Instance.PartyService;
        _characterBaseStatDict = new Dictionary<int, Dictionary<ECharacterStatType, float>>();
        _plusAddDict = new Dictionary<int, Dictionary<ECharacterStatType, float>>();
        _multipleAddDict = new Dictionary<int, Dictionary<ECharacterStatType, float>>();
        _finalStatDict = new Dictionary<int, Dictionary<ECharacterStatType, float>>();

        if (characterDict == null)
        {
            // ..최초 캐릭터 리스트 Add
            _haveCharacterDict = new Dictionary<long, RuntimeCharacter>();
            int partyCount = _partyService.CurrentParty.Characters.Length;
            int characterCount = InGameManager.Instance.AllCharaceterCount;
            for (int i = 0; i < characterCount; i++)
            {
                int templateId = i + DEFAULT_CHARACTER_TEMPLATE_INDEX;
                var characterStat = InGameManager.Instance.GetPlayerController(templateId).CharacterData.StatData;
                RuntimeCharacter newChar = new RuntimeCharacter
                {
                    InstanceId = i,
                    TemplateId = templateId,
                    Level = 1,
                    CurrentHP = Mathf.RoundToInt(characterStat.StatDataArray[(int)ECharacterStatType.HP].Value),
                    MaxHp = Mathf.RoundToInt(characterStat.StatDataArray[(int)ECharacterStatType.HP].Value),
                    Damage = Mathf.RoundToInt(characterStat.StatDataArray[(int)ECharacterStatType.AttackPower].Value),
                    Defence = Mathf.RoundToInt(characterStat.StatDataArray[(int)ECharacterStatType.Defense].Value),
                    ChargeEfficiency = Mathf.RoundToInt(characterStat.StatDataArray[(int)ECharacterStatType.Resound].Value),
                    CriticalPercent = Mathf.RoundToInt(characterStat.StatDataArray[(int)ECharacterStatType.CriticalRate].Value),
                    CriticalDamageRate = Mathf.RoundToInt(characterStat.StatDataArray[(int)ECharacterStatType.CriticalDamage].Value),
                    Exp = 0,
                    EquippedItems = new Dictionary<int, BaseInventoryItem>(),
                };

                // 새로운 계정 기본 장착 무기 셋팅..
                var firstWeaponItem = PlayerManager.Instance.Inventory.GetInventoryItemInstance(i + 1);
                if (firstWeaponItem is WeaponItem weapon)
                {
                    weapon.EquippedCharacterIndex = newChar.TemplateId;
                    newChar.EquippedItems.Add(WEAPON_SLOT_INDEX, weapon);
                    WeaponManager.Instance.InitCharacterEquippedWeaponItemData(newChar.TemplateId, weapon.InstanceId);
                }

                if (i < partyCount)
                {
                    _partyService.GetParty(_partyService.CurrentSelectedPartyPresetIndex).Characters[i] = newChar.InstanceId;
                    _partyService.CurrentParty.Characters[i] = newChar.InstanceId;
                }
                EquipmentManager.Instance.RegisterCharacterLoadout(newChar.TemplateId);                
                _haveCharacterDict.Add(i, newChar);
            }
        }
        else
        {
            // ..보유 캐릭터 리스트 Init
            _haveCharacterDict = characterDict;
            foreach (var character in characterDict.Values)
            {
                EquipmentManager.Instance.RegisterCharacterLoadout(character.TemplateId);                
            }            
        }
        InitializeStatDictionary();
    }


    private void InitializeStatDictionary()
    {
        foreach (var character in _haveCharacterDict.Values)
        {
            CreateStatDictionary(_plusAddDict, character.TemplateId);
            CreateStatDictionary(_multipleAddDict, character.TemplateId);
            CreateStatDictionary(_finalStatDict, character.TemplateId);
        }
    }

    public Dictionary<long, RuntimeCharacter> GetCharacterSaveData()
    {
        return _haveCharacterDict;
    }

    public void UpdateCurrentCharacterHp(int newHp, long instanceId)
    {
        Party party = _partyService.CurrentParty;

        int indexInParty = _partyService.GetCharacterIndexInParty(instanceId);
        RuntimeCharacter character = _haveCharacterDict[instanceId];
        character.CurrentHP = Math.Min(character.MaxHp, newHp);        

        if (party != null && indexInParty >= 0 && indexInParty < party.Characters.Length)
        {            
            OnCharacterStatUpdated?.Invoke(indexInParty, character);
            OnUpdateCharacterCurrentHp?.Invoke(character.TemplateId, character.CurrentHP);
        }        
    }

    public void UpdateCharacterStatData(long instanceId, CharacterStat characterStat)
    {
        var runTimeChar = _haveCharacterDict[instanceId];
        // 캐릭터 baseStat setting (LevelUp시 새로 갱신)
        if (_characterBaseStatDict.TryGetValue(runTimeChar.TemplateId, out Dictionary<ECharacterStatType, float> baseDict) == false)
        {
            baseDict = new Dictionary<ECharacterStatType, float>(characterStat.StatDataArray.Length);
            foreach (var stat in characterStat.StatDataArray)
            {
                float levelfactor = CharacterStatCalculator.CalculateBaseStatFactorByLevel(runTimeChar.Level, stat.StatType);
                baseDict[stat.StatType] = stat.Value * levelfactor;
            }
            _characterBaseStatDict.Add(runTimeChar.TemplateId, baseDict);
        }
        else
        {
            foreach (var stat in characterStat.StatDataArray)
            {
                float levelfactor = CharacterStatCalculator.CalculateBaseStatFactorByLevel(runTimeChar.Level, stat.StatType);
                baseDict[stat.StatType] = stat.Value * levelfactor;
            }
        }

        foreach (var item in characterStat.StatDataArray)
        {
            // plus가중치, rate가중치 다시 연산을 위해 0f로 값 초기화..
            _plusAddDict[runTimeChar.TemplateId][item.StatType] = 0f;
            _multipleAddDict[runTimeChar.TemplateId][item.StatType] = 0f;
        }
        CharacterStatCalculator.UpdateCharacterStatData(runTimeChar.TemplateId);
        SetCharacterStatData(runTimeChar, _finalStatDict[runTimeChar.TemplateId]);
        OnUpdateCharacterStatData?.Invoke(runTimeChar);
        OnUpdateCharacterStatForInfoPage?.Invoke(runTimeChar.TemplateId, _finalStatDict[runTimeChar.TemplateId]);
    }

    private void CreateStatDictionary(Dictionary<int, Dictionary<ECharacterStatType, float>> dictionary, int characterIndex)
    {
        if (dictionary.ContainsKey(characterIndex) == false)
        {
            Dictionary<ECharacterStatType, float> dict = new Dictionary<ECharacterStatType, float>();
            dictionary.Add(characterIndex, dict);
        }
    }

    private void SetCharacterStatData(RuntimeCharacter character, Dictionary<ECharacterStatType, float> finalStatDict)
    {
        foreach (var (statType, value) in finalStatDict)
        {
            switch (statType)
            {
                case ECharacterStatType.HP:
                    character.MaxHp = Mathf.RoundToInt(value);
                    break;
                case ECharacterStatType.AttackPower:
                    character.Damage = Mathf.RoundToInt(value);
                    break;
                case ECharacterStatType.Defense:
                    character.Defence = Mathf.RoundToInt(value);
                    break;
                case ECharacterStatType.Resound:
                    character.ChargeEfficiency = value;
                    break;
                case ECharacterStatType.CriticalRate:
                    character.CriticalPercent = value;
                    break;
                case ECharacterStatType.CriticalDamage:
                    character.CriticalDamageRate = value;
                    break;                
            }
        }
    }

    public float GetCharacterStatValue(int characterIndex, ECharacterStatType type)
    {
        if (_finalStatDict.ContainsKey(characterIndex))
        {
            return _finalStatDict[characterIndex][type];
        }
        return 0f;
    }

    public long GetCharacterUniqueIndex(int characterIndex)
    {
        if (_haveCharacterDict != null)
        {
            foreach (var item in _haveCharacterDict)
            {
                if (item.Value.TemplateId == characterIndex)
                {
                    return item.Key;
                }
            }
        }
        return -1;
    }

    public RuntimeCharacter GetRunTimeCharacterBy(long uniqueIndex)
    {
        if (_haveCharacterDict.ContainsKey(uniqueIndex))
            return _haveCharacterDict[uniqueIndex];
        else
        {
            return PlayerManager.Instance.PartyService.EmptyPartyMember;
        }
    }
}
