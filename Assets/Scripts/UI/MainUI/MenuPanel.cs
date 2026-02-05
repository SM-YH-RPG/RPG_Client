using UnityEngine;
using UnityEngine.UI;

public class MenuPanel : MonoBehaviour
{
    [SerializeField] private GameObject _menuPanel;
    [SerializeField] private Button _menuCloseButton;
    [SerializeField] private Button _characterInfoButton;
    [SerializeField] private Button _partyButton;
    [SerializeField] private Button _mergeButton;

    private IPartyService _partyService;

    private void Awake()
    {
        _partyService = PlayerManager.Instance.PartyService;

        _menuCloseButton.onClick.AddListener(OnClickMenuClose);
        _characterInfoButton.onClick.AddListener(OnClickCharacterInfoButton);
        _partyButton.onClick.AddListener(OnClickPartyButton);
        _mergeButton.onClick.AddListener(OnClickMergeButton);
    }

    private void OnClickMenuClose()
    {
        _menuPanel.SetActive(false);
    }

    private async void OnClickCharacterInfoButton()
    {
        var popup = await UIManager.Instance.Show<CharacterInfoPopup>();
        int selectedCharacterIndex = PlayerManager.Instance.CharacterService.GetRunTimeCharacterBy(_partyService.CurrentParty.Characters[_partyService.SelectedIndexInParty]).TemplateId;
        popup.OnSelectElement(selectedCharacterIndex);
    }

    private async void OnClickPartyButton()
    {
        await UIManager.Instance.Show<PartyPresetPopup>();
    }

    private async void OnClickMergeButton()
    {
        var popup = await UIManager.Instance.Show<MergePopup>();
        popup.CreateMergeItemList();
    }
}
