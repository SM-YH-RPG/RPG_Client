
using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
    public InputActionSystem InputActions { get; private set; }
    public InputActionSystem.PlayerActions PlayerActions => InputActions.Player;
    public InputActionSystem.UIActions UIActions => InputActions.UI;

    private IPartyService _partyService => PlayerManager.Instance.PartyService;
    private IUIManagerService _UIManagerService => UIManager.Instance;

    private void Awake()
    {
        _UIManagerService.OnShowPopup += OnShowPopup;
        _UIManagerService.OnHidePopup += OnHidePopup;

        InputActions = new InputActionSystem();

        UIActions.Cancel.performed += UICancel;
        UIActions.CharacterInfoPopup.performed += ShowCharacterInfoPopup;
        UIActions.PartyPresetPopup.performed += ShowPartyPresetPopup;
        UIActions.InventoryPopup.performed += ShowInventoryPopup;
        UIActions.MergePopup.performed += ShowMergePopup;
        UIActions.CursorLockChange.started += OnCursorLockChange;
        UIActions.CursorLockChange.canceled += OnCursorLockChange;
        UIActions.SwapCharacter1.performed += CharacterSwap1;
        UIActions.SwapCharacter2.performed += CharacterSwap2;
        UIActions.SwapCharacter3.performed += CharacterSwap3;
    }

    private void OnHidePopup()
    {
#if UNITY_STANDALONE
        Cursor.lockState = CursorLockMode.Locked;
        PlayerActions.Enable();
#endif
    }

    private void OnShowPopup()
    {
#if UNITY_STANDALONE
        Cursor.lockState = CursorLockMode.Confined;
        PlayerActions.Disable();
#endif
    }

    private void OnDestroy()
    {
        _UIManagerService.OnShowPopup -= OnShowPopup;
        _UIManagerService.OnHidePopup -= OnHidePopup;

        UIActions.Cancel.performed -= UICancel;
        UIActions.CharacterInfoPopup.performed -= ShowCharacterInfoPopup;
        UIActions.PartyPresetPopup.performed -= ShowPartyPresetPopup;
        UIActions.InventoryPopup.performed -= ShowInventoryPopup;
        UIActions.MergePopup.performed -= ShowMergePopup;
        UIActions.CursorLockChange.started -= OnCursorLockChange;
        UIActions.CursorLockChange.canceled -= OnCursorLockChange;
        UIActions.SwapCharacter1.performed -= CharacterSwap1;
        UIActions.SwapCharacter2.performed -= CharacterSwap2;
        UIActions.SwapCharacter3.performed -= CharacterSwap3;
    }

    private void OnEnable()
    {
        InputActions.Enable();
    }

    private void OnDisable()
    {
        InputActions.Disable();        
    }

    private void OnCursorLockChange(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Started)
        {
            PlayerActions.Disable();
            Cursor.lockState = CursorLockMode.Confined;
        }

        if(context.phase == InputActionPhase.Canceled)
        {
            if(_UIManagerService.IsShow())
            {
                return;
            }

            PlayerActions.Enable();
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    private async void ShowCharacterInfoPopup(InputAction.CallbackContext context)
    {
        var popup = await _UIManagerService.Show<CharacterInfoPopup>();
        int selectedCharacterIndex = PlayerManager.Instance.CharacterService.GetRunTimeCharacterBy(_partyService.CurrentParty.Characters[_partyService.SelectedIndexInParty]).TemplateId;
        popup.OnSelectElement(selectedCharacterIndex);
    }

    private async void ShowPartyPresetPopup(InputAction.CallbackContext context)
    {
        await _UIManagerService.Show<PartyPresetPopup>();
    }

    private void CharacterSwap1(InputAction.CallbackContext context)
    {
        _partyService.RequestCharacterSwap(0);
    }

    private void CharacterSwap2(InputAction.CallbackContext context)
    {
        _partyService.RequestCharacterSwap(1);
    }

    private void CharacterSwap3(InputAction.CallbackContext context)
    {
        _partyService.RequestCharacterSwap(2);
    }

    private async void ShowInventoryPopup(InputAction.CallbackContext context)
    {
        var popup = await _UIManagerService.Show<InventoryPopup>();
        popup.HandleSelectCategoryUpdated((int)EItemCategory.Weapon);
    }

    private async void ShowMergePopup(InputAction.CallbackContext context)
    {
        // 임시 G키..나중에 인터랙션 생기거나 하면 제거 예정
         var popup = await _UIManagerService.Show<MergePopup>();
        popup.CreateMergeItemList();

    }

    private void UICancel(InputAction.CallbackContext context)
    {
        _UIManagerService.Hide();
    }
}