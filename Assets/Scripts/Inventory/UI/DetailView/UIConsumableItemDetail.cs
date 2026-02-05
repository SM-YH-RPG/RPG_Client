using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIConsumableItemDetail : UIItemDetailBase
{
    [SerializeField] private TextMeshProUGUI _categoryText;
    [SerializeField] private TextMeshProUGUI _currentCount;
    [SerializeField] private TextMeshProUGUI _itemEffectText;
    [SerializeField] private TextMeshProUGUI _itemDescText;
    [SerializeField] private TextMeshProUGUI _cooldownText;
    [SerializeField] private Image _itemCategoryImage;
    [SerializeField] private Image _itemGradeLine;
    [SerializeField] private Image _itemTextLine;
    [SerializeField] private Image _cooldownOverlay;
    [SerializeField] private Button _useButton;

    private ConsumableItem _currentItem;
    private ConsumableController _consumableCtrl;
    private float _maxCoolTime;
    private List<BaseInventoryItem> _filterdItemList = new List<BaseInventoryItem>();

    private void Awake()
    {        
        _useButton.onClick.AddListener(OnClickUseButton);
        _consumableCtrl = InGameManager.Instance.ConsumableController;
    }

    private void Update()
    {
        if (_consumableCtrl == null)
            return;

        if (_currentItem == null)
            return;

        float currentCoolTime = _consumableCtrl.GetUsageItemCooldown(_currentItem.ConsumableEffectType, _currentItem.TemplateId);
        UpdateCooldownUI(currentCoolTime);
    }

    public override void UpdateSelectView(BaseInventoryItem data)
    {
        if (data == null)
        {
            EmptyItemData();
            return;
        }

        if (data is ConsumableItem cousume)
        {
            _currentItem = cousume;
            UpdateItemData();
        }
    }

    private void UpdateItemData()
    {
        if (_currentItem == null)
        {
            EmptyItemData();
            return;
        }

        ItemConfigData config = ItemDataManager.Instance.GetItemConfigData(EItemCategory.Consumable, _currentItem.TemplateId);
        _itemImage.enabled = true;
        _itemCategoryImage.enabled = true;
        _itemTextLine.enabled = true;
        _itemImage.sprite = config.Sprite;
        _categoryText.text = "소모품";
        _currentCount.text = $"보유{GetCurrentItemCount(_currentItem)}";
        _itemName.text = config.Name;
        _itemEffectText.text = config.AffectDescription;
        _itemDescText.text = config.Description;
        _itemGradeLine.color = ItemDataManager.Instance.GetGradeColor((int)_currentItem.Grade);
        _useButton.interactable = true;
    }

    private void EmptyItemData()
    {
        _itemImage.enabled = false;
        _itemCategoryImage.enabled = false;
        _itemTextLine.enabled = false;
        _categoryText.text = string.Empty;
        _currentCount.text = string.Empty;
        _itemName.text = string.Empty;
        _itemEffectText.text = string.Empty;
        _itemDescText.text = string.Empty;
        _itemGradeLine.color = Color.white;
        _useButton.interactable = false;
        _cooldownOverlay.fillAmount = 0f;
        _cooldownText.text = string.Empty;
        _currentItem = null;
    }

    private int GetCurrentItemCount(BaseInventoryItem invenItem)
    {
        EItemCategory category = invenItem.Category;
        IReadOnlyList<BaseInventoryItem> itemDatas = PlayerManager.Instance.Inventory.GetInvenItemDataList(category);

        int totalCount = 0;
        if (itemDatas != null)
        {
            foreach (var item in itemDatas)
            {
                if (item.TemplateId == invenItem.TemplateId)
                {
                    totalCount += item.Amount;
                }
            }
            return totalCount;
        }

        return 0;
    }

    private async void OnClickUseButton()
    {
        RuntimeCharacter character = PlayerManager.Instance.PartyService.GetCurrentCharacterInActiveParty();
        if (character != null)
        {
            ConsumableItem currentItem = _currentItem;
            int templateIndex = _currentItem.TemplateId;
            if (InGameManager.Instance.ConsumableController.TryUseConsumableItem(_currentItem, character))
            {                
                // TryUseConsumableItem에서 아이템 사용처리 및 remove후 아이템이 사라졌어도 쿨타임은 적용돼야함.
                ConsumableItemManager.Instance.NotifyConsumableItemUsed(currentItem.ConsumableEffectType, currentItem.TemplateId, currentItem.CooldownSeconds);                

                if (_currentItem != null)
                {
                    var invenList = PlayerManager.Instance.Inventory.GetInvenItemDataList(EItemCategory.Consumable);
                    _filterdItemList.Clear();
                    for (int i = 0; i < invenList.Count; i++)
                    {
                        if (invenList[i].TemplateId == templateIndex)
                        {
                            _filterdItemList.Add(invenList[i]);
                        }
                    }

                    for (int i = 0; i < _filterdItemList.Count; i++)
                    {
                        ConsumableItem item = (ConsumableItem)_filterdItemList[i];
                        if (_currentItem.TemplateId == templateIndex)
                        {
                            // 아이템이 Remove된 후 다른 아이템이 존재하고 (소모품 아이템이 더 없다면 emptySetting출력하면 끝)
                            // _currentItem(사용아이템)과 templateID가 같은(똑같은 아이템)이 더 존재한다면 모두 쿨타임
                            StartCooldownVisual(item.CooldownSeconds);
                        }
                    }
                }

                UpdateItemData();
                var popup = await UIManager.Instance.Show<ToastMessagePopup>();
                popup.PlayToast("사용 완료");
            }
            else
            {
                switch (_currentItem.ConsumableEffectType)
                {
                    case EConsumableEffectType.HPRecovery:
                        if (character.CurrentHP >= character.MaxHp)
                        {
                            var popup = await UIManager.Instance.Show<ToastMessagePopup>();
                            popup.PlayToast("이미 체력이 가득 찼습니다");
                        }
                        break;
                }
            }            
        }
        else
        {
            Debug.Log("현재 플레이중인 캐릭터가 없습니다!!");
        }
    }

    private void StartCooldownVisual(float maxCooltime)
    {
        _maxCoolTime = maxCooltime;

        if (_cooldownText != null)
            _cooldownText.gameObject.SetActive(true);

        _useButton.interactable = false;

        UpdateCooldownUI(maxCooltime);
    }

    private void UpdateCooldownUI(float currentCooldownValue)
    {
        if (currentCooldownValue <= 0f)
        {
            currentCooldownValue = 0f;
            if (_cooldownOverlay != null)
                _cooldownOverlay.fillAmount = 0f;

            if (_cooldownText != null)
                _cooldownText.text = string.Empty;

            _useButton.interactable = true;
        }
        else
        {
            if (_cooldownOverlay != null)
            {
                _cooldownOverlay.fillAmount = currentCooldownValue / _maxCoolTime;
            }

            if (_cooldownText != null)
            {
                _cooldownText.gameObject.SetActive(true);
                _cooldownText.text = Mathf.Ceil(currentCooldownValue).ToString("0");
            }
            _useButton.interactable = false;
        }
    }
}
