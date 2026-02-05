using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractionElement : MonoBehaviour
{
    [SerializeField]
    private Color _highlightColor = Color.yellow;

    [SerializeField]
    private Color _defaultColor = Color.white;

    [SerializeField]
    private Image _background;

    [SerializeField]
    private Image _outline;

    [SerializeField]
    private GameObject _keyImage;

    [SerializeField]
    private TextMeshProUGUI _titleText;

    [SerializeField]
    private TextMeshProUGUI _inputKeyText;

    [SerializeField]
    private Button _interactButton;

    private IInteractable _currentInteracteObject;

    private void Awake()
    {
#if UNITY_ANDROID || UNITY_IOS
        _interactButton.interactable = true;
        _interactButton.onClick.AddListener(OnClickInteractButton);
#else
        _interactButton.interactable = false;
#endif
    }

    public void Setup(IInteractable interacteObject)
    {
        _currentInteracteObject = interacteObject;
        _titleText.text = interacteObject.InteractionTitle;
        _background.color = _defaultColor;
        _outline.enabled = false;
        SetInputTextOSType();
    }

    public void OnHighlight()
    {
        _outline.enabled = true;
        _background.color = _highlightColor;
        _keyImage.SetActive(true);
    }

    public void OnDefault()
    {
        _outline.enabled = false;
        _background.color = _defaultColor;
        _keyImage.SetActive(false);
    }

    private void SetInputTextOSType()
    {
#if UNITY_ANDROID || UNITY_IOS
        _inputKeyText.text = "+";
#else
        _inputKeyText.text = "F";
#endif
    }

    private void OnClickInteractButton()
    {
        if (_currentInteracteObject != null)
        {
            _currentInteracteObject.Interact();
        }
    }
}
