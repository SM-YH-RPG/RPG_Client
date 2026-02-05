using UnityEngine;

public enum ESaveCategory
{
    CurrencyChanged,
    ActivePartyChanged,
    Mix,
    AutoSave,
    NowSave,
}

public class SaveManager : MonoSingleton<SaveManager>
{
    private readonly float DEBOUNCE_SECONDS = 5f; // Dirty후 5초간 추가 변경 판별
    private readonly float MIN_INTERVAL_SECONDS = 10f; // 저장 후 다음 저장 최소 주기 (최소 10초 1번 저장)

    private bool _isDirty;
    private float _lastSaveTime;
    private float _lastDirtyTime;
    private ESaveCategory _lastSaveCategory;

    /// <summary>
    /// 게임 종료 / 일시정지에 호출
    /// </summary>
    public void SaveNow()
    {
        _lastSaveCategory = ESaveCategory.NowSave;
        SaveNowInternal();
    }

    /// <summary>
    /// 특수 Save항목별 Save요청 함수
    /// </summary>
    /// <param name="saveCategory">Save항목</param>
    public void RequestSave(ESaveCategory saveCategory)
    {
        _isDirty = true;
        _lastSaveCategory = saveCategory;
        _lastDirtyTime = Time.unscaledTime;
    }

    private void SaveNowInternal()
    {
        PlayerSaveData data = BuildSaveData();
        LocalSaveSystem.Save(data);

        _isDirty = false;
        _lastSaveTime = Time.unscaledTime;
    }    

    private void Update()
    {
        if (!_isDirty)
            return;

        float now = Time.unscaledTime;

        bool isDebounce = (now - _lastDirtyTime) >= DEBOUNCE_SECONDS;
        bool isMinSaveInterval = (now - _lastSaveTime) >= MIN_INTERVAL_SECONDS;

        if (isDebounce && isMinSaveInterval)
        {
            SaveNowInternal();
        }
    }

    public bool TryLoadOnStart()
    {
        if (!LocalSaveSystem.TryLoad(out var data))
            return false;

        ApplyLoadData(data);
        return true;
    }

    private PlayerSaveData BuildSaveData()
    {
        var data = new PlayerSaveData
        {
            Gold = PlayerManager.Instance.CurrentCurrencyValue,
            LastSelectedPartyPresetIndex = PlayerManager.Instance.PartyService.CurrentSelectedPartyPresetIndex,
            LastSelectedIndexInParty = PlayerManager.Instance.PartyService.SelectedIndexInParty,
            InventoryInstanceId = PlayerManager.Instance.Inventory.GetCurrentInventoryInstanceID(),
            Inventory = PlayerManager.Instance.Inventory.GetSaveInventoryData(),
            Parties = PlayerManager.Instance.PartyService.GetSavePartyData(),
            Characters = PlayerManager.Instance.CharacterService.GetCharacterSaveData(),
        };
        return data;
    }

    private void ApplyLoadData(PlayerSaveData data)
    {
        PlayerManager.Instance.Inventory.Initialize(data.Inventory);
        PlayerManager.Instance.Inventory.SetInventoryInstanceID(data.InventoryInstanceId);
        PlayerManager.Instance.CharacterService.Initialize(data.Characters);
        PlayerManager.Instance.PartyService.SetCurrentSelectedPartyPresetIndex(data.LastSelectedPartyPresetIndex);
        PlayerManager.Instance.PartyService.SetSelectedIndexInParty(data.LastSelectedIndexInParty);
        PlayerManager.Instance.PartyService.Initialize(data.Parties);
        WeaponManager.Instance.Initialize();
        EquipmentManager.Instance.Initialize();
        PlayerManager.Instance.UpdateCurrentCurrencyValue(data.Gold);
    }
}
