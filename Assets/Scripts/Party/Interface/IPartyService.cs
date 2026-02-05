using System;
using System.Collections.Generic;

public interface IPartyService
{
    event Action OnPartyDataAdjusted;
    event Action OnPartyCharacterSwappedSkillData;
    event Action OnPartyCharacterSwappedChangeMonsterCurrentTarget;
    event Action<Party> OnActivePartyChanged;    
    event Action<int, RuntimeCharacter> OnCharacterSelectedInParty;    
    event Action<RuntimeCharacter> OnPartyCharacterSwapped;

    Party GetParty(int index);
    Party CurrentParty { get; }

    int CurrentSelectedPartyPresetIndex { get; }

    int SelectedIndexInParty { get; }

    RuntimeCharacter EmptyPartyMember { get; }

    void Initialize(Dictionary<int, Party> partyInfoDict);

    Dictionary<int, Party> GetSavePartyData();

    int GetPartyPresetArrayIndex(int partyIndex);    

    void UpdateParty(int presetIndex, Party newPartyData);

    void SetSelectedIndexInParty(int index);

    void SetCurrentSelectedPartyPresetIndex(int index);

    void ChangeActiveParty(int newPartyIndex);

    void AssignCharacterToPartySlot(int presetIndex, int indexInParty, RuntimeCharacter newCharacter);
    void RequestCharacterSwap(int indexInParty);
    void ReviveCharacterSwap(int indexInParty);
    bool TryGetSelectedCharacter(out RuntimeCharacter character);

    RuntimeCharacter GetCurrentCharacterInActiveParty();

    int GetCharacterIndexInParty(long instanceId);
}
