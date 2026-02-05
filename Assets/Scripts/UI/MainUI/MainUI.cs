using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainUI : MonoBehaviour
{
    [SerializeField]
    private Canvas _canvas;

    [SerializeField]
    private GraphicRaycaster _graphicRaycaster;

    [SerializeField]
    private PartnerInfoElement[] _partnerInfos;

    [SerializeField]
    private GameObject _menuPanel;

    [SerializeField]
    private Button _menuButton;

    [SerializeField]
    private Button _inventoryButton;

    [SerializeField]
    private GameObject _pcSkillUI;

    [SerializeField]
    private GameObject _mobileSkillUI;

    private IUIManagerService _uiManagerService => UIManager.Instance;

    private void Awake()
    {
        TryGetComponent(out _canvas);
        TryGetComponent(out _graphicRaycaster);

        _menuButton.onClick.AddListener(OnClickMenuButton);
        _inventoryButton.onClick.AddListener(OnClickInventoryButton);

        InGameManager.Instance.OnActiveControlInShop += OnActiveInteractShopState;

#if UNITY_STANDALONE
        _pcSkillUI.gameObject.SetActive(true);
        _mobileSkillUI.gameObject.SetActive(false);
#else
        _pcSkillUI.gameObject.SetActive(false);
        _mobileSkillUI.gameObject.SetActive(true);
#endif
    }

    private void OnDestroy()
    {
        _menuButton.onClick.RemoveListener(OnClickMenuButton);
        _inventoryButton.onClick.RemoveListener(OnClickInventoryButton);

        InGameManager.Instance.OnActiveControlInShop -= OnActiveInteractShopState;
    }

    private void OnClickMenuButton()
    {
        _menuPanel.SetActive(true);
    }

    private async void OnClickInventoryButton()
    {
        var popup = await _uiManagerService.Show<InventoryPopup>();
        popup.HandleSelectCategoryUpdated((int)EItemCategory.Weapon);
    }

    private void OnActiveInteractShopState(bool isActive)
    {
        _canvas.enabled = isActive;
        _graphicRaycaster.enabled = isActive;
    }
}
