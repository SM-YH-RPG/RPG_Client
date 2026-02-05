public class Party
{
    #region Const
    private const int PARTY_MEMBER_MAX = 3;
    public const int EMPTY_MEMBER_INDEX = -1;
    #endregion

    public int Index;
    //public RuntimeCharacter[] Characters = new RuntimeCharacter[PARTY_MEMBER_MAX];
    public long[] Characters = new long[PARTY_MEMBER_MAX];

    public Party(int index)
    {
        Index = index;
    }

    public int GetMemberCount()
    {
        int memberCount = 0;

        int count = Characters.Length;
        foreach (var character in Characters)
        {
            if (character != EMPTY_MEMBER_INDEX)
                memberCount++;
        }

        return memberCount;
    }

    public Party Clone()
    {
        var clone = new Party(Index);
        for (int i = 0; i < PARTY_MEMBER_MAX; i++)
        {
            if (Characters[i] != EMPTY_MEMBER_INDEX)
            {
                clone.Characters[i] = Characters[i];
            }
            else
            {
                clone.Characters[i] = EMPTY_MEMBER_INDEX;
            }
        }

        return clone;
    }
}