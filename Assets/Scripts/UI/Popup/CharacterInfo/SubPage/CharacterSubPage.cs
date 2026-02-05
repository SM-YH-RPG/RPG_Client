using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSubPage : InfoSubPage
{   
    #region Inspector
    [SerializeField] private Camera _previewCamera;
    
    [SerializeField] private TextMeshProUGUI _characterName;
    
    [SerializeField] private TextMeshProUGUI _characterLevel;
    
    [SerializeField] private StatInfo[] _characterStats;

    [SerializeField] private Image _expFillAmount;

    [SerializeField] private TextMeshProUGUI _expText;
    #endregion
    
    private CharacterStat _characterStatData;
    private IPartyService _partyService = PlayerManager.Instance.PartyService;
    private ICharacterService _characterService = PlayerManager.Instance.CharacterService;

    private Dictionary<ECharacterStatType, float> _characterStatsDict = new Dictionary<ECharacterStatType, float>();

    private void Awake()
    {
        SetupPreview();
        InitCharacterStats();

        _UpdatecharacterInfo += SetCharacterInfo;
        PlayerManager.Instance.CharacterService.OnUpdateCharacterStatForInfoPage += UpdateCharacterStats;
    }

    private void OnDestroy()
    {
        _UpdatecharacterInfo -= SetCharacterInfo;
        PlayerManager.Instance.CharacterService.OnUpdateCharacterStatForInfoPage -= UpdateCharacterStats;
    }

    protected override void SetupPreview()
    {
        _preview = new Preview3D(_previewCamera);        
    }

    private void InitCharacterStats()
    {
        _characterStatsDict.Clear();
        for (int i = 0; i < (int)ECharacterStatType.End; i++)
        {
            if (_characterStatsDict.ContainsKey((ECharacterStatType)i) == false)
            {
                _characterStatsDict.Add((ECharacterStatType)i, PlayerManager.Instance.CharacterService.GetCharacterStatValue(_selectedElementIndex, (ECharacterStatType)i));
            }            
        }
    }

    private void UpdateCharacterStats(int characterIndex, Dictionary<ECharacterStatType, float> updateStatDict)
    {
        if (characterIndex == _selectedElementIndex)
        {
            _characterStatsDict = updateStatDict;
            SetTextFinalStatData(InGameManager.Instance.PlayerControllerDict[_selectedElementIndex].CharacterData.StatData);
        }        
    }

    private void SetCharacterInfo()
    {
        _characterStatData = InGameManager.Instance.PlayerControllerDict[_selectedElementIndex].CharacterData.StatData;
        long characterUniqueId = PlayerManager.Instance.CharacterService.GetCharacterUniqueIndex(_selectedElementIndex);
        RuntimeCharacter selectedCharacter = _characterService.GetRunTimeCharacterBy(characterUniqueId);
        if (selectedCharacter != null)
        {
            _characterLevel.text = $"Lv.{selectedCharacter.Level}";
            UpdateExpData(selectedCharacter);
        }
        _characterName.text = InGameManager.Instance.PlayerControllerDict[_selectedElementIndex].CharacterData.Name;        

        PlayerManager.Instance.CharacterService.UpdateCharacterStatData(characterUniqueId, _characterStatData);
        SetTextFinalStatData(_characterStatData);
    }

    private void SetTextFinalStatData(CharacterStat _statData)
    {
        int statCount = _statData.StatDataArray.Length;
        for (int i = 0; i < statCount; i++)
        {
            if (_statData.StatDataArray[i].Percent)
            {
                float finalValue = _characterStatsDict[(ECharacterStatType)i];
                _characterStats[i].SetCharacterStatData(finalValue, _statData.StatDataArray[i].StatType);
            }
            else
            {
                int finalValue = Mathf.RoundToInt(_characterStatsDict[(ECharacterStatType)i]);
                _characterStats[i].SetCharacterStatData(finalValue, _statData.StatDataArray[i].StatType);
            }
        }
    }

    private void UpdateExpData(RuntimeCharacter character)
    {
        if (character.Level < CharacterLevelManager.Instance.GetMaxLevel())
        {
            var data = CharacterLevelManager.Instance.GetLevelData(character.Level);
            _expFillAmount.fillAmount = (float)character.Exp / data.RequireExp;
            _expText.text = $"EXP {character.Exp}/{data.RequireExp}";
        }
        else
        {
            _expFillAmount.fillAmount = 1f;
            _expText.text = $"LEVEL MAX";
        }
    }
}
