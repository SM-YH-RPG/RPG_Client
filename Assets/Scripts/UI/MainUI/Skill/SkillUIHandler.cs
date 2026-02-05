using System.Collections.Generic;
using UnityEngine;

public class SkillUIHandler : MonoBehaviour, ISkillUsageObserver
{
    [SerializeField]
    private List<UISkillButton> _skillButtonList;

    [SerializeField]
    private SkillController _skillController;

    private IPartyService _partyService = PlayerManager.Instance.PartyService;

    private void Awake()
    {
        SkillManager.Instance.Subscribe(this);

        PlayerManager.Instance.PartyService.OnPartyCharacterSwappedSkillData += SetSwapCharacterSkillData;
    }

    private void Start()
    {
        SetSwapCharacterSkillData();
    }

    private void OnDestroy()
    {
        SkillManager.Instance.Unsubscribe(this);

        PlayerManager.Instance.PartyService.OnPartyCharacterSwappedSkillData -= SetSwapCharacterSkillData;
    }

    private void Update()
    {
        if (_skillController == null) return;

        foreach (var button in _skillButtonList)
        {
            float currentCooldown = _skillController.GetCurrentCooldown(button.SkillType);
            button.UpdateCooldownUI(currentCooldown);
        }
    }

    private void SetSwapCharacterSkillData()        
    {
        int characterIndex = PlayerManager.Instance.CharacterService.GetRunTimeCharacterBy(_partyService.CurrentParty.Characters[_partyService.SelectedIndexInParty]).TemplateId;
        _skillController = InGameManager.Instance.PlayerControllerDict[characterIndex].SkillCtrl;
        var skillGroupConfig = InGameManager.Instance.PlayerSkillGroupConfigDict[characterIndex];
        SetSkillIcon(skillGroupConfig);
    }

    private void SetSkillIcon(SkillGroupConfig config)
    {
        foreach (var button in _skillButtonList)
        {
            switch (button.SkillType)
            {
                case ESkillType.Skill:
                    button.SetSkillIcon(config.SkillUIGroup.Skill.skillUIInfo.SkillIcon);
                    break;
                case ESkillType.SpecialSkill:
                    button.SetSkillIcon(config.SkillUIGroup.SpecialSkill.skillUIInfo.SkillIcon);
                    break;
                case ESkillType.UltimateSkill:
                    button.SetSkillIcon(config.SkillUIGroup.UltimateSkill.skillUIInfo.SkillIcon);
                    break;                
            }

            if (button.SkillType == ESkillType.End && button.IsWeakAttackButtonType())
            {
                button.SetSkillIcon(config.SkillUIGroup.WeekAttack.skillUIInfo.SkillIcon);
            }
        }
    }

    public void OnSkillUsed(ESkillType skillType, float cooldown)
    {
        var button = _skillButtonList.Find(x => x.SkillType == skillType);
        if (button != null)
        {
            button.StartCooldownVisual(cooldown);
        }
    }
}
