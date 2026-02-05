using Cysharp.Threading.Tasks;

public class CharacterTable : LazySingleton<CharacterTable>
{
    private CharacterData[] _characterDataArray;

    public async UniTask LoadTable()
    {
        _characterDataArray = await DataLoader.LoadJson<CharacterData[]>("CharacterTable");
    }

    public CharacterData[] GetCharacterDatas()
    {
        return _characterDataArray;
    }

    public CharacterData GetCharacterData(int index)
    {
        return _characterDataArray[index];
    }
}