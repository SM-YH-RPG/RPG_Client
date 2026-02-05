using UnityEngine;
using UnityEngine.EventSystems;
using static InputActionSystem;

public class MobileUIAttackButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private float holdThresholdSeconds = 0.35f;

    private bool _isPressed;
    private float _pressedTime;
    private bool _isStrongAttack;

    private PlayerStateMachine _currentStateMachine;    

    private void Awake()
    {
        
    }

    private void Start()
    {
        PlayerManager.Instance.PartyService.OnPartyCharacterSwapped += SetPlayingRunTimeCharacterFSM;
        SetPlayingRunTimeCharacterFSM(PartyManager.Instance.GetCurrentCharacterInActiveParty());
    }

    private void Update()
    {
        if (!_isPressed) return;

        _pressedTime += Time.deltaTime;

        if (!_isStrongAttack && _pressedTime >= holdThresholdSeconds)
        {
            _isStrongAttack = true;
            StrongAttackInvoke();
        }
    }

    private void OnDestroy()
    {
        PlayerManager.Instance.PartyService.OnPartyCharacterSwapped -= SetPlayingRunTimeCharacterFSM;
    }

    private void SetPlayingRunTimeCharacterFSM(RuntimeCharacter character)
    {
        _currentStateMachine = InGameManager.Instance.GetPlayerController(character.TemplateId).FSM;        
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _isPressed = true;
        _pressedTime = 0f;
        _isStrongAttack = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!_isPressed) return;

        _isPressed = false;

        if (_isStrongAttack) return;

        // 공격 발동
        WeekAttackInvoke();
    }

    private void WeekAttackInvoke()
    {
        if (_currentStateMachine != null && _currentStateMachine.PlayerCtrl.GetInput().enabled)
        {            
            _currentStateMachine.ChangeState(EPlayerStateType.WeakAttack);            
        }
    }

    private void StrongAttackInvoke()
    {
        if (_currentStateMachine != null && _currentStateMachine.PlayerCtrl.GetInput().enabled)
        {
            _currentStateMachine.ChangeState(EPlayerStateType.StrongAttack);
        }        
    }    
}
