using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class EnhancePopupBase : BasePopup
{
    protected const int MAX_TIER = 5;
    #region Inspector
    [SerializeField] protected Image _itemPreview;
    [SerializeField] protected Image _arrowImage;
    [SerializeField] protected EnhanceStatInfo[] _statArray;
    [SerializeField] protected GameObject _materialPrefab;
    [SerializeField] protected Transform _createRoot;
    [SerializeField] protected TextMeshProUGUI _itemName;
    [SerializeField] protected TextMeshProUGUI _currentTier;
    [SerializeField] protected TextMeshProUGUI _nextTier;
    [SerializeField] protected TextMeshProUGUI _currencyValue;
    [SerializeField] protected TextMeshProUGUI _costValue;
    [SerializeField] protected Button _closeButton;
    [SerializeField] protected Button _enhanceButton;
    #endregion

    protected override void Awake()
    {
        base.Awake();

        _enhanceButton.onClick.AddListener(OnClickEnhanceButton);
        _closeButton.onClick.AddListener(OnClickCloseButton);

        PlayerManager.Instance.OnCurrencyValueChanged += UpdateCurrencyValue;
    }

    protected virtual void OnDestroy()
    {
        PlayerManager.Instance.OnCurrencyValueChanged -= UpdateCurrencyValue;
    }

    protected void UpdateCurrencyValue()
    {
        _currencyValue.text = PlayerManager.Instance.CurrentCurrencyValue.ToString();
    }

    protected void SetTierText(int tier)
    {
        _currentTier.text = $"Tier {tier}";
        if (tier >= MAX_TIER)
        {
            _currentTier.color = Color.red;
            _nextTier.text = "MAX Tier";
            _arrowImage.enabled = false;
        }
        else
        {
            _currentTier.color = Color.white;
            _nextTier.text = $"Tier {tier + 1}";
            _arrowImage.enabled = true;
        }
    }

    protected abstract void UpdateStat(int tier);
    protected abstract void CreateMaterialItems(EItemGrade grade, int tier);
    protected abstract bool CheckPossibleEnhance(EItemGrade grade, int tier);    
    protected abstract void OnClickEnhanceButton();
    protected abstract void SuccessEnhanceDataUpdate();
    protected abstract void FaildEnhanceDataUpdate();

    private void OnClickCloseButton()
    {
        UIManager.Instance.Hide();
    }
}
