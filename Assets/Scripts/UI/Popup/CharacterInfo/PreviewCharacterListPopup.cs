using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class PreviewCharacterListPopup : PopupWithPreview
{
    [SerializeField] private GameObject _characterListItemPrefab;
    [SerializeField] private Transform _createRoot;
    [SerializeField] private TextMeshProUGUI _characterName;
    [SerializeField] private Button _closeButton;

    private PreviewController _previewCtrl;
    private List<InfoElement> _itemList;
    private List<int> _characterIndexList;

    private int _prevCharacterIndex;

    protected override void Awake()
    {
        base.Awake();

        _closeButton.onClick.AddListener(OnClickCloseButton);
        _itemList = new List<InfoElement>();
        _characterIndexList = new List<int>();
    }

    public void InitListView(int index, PreviewController preview)
    {
        _prevCharacterIndex = index;
        _previewCtrl = preview;
        _characterIndexList.Clear();
        var characterList = PlayerManager.Instance.CharacterService.HaveCharacterDict;
        foreach (var item in characterList)
        {
            _characterIndexList.Add(item.Value.TemplateId);
        }
        int itemsToShowCount = characterList.Count;
        while (_itemList.Count < itemsToShowCount)
        {
            GameObject elementObject = Instantiate(_characterListItemPrefab, _createRoot);
            if (elementObject.TryGetComponent(out InfoElement item))
            {
                _itemList.Add(item);
            }
        }

        for (int i = 0; i < _itemList.Count; i++)
        {
            bool isActive = i < itemsToShowCount;
            _itemList[i].gameObject.SetActive(isActive);

            if (isActive)
            {
                _itemList[i].Setup(_characterIndexList[i], _previewCtrl.ChangePreviewCharacter, SetCharacterNameText);
            }
        }
        SetCharacterNameText(index);
    }

    private void SetCharacterNameText(int _index)
    {
        CharacterConfig config = InGameManager.Instance.GetPlayerController(_index).CharacterData;
        _characterName.text = config.Name;
    }

    public override void Hide()
    {
        _previewCtrl.ChangePreviewCharacter(_prevCharacterIndex);
        base.Hide();
    }

    private void OnClickCloseButton()
    {
        _previewCtrl.ChangePreviewCharacter(_prevCharacterIndex);
        UIManager.Instance.Hide();
    }
}
