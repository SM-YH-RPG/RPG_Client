using System;
using System.Collections.Generic;
using UnityEngine;


public class PartyManager : LazySingleton<PartyManager>, IPartyService
{
    #region Const
    private const int EMPTY_CHARACTER_INDEX = -1;
    private const int PARTY_PRESET_MAX = 3;
    #endregion

    #region Action
    public event Action OnPartyDataAdjusted;
    public event Action OnPartyCharacterSwappedSkillData;
    public event Action OnPartyCharacterSwappedChangeMonsterCurrentTarget;
    public event Action<Party> OnActivePartyChanged;
    public event Action<int, RuntimeCharacter> OnCharacterSelectedInParty;    
    public event Action<RuntimeCharacter> OnPartyCharacterSwapped;    
    #endregion

    private Party[] _partyPresets;

    private int _currentSelectedPartyPresetIndex;
    public int CurrentSelectedPartyPresetIndex => _currentSelectedPartyPresetIndex;

    private int _selectedIndexInParty;
    public int SelectedIndexInParty => _selectedIndexInParty;

    private bool _isActivePartyChange;
    public bool isActivePartyChange => _isActivePartyChange;

    private Party _currentParty;
    
    public Party CurrentParty
    {
        get { return _currentParty; }
        private set { _currentParty = value; }
    }

    private ICharacterService _characterService = PlayerManager.Instance.CharacterService;

    private RuntimeCharacter _emptyPartyMember;
    public RuntimeCharacter EmptyPartyMember => _emptyPartyMember;

    public PartyManager()
    {
        _partyPresets = new Party[PARTY_PRESET_MAX];                
        _selectedIndexInParty = 0;
        _emptyPartyMember = new RuntimeCharacter(); // Party 멤버 교체 빈슬릇 용도..
        _emptyPartyMember.TemplateId = Party.EMPTY_MEMBER_INDEX;
    }

    public void Initialize(Dictionary<int, Party> partyInfoDict)
    {
        if (partyInfoDict == null)
        {
            for (int i = 0; i < PARTY_PRESET_MAX; i++)
            {
                _partyPresets[i] = new Party(i);
                for (int j = 0; j < PARTY_PRESET_MAX; j++)
                {
                    _partyPresets[i].Characters[j] = Party.EMPTY_MEMBER_INDEX;
                }
            }
            CurrentParty = _partyPresets[0];
        }
        else
        {
            int index = 0;
            foreach (var partyInfo in partyInfoDict.Values)
            {
                var party = partyInfo;
                _partyPresets[index] = party;
                index++;
            }
            _currentParty = _partyPresets[CurrentSelectedPartyPresetIndex];
        }
    }

    public Dictionary<int, Party> GetSavePartyData()
    {
        Dictionary<int, Party> parties = new Dictionary<int, Party>();
        foreach (var item in _partyPresets)
        {            
            parties.Add(item.Index, item);
        }
        return parties;
    }

    public Party GetParty(int index)
    {
        foreach (var party in _partyPresets)
        {
            if (party.Index == index)
                return party;
        }
        return null;
    }

    public int GetPartyPresetArrayIndex(int partyIndex)
    {
        for (int i = 0; i < _partyPresets.Length; i++)
        {
            if (_partyPresets[i].Index == partyIndex)
                return i;
        }
        return 0;
    }

    public void UpdateParty(int presetIndex, Party newPartyData)
    {
        if (GetParty(presetIndex) != null)
        {
            _partyPresets[GetPartyPresetArrayIndex(presetIndex)] = newPartyData;
            if (CurrentParty.Index == presetIndex)
            {
                CurrentParty = newPartyData;
            }

            OnPartyDataAdjusted?.Invoke();
        }
    }

    public void SetSelectedIndexInParty(int index)
    {
        if (index >= 0)
        {
            _selectedIndexInParty = index;
            if (_currentParty != null)
            {
                OnCharacterSelectedInParty?.Invoke(_selectedIndexInParty, _characterService.GetRunTimeCharacterBy(CurrentParty.Characters[_selectedIndexInParty]));
            }
        }
    }

    public void SetCurrentSelectedPartyPresetIndex(int index)
    {
        _currentSelectedPartyPresetIndex = index;
    }

    public void ChangeActiveParty(int newPartyIndex)
    {
        if (newPartyIndex >= 0)
        {
            ApplyActivePartyChange(GetParty(newPartyIndex));
        }
    }

    private void ApplyActivePartyChange(Party party)
    {
        if (party == null || party.GetMemberCount() <= 0)
            return;

        _isActivePartyChange = _currentParty.Index != party.Index;
        _currentParty = party;

        ValidateSelectedIndexInParty();
        SetCurrentSelectedPartyPresetIndex(_currentParty.Index);

        var character = _characterService.GetRunTimeCharacterBy(_currentParty.Characters[SelectedIndexInParty]);

        OnActivePartyChanged?.Invoke(_currentParty);
        OnPartyCharacterSwapped?.Invoke(character);
        OnPartyCharacterSwappedChangeMonsterCurrentTarget?.Invoke();
        CharacterLevelManager.Instance.ChangedPlayCharacter(character);
        OnPartyCharacterSwappedSkillData?.Invoke();

        PlayerManager.Instance.CharacterService.UpdateCurrentCharacterHp(character.CurrentHP, CurrentParty.Characters[SelectedIndexInParty]);

        if (_isActivePartyChange)
        {
            // 다른 프리셋으로 교체 했을때만 저장 요청
            SaveManager.Instance.RequestSave(ESaveCategory.ActivePartyChanged);
        }
    }

    private void ValidateSelectedIndexInParty()
    {
        int firstValidIndex = Array.FindIndex(_currentParty.Characters, idx => idx != EMPTY_CHARACTER_INDEX);

        if (_currentParty.Characters[_selectedIndexInParty] != EMPTY_CHARACTER_INDEX)
        {
            // 마지막 저장데이터 플레이 캐릭터로 시작
            var lastSavedCharacter = _characterService.GetRunTimeCharacterBy(_currentParty.Characters[_selectedIndexInParty]);
            if (lastSavedCharacter.CurrentHP > 0)
            {                
                return;
            }
        }

        // 액티브 파티 교체 했을때 같은 슬릇의 캐릭터가 죽어있는 상태면 다음 살아있는 캐릭터로 인덱스 교체
        int firstAliveIndex = GetFirstAliveIndex(_currentParty.Characters);
        if (firstAliveIndex >= 0)
        {
            SetSelectedIndexInParty(firstAliveIndex);
            return;
        }

        // 모두 해당 안되면 빈칸 아닌 슬릇으로 바로 교체
        SetSelectedIndexInParty(firstValidIndex);
    }

    private int GetFirstAliveIndex(long[] characters)
    {
        for (int i = 0; i < characters.Length; i++)
        {
            if (characters[i] == EMPTY_CHARACTER_INDEX)
                continue;

            var character = _characterService.GetRunTimeCharacterBy(characters[i]);
            if (character.CurrentHP > 0)
                return i;
        }
        return -1;
    }

    private void AssignCharacterAndHandleOverlap(Party targetParty, int indexInParty, RuntimeCharacter newCharacter)
    {
        var prevCharacterInSlot = targetParty.Characters[indexInParty];
        if (_characterService.GetRunTimeCharacterBy(prevCharacterInSlot).TemplateId == newCharacter.TemplateId)
        {
            targetParty.Characters[indexInParty] = Party.EMPTY_MEMBER_INDEX;
            OnPartyDataAdjusted?.Invoke();
            return;
        }

        int count = targetParty.Characters.Length;
        for (int i = 0; i < count; i++)
        {
            if (i != indexInParty && _characterService.GetRunTimeCharacterBy(targetParty.Characters[i]).TemplateId == newCharacter.TemplateId)
            {
                targetParty.Characters[i] = prevCharacterInSlot;
                break;
            }
        }

        targetParty.Characters[indexInParty] = newCharacter.InstanceId;

        OnPartyDataAdjusted?.Invoke();
    }

    public void AssignCharacterToPartySlot(int targetPresetIndex, int indexInParty, RuntimeCharacter selectedCharacter)
    {
        if (targetPresetIndex == CurrentParty.Index)
        {
            Party activePreset = GetParty(targetPresetIndex);
            ChangeCharacterInActiveParty(activePreset, indexInParty, selectedCharacter);
        }
        else
        {
            ChangeInactivePartyPreset(targetPresetIndex, indexInParty, selectedCharacter);
        }        
    }

    private void ChangeCharacterInActiveParty(Party party, int indexInParty, RuntimeCharacter newCharacter)
    {
        AssignCharacterAndHandleOverlap(party, indexInParty, newCharacter);
    }

    private void ChangeInactivePartyPreset(int presetIndex, int indexInParty, RuntimeCharacter newCharacter)
    {
        Party preset = GetParty(presetIndex);
        if (preset == null || CurrentParty.Index == preset.Index)
            return;

        AssignCharacterAndHandleOverlap(preset, indexInParty, newCharacter);
    }

    public void RequestCharacterSwap(int indexInParty)
    {
        var character = _characterService.GetRunTimeCharacterBy(CurrentParty.Characters[indexInParty]);
        if (character.TemplateId != Party.EMPTY_MEMBER_INDEX) // 빈 슬릇이 아닐때
        {
            if (character.InstanceId != GetCurrentCharacterInActiveParty().InstanceId) // 같은 캐릭터가 아닐때
            {
                if (character.CurrentHP > 0)
                {                    
                    SwapCharacter(indexInParty);
                }
            }
        }
    }

    public void ReviveCharacterSwap(int indexInParty)
    {
        var character = _characterService.GetRunTimeCharacterBy(CurrentParty.Characters[indexInParty]);
        if (character.TemplateId != Party.EMPTY_MEMBER_INDEX) // 빈 슬릇이 아닐때
        {
            if (character.CurrentHP > 0)
            {
                SwapCharacter(indexInParty);
            }
        }
    }

    private void SwapCharacter(int indexInParty)
    {
        SetSelectedIndexInParty(indexInParty);

        var character = _characterService.GetRunTimeCharacterBy(CurrentParty.Characters[SelectedIndexInParty]);

        OnPartyCharacterSwappedSkillData?.Invoke();
        OnPartyCharacterSwapped?.Invoke(character);
        OnPartyCharacterSwappedChangeMonsterCurrentTarget?.Invoke();
        CharacterLevelManager.Instance.ChangedPlayCharacter(character);
        PlayerManager.Instance.CharacterService.UpdateCurrentCharacterHp(character.CurrentHP, CurrentParty.Characters[SelectedIndexInParty]);
    }

    public bool TryGetSelectedCharacter(out RuntimeCharacter character)
    {
        bool isValid = CurrentParty != null &&
                       SelectedIndexInParty >= 0 &&
                       SelectedIndexInParty < CurrentParty.Characters.Length;

        if (isValid == false)
        {
            character = null;
            return false;
        }

        character = GetCurrentCharacterInActiveParty();
        return isValid;
    }

    public RuntimeCharacter GetCurrentCharacterInActiveParty()
    {
        return _characterService.GetRunTimeCharacterBy(_currentParty.Characters[SelectedIndexInParty]);
    }

    public int GetCharacterIndexInParty(long instanceId)
    {
        int count = _currentParty.Characters.Length;
        for (int i = 0; i < count; i++)
        {
            if (_currentParty.Characters[i] == instanceId)
            {
                return i;
            }
        }
        return -1;
    }
}