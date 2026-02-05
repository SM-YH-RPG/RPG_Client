using UnityEngine;
using UnityEngine.UI;

public class PartnerInfoElement : MonoBehaviour
{
    #region Inspector
    [SerializeField] private Button _swapButton;
    
    [SerializeField] private Image _selectImage;

    [SerializeField] private Image _thumbnailImage;

    [SerializeField] private Image _progressImage;

    [SerializeField] private GameObject _bindingKeyObject;
    #endregion

    private int _indexInPartySlot;
    private int _maxHp;
    private RuntimeCharacter _assignedCharacterData;

    private PlayerManager _playerManager => PlayerManager.Instance;
    private IPartyService _partyService => PlayerManager.Instance.PartyService;

    private void Awake()
    {
#if !UNITY_STANDALONE
        _swapButton.onClick.AddListener(OnClickSwapButton);
#endif
        CheckMobileButtonInteractable();
        
        _playerManager.CharacterService.OnCharacterStatUpdated += HandleCharacterHPGaugeUpdated;
        _partyService.OnCharacterSelectedInParty += HandleSelectionVisualsUpdated;
    }

    private void OnDestroy()
    {        
        _playerManager.CharacterService.OnCharacterStatUpdated -= HandleCharacterHPGaugeUpdated;
        _partyService.OnCharacterSelectedInParty -= HandleSelectionVisualsUpdated;
    }


    public async void InitPartyMember(int indexInParty, RuntimeCharacter character)
    {
        _indexInPartySlot = indexInParty;
        _assignedCharacterData = character;
           
        _thumbnailImage.sprite = await ResourcesManager.Instance.LoadSpriteAsync($"equip_chacacter_icon_{_assignedCharacterData.TemplateId}");

        HandleSelectionVisualsUpdated(_partyService.SelectedIndexInParty, _playerManager.CharacterService.GetRunTimeCharacterBy(_partyService.CurrentParty.Characters[_partyService.SelectedIndexInParty]));
        UpdateThumbnailAndHPBar();
    }

    private void UpdateThumbnailAndHPBar()
    {
        if (_assignedCharacterData.TemplateId == -1)
        {
            _thumbnailImage.enabled = false;
            _progressImage.fillAmount = 0f;
            return;
        }

        _thumbnailImage.enabled = true;
        
        _progressImage.fillAmount = (float)_assignedCharacterData.CurrentHP / _assignedCharacterData.MaxHp;
    }


    private void CheckMobileButtonInteractable()
    {
#if UNITY_STANDALONE
        _swapButton.interactable = false;
#else
        _swapButton.interactable = true;
#endif
    }

    private void OnClickSwapButton()
    {
        if (_assignedCharacterData.TemplateId != -1)
        {
            _partyService.RequestCharacterSwap(_indexInPartySlot);
        }
    }

    #region Handle
    private void HandleSelectionVisualsUpdated(int newlySelectedIndexInParty, RuntimeCharacter selectedCharacter)
    {
        bool isSelected = _partyService.SelectedIndexInParty == _indexInPartySlot;
        _selectImage.enabled = isSelected;

        _bindingKeyObject.SetActive(!isSelected);
    }

    private void HandleCharacterHPGaugeUpdated(long indexThatChanged, RuntimeCharacter characterDataThatChanged)
    {
        if (indexThatChanged == _indexInPartySlot)
        {
            _assignedCharacterData = characterDataThatChanged;
            UpdateThumbnailAndHPBar();
        }
    }
    #endregion
}
