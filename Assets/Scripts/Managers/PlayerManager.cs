using System;

public class PlayerManager : LazySingleton<PlayerManager>
{
    #region Action    
    public event Action OnCurrencyValueChanged;
    public event Action<int, int> OnPlayerStaminaUpdated;
    #endregion

    public IInteractionService InteractionService => InteractionEventManager.Instance;

    public IPartyService PartyService => PartyManager.Instance;

    public IInventoryManagerService Inventory => InventoryManager.Instance;

    public ICharacterService CharacterService => CharacterManager.Instance;

    private int _currentCurrencyValue;
    public int CurrentCurrencyValue => _currentCurrencyValue;

    private int _maxStamina = 300;
    public int MaxStamina => _maxStamina;

    private int _currentStamina = 300;
    public int CurrentStamina => _currentStamina;


    public void Initialize()
    {
        _currentCurrencyValue = 10000;
    }

    public void UpdateCurrentCurrencyValue(int newCostValue)
    {
        _currentCurrencyValue = newCostValue;
        OnCurrencyValueChanged?.Invoke();
        SaveManager.Instance.RequestSave(ESaveCategory.CurrencyChanged);
    }

    public void UpdatePlayerStamina(int newStamina)
    {
        _currentStamina = newStamina;
        OnPlayerStaminaUpdated?.Invoke(newStamina, MaxStamina);
    }
}