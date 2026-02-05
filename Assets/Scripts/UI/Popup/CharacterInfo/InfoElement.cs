using System;
using UnityEngine;
using UnityEngine.UI;

public class InfoElement : MonoBehaviour
{
    protected int _index;
    protected Action<int> OnClickElementCallback;
    private Action<int> _OnChangedSelectCharacterNameCallback;

    [SerializeField] private Button _elementButton;
    [SerializeField] private Image _characterIcon;

    private void Awake()
    {
        TryGetComponent(out _elementButton);
        _elementButton.onClick.AddListener(OnClickElement);
    }

    private void OnDestroy()
    {
        _elementButton.onClick.RemoveListener(OnClickElement);
    }

    public async void Setup(int elementIndex, Action<int> clickCallback, Action<int> changeSelectCharacter = null)
    {
        _index = elementIndex;
        OnClickElementCallback = clickCallback;
        _OnChangedSelectCharacterNameCallback = changeSelectCharacter;
        _characterIcon.sprite = await ResourcesManager.Instance.LoadSpriteAsync($"equip_chacacter_icon_{elementIndex}");
    }

    private void OnClickElement()
    {
        OnClickElementCallback?.Invoke(_index);
        _OnChangedSelectCharacterNameCallback?.Invoke(_index);
    }
}
