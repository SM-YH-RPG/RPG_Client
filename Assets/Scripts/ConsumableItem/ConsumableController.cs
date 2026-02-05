using System;
using System.Collections.Generic;
using UnityEngine;

public class ConsumableController : MonoBehaviour, IConsumableItemUsageObserver
{
    private Dictionary<EConsumableEffectType, Dictionary<int, float>> _usageConsumableItemCooldownDict = new Dictionary<EConsumableEffectType, Dictionary<int, float>>();

    private List<int> templateIndexList = new List<int>();

    private ConsumableItemManager _consumableItemManager;

    private void Start()
    {        
        foreach (EConsumableEffectType type in Enum.GetValues(typeof(EConsumableEffectType)))
        {
            if (type != EConsumableEffectType.End)
            {
                _usageConsumableItemCooldownDict[type] = new Dictionary<int, float>();
            }            
        }

        var itemConfigArray = ItemDataManager.Instance.ConsumeableItemConfig.ConsumeableItemDataConfigs;
        for (int i = 0; i < itemConfigArray.Count; i++)
        {
            _usageConsumableItemCooldownDict[itemConfigArray[i].Type].Add(itemConfigArray[i].Index, 0f);
        }

        _consumableItemManager = ConsumableItemManager.Instance;
        _consumableItemManager.Subscribe(this);

        InGameManager.Instance.InitConsumableController(this);
    }

    private void Update()
    {        
        foreach (var typeDict in _usageConsumableItemCooldownDict)
        {
            var cooldownDict = typeDict.Value;
            templateIndexList.Clear();

            foreach (var key in cooldownDict.Keys)
            {
                templateIndexList.Add(key);
            }

            for (int i = 0; i < templateIndexList.Count; i++)
            {
                int templateIndex = templateIndexList[i];
                float coolTime = cooldownDict[templateIndex];

                if (coolTime <= 0f)
                {
                    continue;
                }

                float newCoolTime = coolTime - Time.deltaTime;
                if (newCoolTime <= 0f)
                {
                    newCoolTime = 0f;
                }
                cooldownDict[templateIndex] = newCoolTime;
            }
        }
    }

    private void OnDestroy()
    {
        _consumableItemManager.Unsubscribe(this);
    }

    public float GetUsageItemCooldown(EConsumableEffectType type, int templateIndex)
    {
        if (_usageConsumableItemCooldownDict.TryGetValue(type, out var cooldownDict))
        {
            if (cooldownDict.TryGetValue(templateIndex, out float cooldown))
            {
                return cooldown;
            }
        }

        return 0;
    }

    public void OnConsumableItemUsed(EConsumableEffectType type, int templateIndex, float cooldown)
    {
        if (_usageConsumableItemCooldownDict.TryGetValue(type, out var cooldownDict))
        {
            if (cooldownDict.ContainsKey(templateIndex))
            {
                cooldownDict[templateIndex] = cooldown;
            }
        }
    }

    public bool TryUseConsumableItem(ConsumableItem item, RuntimeCharacter character)
    {
        switch (item.ConsumableEffectType)
        {
            case EConsumableEffectType.HPRecovery:
                if (character.CurrentHP > 0 && character.CurrentHP < character.MaxHp)
                {
                    UpdateCharacterDataOnUsedConsumableItem(item, character);
                    return true;
                }
                else
                {
                    return false;
                }
            default:
                return false;
        }
    }

    private void UpdateCharacterDataOnUsedConsumableItem(ConsumableItem item, RuntimeCharacter character)
    {
        character.CurrentHP += (int)item.ConsumableEffectValue;
        PlayerManager.Instance.Inventory.RemoveItem(EItemCategory.Consumable, item.TemplateId);
        PlayerManager.Instance.CharacterService.UpdateCurrentCharacterHp(character.CurrentHP, character.InstanceId);
    }
}
