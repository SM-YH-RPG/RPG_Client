using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectorPopup : BasePopup
{
    #region inspector
    [SerializeField]
    private Button _closeButton;

    [SerializeField]
    private Button _changeButton;

    [SerializeField]
    private Button _infoButton;

    [SerializeField]
    private ToggleGroup _toggleGroup;

    [SerializeField]
    private Transform _createTransform;

    [SerializeField]
    private GameObject _partyListCharacterPrefab;

    [SerializeField]
    private PartyListSkillToggle[] _skillToggles;

    [SerializeField]
    private TextMeshProUGUI _characterName;

    [SerializeField]
    private TextMeshProUGUI _skillName;

    [SerializeField]
    private TextMeshProUGUI _skillDesc;
    #endregion

    private List<PartyCharacterElement> _characterElementList = new List<PartyCharacterElement>();
    private int _indexInParty;
    private int _targetPresetIndex;
    private RuntimeCharacter _selectedCharacter;
    private SkillGroupConfig _selectedCharacterSkillGroup;

    private PlayerManager _playerManager => PlayerManager.Instance;
    private IPartyService _partyService => PlayerManager.Instance.PartyService;

    protected override void Awake()
    {
        base.Awake();

        _closeButton.onClick.AddListener(OnClickCloseButton);
        _infoButton.onClick.AddListener(OnClickInfoButton);
        _changeButton.onClick.AddListener(OnClickChangeButton);
    }

    public void Initialize(int presetIndex, int indexInPartySlot)
    {
        _targetPresetIndex = presetIndex;
        _indexInParty = indexInPartySlot;

        List<RuntimeCharacter> charactersToShow = GenerateCharacterList();
        PopulateCharacterElements(charactersToShow);

        if (_characterElementList.Count > 0)
        {
            _characterElementList[0].OnClickCharacterButton(true);
        }
    }

    private List<RuntimeCharacter> GenerateCharacterList()
    {
        var targetParty = _partyService.GetParty(_targetPresetIndex);
        var allOwnedCharacters = _playerManager.CharacterService.HaveCharacterDict.Values;

        List<RuntimeCharacter> charactersInParty = new List<RuntimeCharacter>();
        List<RuntimeCharacter> charactersNotInParty = new List<RuntimeCharacter>();

        foreach (var ownedChar in allOwnedCharacters)
        {
            bool isInParty = false;
            foreach (var partyChar in targetParty.Characters)
            {
                var character = PlayerManager.Instance.CharacterService.GetRunTimeCharacterBy(partyChar);
                if (character != null)
                {
                    if (ownedChar.TemplateId == PlayerManager.Instance.CharacterService.GetRunTimeCharacterBy(partyChar).TemplateId)
                    {
                        isInParty = true;
                        break;
                    }
                }                
            }

            if (isInParty)
            {
                charactersInParty.Add(ownedChar);
            }
            else
            {
                charactersNotInParty.Add(ownedChar);
            }
        }

        List<RuntimeCharacter> combinedList = new List<RuntimeCharacter>();
        combinedList.AddRange(charactersInParty);
        combinedList.AddRange(charactersNotInParty);

        return combinedList;
    }

    private void PopulateCharacterElements(List<RuntimeCharacter> charactersToShow)
    {
        while (_characterElementList.Count < charactersToShow.Count)
        {
            GameObject elementObject = Instantiate(_partyListCharacterPrefab, _createTransform);
            var element = elementObject.GetComponent<PartyCharacterElement>();
            if (element != null)
            {
                _characterElementList.Add(element);
            }
        }

        for (int i = charactersToShow.Count; i < _characterElementList.Count; i++)
        {
            _characterElementList[i].gameObject.SetActive(false);
        }

        var targetParty = _partyService.GetParty(_targetPresetIndex);
        var charactersInParty = targetParty.Characters;

        charactersToShow.Sort((a, b) => // 캐릭터 파티 인덱스번호 순서로 정렬
        {
            int aIndex = GetIndexInParty(a, charactersInParty);
            int bIndex = GetIndexInParty(b, charactersInParty);

            return aIndex.CompareTo(bIndex);
        });

        for (int i = 0; i < charactersToShow.Count; i++)
        {
            _characterElementList[i].gameObject.SetActive(true);

            RuntimeCharacter currentCharacter = charactersToShow[i];
            int indexInParty = GetIndexInParty(currentCharacter, charactersInParty);

            _characterElementList[i].updateCharacterData(
                _targetPresetIndex,
                indexInParty == int.MaxValue ? -1 : indexInParty,
                currentCharacter,
                _characterName,
                _toggleGroup,
                SetSelectedCharacter);
        }
    }

    private int GetIndexInParty(RuntimeCharacter character, long[] charactersInParty)
    {
        for (int j = 0; j < charactersInParty.Length; j++)
        {
            var partyChar = PlayerManager.Instance.CharacterService.GetRunTimeCharacterBy(charactersInParty[j]);

            if (partyChar != null && partyChar.TemplateId == character.TemplateId)
            {
                return j;
            }
        }

        return int.MaxValue; // 파티에 없으면 맨 뒤로
    }

    private void UpdateSkillToggle()
    {
        int skillCount = _skillToggles.Length;
        for (int i = 0; i < skillCount; i++)
        {
            switch (i)
            {
                case (int)ECharacterSkillOrder.WeekAttack:
                    _skillToggles[i].InitSkillToggleData(_selectedCharacterSkillGroup.SkillUIGroup.WeekAttack);
                    break;
                case (int)ECharacterSkillOrder.StrongAttack:
                    if (_selectedCharacterSkillGroup.SkillUIGroup.StrongAttack.isVisible == false)
                    {
                        // Rifle캐릭터 강공격 X..
                        _skillToggles[i].gameObject.SetActive(false);
                    }
                    else
                    {
                        _skillToggles[i].gameObject.SetActive(true);
                        _skillToggles[i].InitSkillToggleData(_selectedCharacterSkillGroup.SkillUIGroup.StrongAttack);
                    }
                    break;
                case (int)ECharacterSkillOrder.Skill:
                    _skillToggles[i].InitSkillToggleData(_selectedCharacterSkillGroup.SkillUIGroup.Skill);
                    break;
                case (int)ECharacterSkillOrder.UltimateSkill:
                    _skillToggles[i].InitSkillToggleData(_selectedCharacterSkillGroup.SkillUIGroup.UltimateSkill);
                    break;
                default:
                    break;
            }
        }
        _skillToggles[0].SelectSkillToggle(true);
    }

    private void SetSelectedCharacter(RuntimeCharacter _character)
    {
        _selectedCharacter = _character;
        _selectedCharacterSkillGroup = InGameManager.Instance.PlayerSkillGroupConfigDict[_selectedCharacter.TemplateId];
        UpdateSkillToggle();
    }

    private async void OnClickInfoButton()
    {        
        var popup = await UIManager.Instance.Show<CharacterInfoPopup>();
        popup.ShowSelectedCharacterInfo(_selectedCharacter.TemplateId);
    }

    private void OnClickChangeButton()
    {
        _partyService.AssignCharacterToPartySlot(_targetPresetIndex, _indexInParty, _selectedCharacter);

        UIManager.Instance.Hide();
    }

    private void OnClickCloseButton()
    {
        UIManager.Instance.Hide();
    }
}
