using System;
using System.Collections.Generic;

public interface ICharacterService
{    
    event Action<long, RuntimeCharacter> OnCharacterStatUpdated;

    event Action<int, int> OnUpdateCharacterCurrentHp;

    event Action<RuntimeCharacter> OnUpdateCharacterStatData;

    event Action<int, Dictionary<ECharacterStatType, float>> OnUpdateCharacterStatForInfoPage;

    IReadOnlyDictionary<long, RuntimeCharacter> HaveCharacterDict { get; }

    IReadOnlyDictionary<int, Dictionary<ECharacterStatType, float>> CharacterBaseStatDict { get; }
    IReadOnlyDictionary<int, Dictionary<ECharacterStatType, float>> PlusAddDict { get; }
    IReadOnlyDictionary<int, Dictionary<ECharacterStatType, float>> MultipleAddDict { get; }
    IReadOnlyDictionary<int, Dictionary<ECharacterStatType, float>> FinalStatDict { get; }

    void Initialize(Dictionary<long, RuntimeCharacter> characterDict);    

    Dictionary<long, RuntimeCharacter> GetCharacterSaveData();

    void UpdateCharacterStatData(long instanceId, CharacterStat characterStat);

    float GetCharacterStatValue(int characterIndex, ECharacterStatType type);

    void UpdateCurrentCharacterHp(int newHp, long instanceId);

    long GetCharacterUniqueIndex(int characterIndex);

    RuntimeCharacter GetRunTimeCharacterBy(long uniqueIndex);
}
