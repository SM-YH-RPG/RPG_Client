using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RevivePopup : BasePopup
{
    private const int REVIVE_CHARACTER_HP = 50;

    [SerializeField] private TextMeshProUGUI _reviveText;
    [SerializeField] private TextMeshProUGUI _gameOverText;
    [SerializeField] private Button _confirmButton;

    protected override void Awake()
    {
        base.Awake();
        _confirmButton.onClick.AddListener(OnClickConfirmButton);
    }

    private void OnClickConfirmButton()
    {        
        var party = PlayerManager.Instance.PartyService.CurrentParty;
        for (int i = 0; i < party.Characters.Length; i++)
        {
            if (party.Characters[i] != Party.EMPTY_MEMBER_INDEX) // 파티 빈 슬릇이 아니라면
            {
                var character = PlayerManager.Instance.CharacterService.GetRunTimeCharacterBy(party.Characters[i]);
                character.CurrentHP = REVIVE_CHARACTER_HP; // 살려는 주되 포션으로 채우게 하자
                PlayerManager.Instance.CharacterService.UpdateCurrentCharacterHp(REVIVE_CHARACTER_HP, party.Characters[i]);
                if (InGameManager.Instance.GetPlayerController(character.TemplateId).FSM != null)
                {
                    InGameManager.Instance.GetPlayerController(character.TemplateId).FSM.ChangeState(EPlayerStateType.Idle);
                }
            }
        }
        PlayerManager.Instance.PartyService.ReviveCharacterSwap(PlayerManager.Instance.PartyService.SelectedIndexInParty);
        UIManager.Instance.Hide();
    }
}
