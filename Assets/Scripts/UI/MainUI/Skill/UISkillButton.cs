using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISkillButton : MonoBehaviour
{
    private enum EActionButtonType
    {
        None,
        Skill,
        Jump,
        Dash,
        ChangeMoveState,
        WeakAttack,
        End
    }


    public ESkillType SkillType;

    [SerializeField]
    private EActionButtonType _actionType;

    [SerializeField]
    private Button _button;

    [SerializeField]
    private Image _skillIcon;

    [SerializeField]
    private Image _cooldownOverlay;

    [SerializeField]
    private TextMeshProUGUI _cooldownText;

    private float _maxCooldown = 0f;

    private PlayerStateMachine _currentStateMachine;

    private void Awake()
    {
        TryGetComponent(out _button);

#if UNITY_ANDROID || UNITY_IOS
        _button.enabled = true;
        _button.onClick.AddListener(OnClickButton);
        PlayerManager.Instance.PartyService.OnPartyCharacterSwapped += SetPlayingRunTimeCharacterFSM;
#else
        _button.enabled = false;
#endif

        if (_cooldownOverlay != null)
            _cooldownOverlay.fillAmount = 0;

        if (_cooldownText != null)
            _cooldownText.gameObject.SetActive(false);        
    }

    private void Start()
    {
        SetPlayingRunTimeCharacterFSM(PartyManager.Instance.GetCurrentCharacterInActiveParty());
    }

    private void OnDestroy()
    {
#if UNITY_ANDROID || UNITY_IOS
        PlayerManager.Instance.PartyService.OnPartyCharacterSwapped -= SetPlayingRunTimeCharacterFSM;
#endif
    }

    private void SetPlayingRunTimeCharacterFSM(RuntimeCharacter character)
    {
        _currentStateMachine = InGameManager.Instance.GetPlayerController(character.TemplateId).FSM;
    }

#if UNITY_ANDROID || UNITY_IOS
    private void OnClickButton()
    {
        bool isPossibleChangeState = _currentStateMachine != null && _currentStateMachine.PlayerCtrl.GetInput().enabled;
        if (_actionType == EActionButtonType.Skill)
        {
            switch (SkillType)
            {
                case ESkillType.Skill:
                    if (isPossibleChangeState)
                    {
                        if (_currentStateMachine.PlayerCtrl.SkillCtrl.IsPossibleUseSkill(ESkillType.Skill))
                        {
                            _currentStateMachine.ChangeState(EPlayerStateType.GroundedSkill);
                        }
                    }
                    break;
                case ESkillType.SpecialSkill:
                    break;
                case ESkillType.UltimateSkill:
                    if (isPossibleChangeState)
                    {
                        if (_currentStateMachine.PlayerCtrl.SkillCtrl.IsPossibleUseSkill(ESkillType.UltimateSkill))
                        {
                            _currentStateMachine.ChangeState(EPlayerStateType.UltimateSkill);
                        }
                    }
                    break;
            }
        }
        else
        {
            switch (_actionType)
            {                
                case EActionButtonType.Jump:
                    if (isPossibleChangeState)
                        _currentStateMachine.ChangeState(EPlayerStateType.Jump);
                    break;
                case EActionButtonType.Dash:
                    if (isPossibleChangeState)
                        _currentStateMachine.ChangeState(EPlayerStateType.Dash);
                    break;
                case EActionButtonType.ChangeMoveState:
                    if (isPossibleChangeState)
                        _currentStateMachine.IsWalkingMode = !_currentStateMachine.IsWalkingMode;
                    break;
            }
        }
    }
#endif

    public void StartCooldownVisual(float maxCooltime)
    {
        _maxCooldown = maxCooltime;

        if (_cooldownText != null)
            _cooldownText.gameObject.SetActive(true);

        _button.interactable = false;

        UpdateCooldownUI(maxCooltime);
    }

    public void UpdateCooldownUI(float currentCooldownValue)
    {
        if (currentCooldownValue <= 0)
        {
            currentCooldownValue = 0;
            if (_cooldownOverlay != null)
                _cooldownOverlay.fillAmount = 0;

            if (_cooldownText != null)
                _cooldownText.gameObject.SetActive(false);

            _button.interactable = true;
        }
        else
        {
            if (_cooldownOverlay != null)
            {
                _cooldownOverlay.fillAmount = currentCooldownValue / _maxCooldown;
            }

            if (_cooldownText != null)
            {
                _cooldownText.gameObject.SetActive(true);
                _cooldownText.text = Mathf.Ceil(currentCooldownValue).ToString("0");
            }
        }
    }

    public void SetSkillIcon(Sprite skillIcon)
    {
        _skillIcon.sprite = skillIcon;
    }

    public bool IsWeakAttackButtonType()
    {
        return _actionType == EActionButtonType.WeakAttack;
    }
}
