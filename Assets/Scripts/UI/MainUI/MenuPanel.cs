using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MenuPanel : MonoBehaviour
{
    [SerializeField] private GameObject _menuPanel;
    [SerializeField] private Button _menuCloseButton;
    [SerializeField] private Button _characterInfoButton;
    [SerializeField] private Button _partyButton;
    [SerializeField] private Button _mergeButton;

    private IPartyService _partyService;
    private InputActionSystem _inputSystem;

    private void Awake()
    {
        _partyService = PlayerManager.Instance.PartyService;

        _menuCloseButton.onClick.AddListener(OnClickMenuClose);
        _characterInfoButton.onClick.AddListener(OnClickCharacterInfoButton);
        _partyButton.onClick.AddListener(OnClickPartyButton);
        _mergeButton.onClick.AddListener(OnClickMergeButton);

#if UNITY_STANDALONE
        _inputSystem = new InputActionSystem();
#endif
    }

#if UNITY_STANDALONE
    private void OnEnable()
    {
        _inputSystem.UI.Cancel.performed += UICancel;
        _inputSystem.Enable();
    }

    private void OnDisable()
    {
        _inputSystem.UI.Cancel.performed -= UICancel;
        _inputSystem.Disable();
    }
#endif

    private void OnClickMenuClose()
    {
        _menuPanel.SetActive(false);
    }

    private async void OnClickCharacterInfoButton()
    {
        var popup = await UIManager.Instance.Show<CharacterInfoPopup>();
        int selectedCharacterIndex = PlayerManager.Instance.CharacterService.GetRunTimeCharacterBy(_partyService.CurrentParty.Characters[_partyService.SelectedIndexInParty]).TemplateId;
        popup.OnSelectElement(selectedCharacterIndex);
        OnClickMenuClose();
    }

    private async void OnClickPartyButton()
    {
        await UIManager.Instance.Show<PartyPresetPopup>();
        OnClickMenuClose();
    }

    private async void OnClickMergeButton()
    {
        var popup = await UIManager.Instance.Show<MergePopup>();
        popup.CreateMergeItemList();
        OnClickMenuClose();
    }

#if UNITY_STANDALONE
    private void UICancel(InputAction.CallbackContext context)
    {
        OnClickMenuClose();
    }
#endif
}
