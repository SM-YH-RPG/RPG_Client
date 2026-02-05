using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIEquipmentItemDetail : UIItemDetailBase
{
    [SerializeField] private Image _gradeLineImage;
    [SerializeField] private StatInfo[] _equipStatArray;
    [SerializeField] private TextMeshProUGUI _equipAffectText;
    [SerializeField] private TextMeshProUGUI _equipLevelText;
    [SerializeField] private TextMeshProUGUI _equipCostText;
    [SerializeField] private TextMeshProUGUI _currentEquipCharacterText;
    [SerializeField] private GameObject _currentEquipObject;
    [SerializeField] private Image _currentEquipCharacterImage;

    private void Awake()
    {
        EquipmentManager.Instance.OnEquipmentItemDataChanged += UpdateEquipmentDetailView;
    }

    private void OnDestroy()
    {
        EquipmentManager.Instance.OnEquipmentItemDataChanged -= UpdateEquipmentDetailView;
    }

    public override void UpdateSelectView(BaseInventoryItem data)
    {
        if (data is EquipmentItem equip)
        {
            UpdateEquipmentDetailView(equip);
        }
    }

    private void UpdateEquipmentDetailView(EquipmentItem equip)
    {        
        ItemConfigData itemConfig = ItemDataManager.Instance.GetItemConfigData(equip.Category, equip.TemplateId);
        _itemImage.sprite = itemConfig.Sprite;
        _itemName.text = itemConfig.Name;
        _gradeLineImage.color = ItemDataManager.Instance.GetGradeColor((int)equip.Grade);
        _equipAffectText.text = itemConfig.AffectDescription;
        if (equip.Tier >= EquipmentDataManager.Instance.MaxTier)
        {
            _equipLevelText.color = Color.red;
        }
        else
        {
            _equipLevelText.color = Color.white;
        }

        _equipLevelText.text = $"Tier {equip.Tier}";
        _equipCostText.text = $"COST {equip.EquipCost}";
        EquipmentStatDataInit(equip);
        if (equip.EquippedCharacterIndex != -1)
        {
            SetEquipCharacterData(equip);
        }
        _currentEquipObject.SetActive(equip.EquippedCharacterIndex != -1);
    }

    private void EquipmentStatDataInit(EquipmentItem equip)
    {
        int statCount = equip.SubStatDict.Count + 2; // SubΩ∫≈» + mainΩ∫≈» 2∞≥
        int statIndex = 0;

        SetStatData(statIndex++, equip.RandomMainStatType, equip.RandomMainStatValue);
        SetStatData(statIndex++, equip.MainStatType, equip.MainStatValue);

        foreach (var (statType, value) in equip.SubStatDict)
        {
            SetStatData(statIndex++, statType, value);
        }

        for (int i = statCount; i < _equipStatArray.Length; i++)
        {
            _equipStatArray[i].gameObject.SetActive(false);
        }
    }

    private void SetStatData(int index, EItemStatType type, float value)
    {
        StatInfo stat = _equipStatArray[index];
        stat.SetEquipItemStatData(value, type);
        stat.gameObject.SetActive(true);
    }

    private async void SetEquipCharacterData(EquipmentItem equip)
    {
        _currentEquipCharacterImage.sprite = await AddressableManager.Instance.LoadAssetAsync<Sprite>($"equip_chacacter_icon_{equip.EquippedCharacterIndex}");
        CharacterConfig config = InGameManager.Instance.GetPlayerController(equip.EquippedCharacterIndex).CharacterData;
        _currentEquipCharacterText.text = $"{config.Name} ¿Â¬¯ ¡ﬂ";
    }
}
