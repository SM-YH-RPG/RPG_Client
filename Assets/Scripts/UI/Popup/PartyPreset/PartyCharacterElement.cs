using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PartyCharacterElement : MonoBehaviour
{
    #region Inspector
    [SerializeField]
    private Toggle _characterToggle;

    [SerializeField]
    private Image _characterIcon;

    [SerializeField]
    private Image _selectLine;

    [SerializeField]
    private GameObject _partyIndexObject;

    [SerializeField]
    private TextMeshProUGUI _partyIndexText;

    [SerializeField]
    private TextMeshProUGUI _levelText;
    #endregion

    private RuntimeCharacter _runTimeCharacter;
    CharacterConfig characterConfig;
    //private CharacterData _characterData;    

    private TextMeshProUGUI _nameText;

    #region Action
    private Action<RuntimeCharacter> _selectCharacter;
    #endregion

    private void Awake()
    {
        _characterToggle.onValueChanged.AddListener(OnClickCharacterButton);
    }

    public async void updateCharacterData(int preset, int indexInParty, RuntimeCharacter character, TextMeshProUGUI name, ToggleGroup group, Action<RuntimeCharacter> callback)
    {
        _runTimeCharacter = character;
        _characterToggle.group = group;
        characterConfig = InGameManager.Instance.GetPlayerController(_runTimeCharacter.TemplateId).CharacterData;
        _nameText = name;
        _selectCharacter = callback;

        _partyIndexObject.SetActive(false);
        if ( indexInParty >= 0)
        {
            _partyIndexObject.SetActive(true);
            _partyIndexText.text = $"{indexInParty + 1}";
        }        
            
        _levelText.text = $"Lv.{character.Level}";
        _characterIcon.sprite = await AddressableManager.Instance.LoadAssetAsync<Sprite>($"party_characterList_icon_{character.TemplateId}");
    }

    public void OnClickCharacterButton(bool isOn)
    {
        _selectLine.enabled = isOn;
        _characterToggle.isOn = isOn;
        if (isOn)
        {
            _nameText.text = characterConfig.Name;
            _selectCharacter?.Invoke(_runTimeCharacter);
        }
    }
}
