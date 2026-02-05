using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EditPartyElement : MonoBehaviour
{
    #region Inspector
    [SerializeField]
    private Sprite _nonCharacterBg;

    [SerializeField]
    private Sprite _CharacterBg;

    [SerializeField]
    private Image _backgroundImage;

    [SerializeField]
    private Image _characterImage;

    [SerializeField]
    private Image _plusImage;

    [SerializeField]
    private TextMeshProUGUI _levelText;

    [SerializeField]
    private TextMeshProUGUI _nameText;

    [SerializeField]
    private Button _editButton;
    #endregion

    private int _displayingPresetIndex;
    private int _indexInPartySlot;

    private IPartyService _partyService = PlayerManager.Instance.PartyService;

    private void Awake()
    {    
        _editButton.onClick.AddListener(OnClickEditPartyElement);
        _partyService.OnPartyDataAdjusted += HandlePartyDataAdjusted;        
    }

    private void OnDestroy()
    {
        _editButton.onClick.RemoveListener(OnClickEditPartyElement);

        _partyService.OnPartyDataAdjusted -= HandlePartyDataAdjusted;        
    }

    public void Init(int presetIndex, int indexInParty, Party party)
    {
        _displayingPresetIndex = presetIndex;
        _indexInPartySlot = indexInParty;

        UpdateEditPartyElement(presetIndex, party);
    }


    public async void UpdateEditPartyElement(int contextPresetIndex, Party contextParty)
    {
        if (_displayingPresetIndex != contextPresetIndex)
        {
            _displayingPresetIndex = contextPresetIndex;
        }

        RuntimeCharacter assignedCharacter = PlayerManager.Instance.CharacterService.GetRunTimeCharacterBy(contextParty.Characters[_indexInPartySlot]);
        if (assignedCharacter != null && assignedCharacter.TemplateId != -1)
        {
            _levelText.text = $"Lv.{assignedCharacter.Level}";
            
            CharacterConfig characterConfig = InGameManager.Instance.GetPlayerController(assignedCharacter.TemplateId).CharacterData;
            _nameText.text = characterConfig.Name;

            _plusImage.enabled = false;
            _characterImage.enabled = true;
            _backgroundImage.sprite = _CharacterBg;

            _characterImage.sprite = await AddressableManager.Instance.LoadAssetAsync<Sprite>($"party_member_icon_{assignedCharacter.TemplateId}");
        }
        else
        {
            _levelText.text = "";
            _nameText.text = "";
            _plusImage.enabled = true;
            _characterImage.enabled = false;
            _backgroundImage.sprite = _nonCharacterBg;
        }
    }

    private async void OnClickEditPartyElement()
    {        
        var popup = await UIManager.Instance.Show<CharacterSelectorPopup>();
        popup.Initialize(_displayingPresetIndex, _indexInPartySlot);
    }

    #region Handle
    private void HandlePartyDataAdjusted()
    {
        UpdateEditPartyElement(_displayingPresetIndex, _partyService.GetParty(_displayingPresetIndex));
    }
    #endregion
}
