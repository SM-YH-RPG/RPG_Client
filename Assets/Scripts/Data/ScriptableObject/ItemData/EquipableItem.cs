public class EquipableItem : BaseInventoryItem
{
    private const int EMPTY_SLOT = -1;

    public int Tier { get; set; }

    private int _equippedCharacterIndex = EMPTY_SLOT;
    public int EquippedCharacterIndex
    {
        get => _equippedCharacterIndex;
        set
        {
            _equippedCharacterIndex = value;
            IsEquipped = value != EMPTY_SLOT;
        }
    }

    public bool IsEquipped { get; private set; }
}