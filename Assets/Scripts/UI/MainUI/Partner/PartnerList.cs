using UnityEngine;

public class PartnerList : MonoBehaviour
{
    [SerializeField]
    private PartnerInfoElement[] _elements;

    private IPartyService _partyService = PlayerManager.Instance.PartyService;

    private void Start()
    {
        _partyService.OnActivePartyChanged += HandlePartyMemberListChanged;        

        HandlePartyMemberListChanged(_partyService.CurrentParty);
    }

    private void OnDestroy()
    {
        if (_partyService != null)
        {
            _partyService.OnActivePartyChanged -= HandlePartyMemberListChanged;            
        }
    }

    public void HandlePartyMemberListChanged(Party party)
    {
        for (int i = 0; i < _elements.Length; i++)
        {
            if (i >= party.Characters.Length)
            {
                _elements[i].gameObject.SetActive(false);
                continue;
            }

            RuntimeCharacter characterInSlot = PlayerManager.Instance.CharacterService.GetRunTimeCharacterBy(party.Characters[i]);

            bool slotIsActive = characterInSlot.TemplateId != -1;
            _elements[i].gameObject.SetActive(slotIsActive);

            if (slotIsActive)
            {
                _elements[i].InitPartyMember(i, characterInSlot);
            }
        }
    }
}
