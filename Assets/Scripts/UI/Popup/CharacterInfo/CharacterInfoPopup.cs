using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInfoPopup : PopupWithPreview
{
    private ECategory _currentCategory = ECategory.Characters;

    #region Inspector
    [SerializeField]
    private InfoSubPage[] _subPages = null;
    private InfoSubPage _currentSubPage = null;

    [SerializeField]
    private PreviewController _previewCtrl = null;

    [SerializeField]
    private Button _closeButton;

    [SerializeField]
    private Button _characterListButton;    

    [SerializeField]
    private Button[] _categoryButtons;

    [SerializeField]
    private GameObject _elementPrefab;

    [SerializeField]
    private Transform _elementRoot;
    #endregion

    private List<InfoElement> _elementList = new List<InfoElement>();

    protected override void Awake()
    {
        base.Awake();

        _closeButton.onClick.AddListener(OnClickCloseButton);
        _characterListButton.onClick.AddListener(OnClickCharacterListButton);

        foreach (var subPage in _subPages)
        {
            subPage.Initialize(this);            
        }

        for (int i = 0; i < _categoryButtons.Length; i++)
        {
            int index = i;
            var button = _categoryButtons[i];
            button.onClick.AddListener(() =>
            {
                OnChangePage((ECategory)index);
            });
        }

        CreateElementList();
    }

    private void OnDestroy()
    {
        _closeButton.onClick.RemoveListener(OnClickCloseButton);
    }

    public override void Show()
    {
        OnChangePage(ECategory.Characters);        

        base.Show();
    }

    public override void Hide()
    {
        base.Hide();
    }

    public void ShowSelectedCharacterInfo(int _index)
    {
        OnSelectElement(_index);
    }

    private void OnChangePage(ECategory category)
    {
        var newPage = _subPages[(int)category];
        if (_currentSubPage == newPage)
        {
            return;
        }

        if (_currentSubPage != null)
        {
            _currentSubPage.gameObject.SetActive(false);
        }
        _currentCategory = category;
        _currentSubPage = newPage;
        newPage.gameObject.SetActive(true);        
        newPage.SetPreviewImage();        
    }

    private void CreateElementList()
    {
        var haveCharacterDict = PlayerManager.Instance.CharacterService.HaveCharacterDict;
        foreach (var item in haveCharacterDict.Values)
        {
            CreateElement(item.TemplateId);
        }
    }

    private void CreateElement(int index)
    {
        GameObject elementObj = Instantiate(_elementPrefab, _elementRoot);
        elementObj.TryGetComponent(out InfoElement element);
        element.Setup(index, OnSelectElement);

        _elementList.Add(element);
    }

    private void OnClickCloseButton()
    {        
        UIManager.Instance.Hide();
    }

    private async void OnClickCharacterListButton()
    {
        var popup = await UIManager.Instance.Show<PreviewCharacterListPopup>();
        popup.InitListView(currentElementIndex, _previewCtrl);        
    }
}
