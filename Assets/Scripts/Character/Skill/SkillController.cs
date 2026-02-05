using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillController : MonoBehaviour, ISkillUsageObserver
{    
    private PlayerController _playerCtrl;

    private Dictionary<ESkillType, float> _runtimeSkillCooldown = new Dictionary<ESkillType, float>();

    private SkillManager _skillManager;

    private void Awake()
    {
        foreach (ESkillType skillType in Enum.GetValues(typeof(ESkillType)))
        {
            if (skillType != ESkillType.End)
            {
                _runtimeSkillCooldown[skillType] = 0f;
            }
        }

        TryGetComponent(out _playerCtrl);

        _skillManager = SkillManager.Instance;
        _skillManager.Subscribe(this);
    }

    private void Update()
    {
        int count = _runtimeSkillCooldown.Count;
        for (int i = 0; i < count; i++)
        {
            var skillType = (ESkillType)i;
            float coolDown = _runtimeSkillCooldown[skillType];

            if (coolDown > 0f)
            {
                coolDown = Mathf.Max(0f, coolDown - Time.deltaTime);
                _runtimeSkillCooldown[skillType] = coolDown;
            }
        }
    }

    private void OnDestroy()
    {
        _skillManager.Unsubscribe(this);
    }

    public float GetCurrentCooldown(ESkillType type)
    {
        if (_runtimeSkillCooldown.TryGetValue(type, out float coolDown))
        {
            return coolDown;
        }

        return 0f;
    }

    public bool IsPossibleUseSkill(ESkillType type)
    {
        return GetCurrentCooldown(type) <= 0f;
    }

    public float GetConfigCooldown(ESkillType type)
    {        
        switch (type)
        {
            case ESkillType.Skill:
                return _playerCtrl.CharacterData.SkillGroup.Skill.SkillInfo.Cooldown;                
            case ESkillType.SpecialSkill:
                return _playerCtrl.CharacterData.SkillGroup.SpecialSkill.SkillInfo.Cooldown;                
            case ESkillType.UltimateSkill:
                return _playerCtrl.CharacterData.SkillGroup.UltimateSkill.SkillInfo.Cooldown;                
            case ESkillType.End:                
            default:
                return 0;
        }        
    }

    public float GetConfigDamageRate(ESkillType type)
    {
        switch (type)
        {
            case ESkillType.Skill:
                return _playerCtrl.CharacterData.SkillGroup.Skill.DamageRate;
            case ESkillType.SpecialSkill:
                return _playerCtrl.CharacterData.SkillGroup.SpecialSkill.DamageRate;
            case ESkillType.UltimateSkill:
                return _playerCtrl.CharacterData.SkillGroup.UltimateSkill.DamageRate;
            case ESkillType.End:                
            default:
                return 0;
        }
    }

    public void OnSkillUsed(ESkillType skillType, float cooldown)
    {
        IPartyService partyService = PlayerManager.Instance.PartyService;
        if (_playerCtrl == InGameManager.Instance.PlayerControllerDict[partyService.GetCurrentCharacterInActiveParty().TemplateId]) // 캐릭터 쿨타임 개별적용을 위한 비교..
        {
            if (_runtimeSkillCooldown.ContainsKey(skillType))
            {
                _runtimeSkillCooldown[skillType] = cooldown;                
            }
        }
    }
}