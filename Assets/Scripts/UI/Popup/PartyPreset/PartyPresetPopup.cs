using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PartyPresetPopup : BasePopup
{
    #region Inspector
    [SerializeField]
    private TextMeshProUGUI _presetNumText;

    [SerializeField]
    private Button _closeButton;

    [SerializeField]
    private Button _confirmButton;

    [SerializeField]
    private PresetToggle[] _presetToggles;

    [SerializeField]
    private EditPartyElement[] _editPartyElements;
    #endregion

    private int _selectedPresetIndex;
    private int _originPresetIndex;
    private Party _originalPartySnapshot;
    
    private IPartyService _partyService = PlayerManager.Instance.PartyService;    
    private bool _isChangeActiveParty;

    protected override void Awake()
    {
        base.Awake();

        _closeButton.onClick.AddListener(OnClickCloseButton);
        _confirmButton.onClick.AddListener(OnClickConfirmButton);

        _partyService.OnPartyDataAdjusted += HandlePartyDataAdjusted;                      
    }

    private void OnDestroy()
    {
        _partyService.OnPartyDataAdjusted -= HandlePartyDataAdjusted;
    }

    public override void Show()
    {
        base.Show();

        _originPresetIndex = GetCurrentActiveParty().Index;
        _originalPartySnapshot = _partyService.CurrentParty.Clone();

        _confirmButton.interactable = false;
        _isChangeActiveParty = false;

        InitEditPartyButton();
        InitPresetToggleText();

        SetCurrentPresetIndex(_originPresetIndex);

        int presetNum = _partyService.GetPartyPresetArrayIndex(_originPresetIndex);
        _presetToggles[presetNum].SetToggleIsOn(true);
    }

    private Party GetCurrentActiveParty()
    {
        return _partyService.CurrentParty;
    }

    //.. 파티 편집
    private void InitEditPartyButton()
    {
        int count = _editPartyElements.Length;
        for (int i = 0; i < count; i++)
        {
            _editPartyElements[i].Init(_selectedPresetIndex, i, _partyService.GetParty(_partyService.CurrentSelectedPartyPresetIndex));
        }
    }

    private void InitPresetToggleText()
    {        
        int count = _presetToggles.Length;
        for (int i = 0; i < count; i++)
        {
            _presetToggles[i].InitTogglePreset(i, SetCurrentPresetIndex);
            _presetToggles[i].SetToggleText(i + 1);
        }
    }

    private void SetCurrentPresetIndex(int index)
    {
        _selectedPresetIndex = index;
        _presetNumText.text = $"Party {index + 1}";

        var selectedPartyData = _partyService.GetParty(_selectedPresetIndex);

        int count = _editPartyElements.Length;
        for (int i = 0; i < count; i++)
        {
            _editPartyElements[i].UpdateEditPartyElement(_selectedPresetIndex, selectedPartyData);
        }

        UpdateConfirmButtonState();
    }


    private void UpdateConfirmButtonState()
    {
        bool isPresetIndexDifferent = _originPresetIndex != _selectedPresetIndex;
        if (isPresetIndexDifferent)
        {
            var newParty = _partyService.GetParty(_selectedPresetIndex);
            _confirmButton.interactable = newParty.GetMemberCount() > 0;
            return;
        }
        
        _confirmButton.interactable = CheckPartyMemberChangedInPreset(); //true; 아무것도 없는 프리셋 선택해서 1명장착 -> 해제 하면 버튼 활성화 돼있는 이슈가 있어 임시 조치..
    }

    private bool CheckPartyMemberChangedInPreset()
    {
        if (_originalPartySnapshot != null)
        {            
            for (int i = 0; i < _partyService.CurrentParty.Characters.Length; i++)
            {
                if (_partyService.GetParty(_selectedPresetIndex).Characters[i] != _originalPartySnapshot.Characters[i])                    
                {
                    return true;
                }
            }            
        }
        return false;
    }

    private void OnClickConfirmButton()
    {        
        _partyService.ChangeActiveParty(_selectedPresetIndex);
        _isChangeActiveParty = true;
        UIManager.Instance.Hide();
    }

    private void OnClickCloseButton()
    {
        if (_originalPartySnapshot != null)
        {
            _partyService.UpdateParty(_originalPartySnapshot.Index, _originalPartySnapshot);
        }
        UIManager.Instance.Hide();
    }

    public override void Hide()
    {
        if (_originalPartySnapshot != null && _isChangeActiveParty == false)
        {
            _partyService.UpdateParty(_originalPartySnapshot.Index, _originalPartySnapshot);
        }
        base.Hide();
    }

    #region Handle
    private void HandlePartyDataAdjusted()
    {
        //.. TODO :: 추가로 UI 업데이트 필요하면 여기에 함수 추가
        UpdateConfirmButtonState();
    }
    #endregion
}
