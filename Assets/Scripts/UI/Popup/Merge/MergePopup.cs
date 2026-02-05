using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class MergePopup : BasePopup
{
    [SerializeField] private GameObject _mergeItemPrefab;
    [SerializeField] private Transform _createRoot;    
    [SerializeField] private ToggleGroup _toggleGroup;
    [SerializeField] private TextMeshProUGUI _currencyValue;
    [SerializeField] private MergeCraftDetail _detailView;
    [SerializeField] private Button _closeButton;

    private List<MergeItemElement> _itemList = new List<MergeItemElement>();

    protected override void Awake()
    {
        base.Awake();

        _closeButton.onClick.AddListener(OnClickCloseButton);
        SetCurrentCurrencyValue();        
        PlayerManager.Instance.OnCurrencyValueChanged += SetCurrentCurrencyValue;
    }

    private void OnDestroy()
    {
        PlayerManager.Instance.OnCurrencyValueChanged -= SetCurrentCurrencyValue;
    }

    public void CreateMergeItemList()
    {
        var mergeItemList = MergeDataManager.Instance.MergeItemConfig.MergeItemConfigs;
        int itemCount = mergeItemList.Count;
        while (_itemList.Count < itemCount)
        {
            GameObject elementObject = Instantiate(_mergeItemPrefab, _createRoot);
            if (elementObject.TryGetComponent(out MergeItemElement item))
            {                
                _itemList.Add(item);
            }
        }      
        
        for (int i = 0; i < _itemList.Count; i++)
        {
            bool isActive = i < itemCount;
            _itemList[i].gameObject.SetActive(isActive);
            if (isActive)
            {
                var itemConfig = ItemDataManager.Instance.GetItemConfigData(mergeItemList[i].Category, mergeItemList[i].ItemIndex);
                _itemList[i].InitMergeItemElement(mergeItemList[i], itemConfig, _toggleGroup, SetDetailViewData);
            }
        }
        _itemList[0].OnSelectToggle(true);
    }

    private void SetDetailViewData(MergeItemConfigData config)
    {
        _detailView.UpdateDetailView(config);
    }

    private void SetCurrentCurrencyValue()
    {
        _currencyValue.text = PlayerManager.Instance.CurrentCurrencyValue.ToString();
    }

    private void OnClickCloseButton()
    {
        UIManager.Instance.Hide();
    }
}
