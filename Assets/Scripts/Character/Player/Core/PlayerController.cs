using System;
using UnityEngine;


public class PlayerController : MonoBehaviour, IMoveTarget, IAttackTarget, IAttackableCtrl
{
    #region CONST
    private const float SlOPE_ANGLE_TOLERANCE = 0.1f;
    private const int NEED_DASH_STAMINA = 30;
    private const int CONSUME_RUNNING_STAMINA = 1;

    [SerializeField]
    private float GroundCheckDistance = 0.05f;
    #endregion

    private Transform _transform;

    private MiniMapCamera _minimapCamera;

    private CameraDirector _camera = null;
    public CameraDirector Camera => _camera;

    private PlayerStateMachine _fsm = null;
    public PlayerStateMachine FSM => _fsm;

    private IInteractionService _interactionService => PlayerManager.Instance.InteractionService;
    private IPartyService _partyService => PlayerManager.Instance.PartyService;
    private ICharacterService _characterService => PlayerManager.Instance.CharacterService;

    #region Inspector
    [SerializeField]
    private CharacterController _characterCtrl = null;
    public CharacterController CharacterCtrl => _characterCtrl;

    [SerializeField]
    private InputController _inputCtrl = null;

    [SerializeField]
    private AnimationController _animationCtrl = null;

    [SerializeField]
    private CombatController _combatCtrl = null;
    public CombatController CombatCtrl => _combatCtrl;

    [SerializeField]
    private SkillController _skillCtrl = null;
    public SkillController SkillCtrl => _skillCtrl;

    [SerializeField]
    private InteractionDetector _interactionDetector = null;
    public InteractionDetector InteractionDetector => _interactionDetector;

    [SerializeField]
    private InteractionHandler _interactionHandler = null;

    [SerializeField]
    private CharacterConfig _characterData;
    public CharacterConfig CharacterData => _characterData;


    [SerializeField]
    private float _maxSlopeAngle = 45f;
    
    [SerializeField]
    private float _slopeSlideSpeed = 8f;

    [SerializeField]
    private GameObject[] _attackEffectPrefabArray;

    [SerializeField]
    private GameObject[] _skillEffectPrefabArray;

    [SerializeField]
    private Vector3 _uiSpawnPosition;
    #endregion

    private RuntimeCharacter _currentCharacter;
    public RuntimeCharacter CurrentCharacter => _currentCharacter;

    private IDamageCalculator _calculator;

    private Vector3 _groundNormal;
    public Vector3 GroundNormal => _groundNormal;

    private Vector3 _characterVelocity = Vector3.zero;
    public Vector3 CharacterVelocity  { get { return _characterVelocity; } set { _characterVelocity = value; } }

    public bool IsGrounded => _characterCtrl.isGrounded;

    private Vector3 _moveDirection = Vector3.zero;
    public Vector3 MoveDirection => _moveDirection;

    private bool _Invulnerability = false;

    [Header("Gravity")]
    [SerializeField]
    private float _gravity = -15f;
    
    [SerializeField]
    private float _groundedOffset = -0.14f;        

    [Header("Falling Damage")]
    [SerializeField]
    private float _minFallSpeedForDamage = -10f;
    
    [SerializeField]
    private float _damageMultiplier = 5f;

    private float _maxFallVelocityY = 0f;

    private int _hitsReceived = 0;
    private int _hitsToAirborne = 3;

    //private IMovement _movement;

    private void Awake()
    {
        _transform = transform;
        //_movement = new PlayerMovement();
        _calculator = new PlayerDamageCalculator(this);

        _interactionService.OnInteractionTargetChanged += HandleInteractionTargetChanged;
        _characterService.OnUpdateCharacterStatData += UpdateCurrentCharacterData;
        CharacterLevelManager.Instance.OnUpdateLevelUpCharacterCurrentHp += UpdateCharacterHp;
        _characterService.OnUpdateCharacterCurrentHp += UpdateCharacterHp;
    }

    private void OnDestroy()
    {
        _interactionService.OnInteractionTargetChanged -= HandleInteractionTargetChanged;
        _characterService.OnUpdateCharacterStatData -= UpdateCurrentCharacterData;
        CharacterLevelManager.Instance.OnUpdateLevelUpCharacterCurrentHp -= UpdateCharacterHp;
        _characterService.OnUpdateCharacterCurrentHp -= UpdateCharacterHp;
    }

    public void Init(CameraDirector camera, MiniMapCamera minimapCamera, Vector3 position)
    {
        _camera = camera;
        _minimapCamera = minimapCamera;

        _camera?.SetTarget(this);
        _minimapCamera?.SetTarget(this);

        _fsm = new PlayerStateMachine(this);
        _fsm.ChangeState(EPlayerStateType.Idle);

        _interactionHandler.Init(FindAnyObjectByType<InteractionUI>());

        Warp(position);

        SetCurrentCharacter();
    }

    public void Rebind(CameraDirector camera, MiniMapCamera minimapCamera)
    {
        _camera = camera;
        _minimapCamera = minimapCamera;

        _camera?.SetTarget(this);
        _minimapCamera?.SetTarget(this);

        _moveDirection = Vector3.zero;

        if(_fsm == null)
        {
            _fsm = new PlayerStateMachine(this);            
        }
        _fsm.ChangeState(EPlayerStateType.Idle);
        _interactionHandler.Init(FindAnyObjectByType<InteractionUI>());
        _interactionService.NotifyInteractionSourceChanged(transform);
        SetCurrentCharacter();
    }

    private void SetCurrentCharacter()
    {
        if (_partyService.TryGetSelectedCharacter(out RuntimeCharacter runtimeStats))
        {
            _currentCharacter = runtimeStats;
        }
        else
        {
            long instanceId = _characterService.GetCharacterUniqueIndex(_characterData.Index);
            _currentCharacter = _characterService.GetRunTimeCharacterBy(instanceId);
        }
    }

    private void UpdateCharacterHp(int characterIndex, int updateHp)
    {
        if (characterIndex == CharacterData.Index)
        {
            _currentCharacter.CurrentHP = updateHp;
        }
    }

    private void UpdateCurrentCharacterData(RuntimeCharacter character)
    {
        if (character.TemplateId == _characterData.Index)
        {
            _currentCharacter = character;
        }
    }

    private void Update()
    {
        if (_fsm == null || _isNPCInteraction)
            return;

        HandleCharacterPhysics();        

        _fsm.Input();
        _fsm.Update();        
    }

    private void FixedUpdate()
    {
        _fsm?.PhysicsUpdate();
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        float angle = Vector3.Angle(Vector3.up, hit.normal);
        if (angle <= _maxSlopeAngle + SlOPE_ANGLE_TOLERANCE)
        {
            _groundNormal = hit.normal;
        }
        else
        {
            _groundNormal = Vector3.up;
        }
    }

    public void Warp(Vector3 position)
    {
        _characterCtrl.enabled = false;
        _transform.position = position;
        _characterCtrl.enabled = true;
    }

    public void SetDirection(Vector3 _direction)
    {
        if (_direction == Vector3.zero)
        {
            _fsm.ChangeState(EPlayerStateType.Idle);            
        }
        else
        {
            ChangeJoyPadMoveState();            
        }
        _moveDirection = _direction;
    }

    private void ChangeJoyPadMoveState()
    {
        if (_fsm.IsWalkingMode)
        {
            _fsm.ChangeState(EPlayerStateType.Walk);
        }
        else
        {
            _fsm.ChangeState(EPlayerStateType.Run);
        }
    }

    public void OnDamaged(CombatData combatData)
    {
        if(_Invulnerability)
        {
            return;
        }

        float decreaseDamange = combatData.damage * (_currentCharacter.Defence * 0.01f);
        int finalDamage = Mathf.Max(1, combatData.damage - Mathf.RoundToInt(decreaseDamange));
        combatData.damage = finalDamage;

        _currentCharacter.CurrentHP -= combatData.damage;

        PlayerManager.Instance.CharacterService.UpdateCurrentCharacterHp(_currentCharacter.CurrentHP, _currentCharacter.InstanceId);
        _hitsReceived++;

        if (_hitsReceived >= _hitsToAirborne)
        {
            _hitsReceived = 0;
            if (_currentCharacter.CurrentHP <= 0)
            {
                _fsm.ChangeState(EPlayerStateType.Death);
            }
            else
            {
                _fsm.ChangeState(EPlayerStateType.Airborne);

                UIPoolingObjectSpawnManager.Instance.ShowDamageText(transform, _uiSpawnPosition, combatData, Color.red);
            }
        }
        else
        {
            if (_currentCharacter.CurrentHP <= 0)
            {
                _fsm.ChangeState(EPlayerStateType.Death);
            }
            else
            {
                _fsm.ChangeState(EPlayerStateType.Hit);

                UIPoolingObjectSpawnManager.Instance.ShowDamageText(transform, _uiSpawnPosition, combatData, Color.red);
            }
        }
    }

    #region AnimationEventFunc
    public void AnimationEvent_CharacteerDeathSwap()
    {
        gameObject.SetActive(false);
        var party = PlayerManager.Instance.PartyService.CurrentParty;
        bool isSwapped = false;
        for (int i = 0; i < party.Characters.Length; i++)
        {
            if (PlayerManager.Instance.CharacterService.GetRunTimeCharacterBy(party.Characters[i]).CurrentHP > 0)
            {
                PlayerManager.Instance.PartyService.RequestCharacterSwap(i);
                isSwapped = true;
                break;
            }
        }
        if (isSwapped == false)
        {
            _ = UIManager.Instance.Show<RevivePopup>();
        }        
    }

    public void AnimationEvent_CreateAttackEffect(int index)
    {
        GameObject presetPrefab = _attackEffectPrefabArray[index];
        EffectManager.Instance.SpawnEffect(presetPrefab, transform.position + Vector3.up, transform.rotation);        
    }

    public void AnimationEvent_CreateSkillEffect(int index)
    {

        GameObject presetPrefab = _skillEffectPrefabArray[index];
        EffectManager.Instance.SpawnEffect(presetPrefab, transform.position + Vector3.up, transform.rotation);        
    }
    #endregion

    public void SetInvulnerability(bool Invulnerability)
    {
        _Invulnerability = Invulnerability;
    }

    public InputController GetInput()
    {
        return _inputCtrl;
    }

    public AnimationController GetAnimator()
    {
        return _animationCtrl;
    }

    private void HandleCharacterPhysics()
    {
        if (IsGrounded)
        {
            if(_characterVelocity.y < 0f)
            {
                CalculateFallDamage();
                _characterVelocity.y = _groundedOffset;

                _maxFallVelocityY = 0f;
            }
        }
        else
        {
            _characterVelocity.y += _gravity * Time.deltaTime;
            if (_characterVelocity.y < _maxFallVelocityY)
            {
                _maxFallVelocityY = _characterVelocity.y;
            }
        }
        
        _characterCtrl.Move(_characterVelocity * Time.deltaTime);
    }
    
    private void CalculateFallDamage()
    {
        if (_maxFallVelocityY < _minFallSpeedForDamage)
        {
            float fallSpeedDifference = _minFallSpeedForDamage - _maxFallVelocityY;
            int damage = Mathf.RoundToInt(fallSpeedDifference * _damageMultiplier);            

            if (damage > 0)
            {
                CombatData combatData = new CombatData();
                combatData.damage = damage;
                combatData.isCritical = false;

                OnDamaged(combatData);
            }
        }
    }

    public bool CheckDashStaminaCostAndReduction()
    {
        if (PlayerManager.Instance.CurrentStamina < NEED_DASH_STAMINA)
        {
            return false;
        }
        int newStamina = PlayerManager.Instance.CurrentStamina - NEED_DASH_STAMINA;
        PlayerManager.Instance.UpdatePlayerStamina(newStamina);        
        return true;
    }

    public bool CheckRunStaminaCostAndReduction()
    {
        if (PlayerManager.Instance.CurrentStamina <= 0)
        {
            _fsm.ChangeState(EPlayerStateType.Walk);
            return false;
        }
        int newStamina = PlayerManager.Instance.CurrentStamina - CONSUME_RUNNING_STAMINA;
        PlayerManager.Instance.UpdatePlayerStamina(newStamina);        
        return true;
    }

    public void RecoveryStamina(int _stamina)
    {
        int newStamina = Mathf.Min(PlayerManager.Instance.MaxStamina, PlayerManager.Instance.CurrentStamina + _stamina);
        PlayerManager.Instance.UpdatePlayerStamina(newStamina);        
    }

    public void SetActivate(bool activate)
    {
        gameObject.SetActive(activate);
    }

    private void HandleInteractionTargetChanged(Transform target)
    {
        _isNPCInteraction = target != null;
        UpdateLookAtTarget(target);
    }

    private bool _isNPCInteraction = false;
    private void UpdateLookAtTarget(Transform targetNPC)
    {
        if (targetNPC == null)
            return;

        Vector3 directionToTarget = targetNPC.position - transform.position;
        directionToTarget.y = 0;
        if (directionToTarget != Vector3.zero)
        {
            var targetRotation = Quaternion.LookRotation(directionToTarget);
            transform.rotation = targetRotation;
        }
    }

    #region Debug
    private void OnDrawGizmos()
    {
        Vector3 spherePosition = new Vector3(_characterCtrl.transform.position.x,
                                             _characterCtrl.bounds.min.y + GroundCheckDistance,
                                             _characterCtrl.transform.position.z);

        Gizmos.color = IsGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(spherePosition, _characterCtrl.radius);
    }

    public IAttackTypeSate GetAttackTypeState()
    {
        if(FSM == null || FSM.GetCurrentState() is not IAttackTypeSate attackTypeState)
            return null;

        return attackTypeState;
    }

    public AttackConfig GetAttackConfig()
    {
        AttackConfig configToUse = null;

        var currentState = FSM.GetCurrentState();
        if (currentState is WeakAttackState weakState)
        {
            var configs = CharacterData.AttackGroup.WeakAttack;

            int comboIndex = weakState.CurrentComboIndex;
            if (comboIndex >= 0 && comboIndex < configs.Length)
            {
                configToUse = configs[comboIndex];
            }
        }
        else if (currentState is StrongAttackState strongState)
        {
            var configs = CharacterData.AttackGroup.StrongAttack;
            if (configs.Length > 0)
            {
                configToUse = configs[0];
            }
        }
        else if (currentState is GroundedSkillState skill)
        {
            configToUse = CharacterData.SkillGroup.Skill;
        }
        else if (currentState is UltimateSkillState ultimateSkill)
        {
            configToUse = CharacterData.SkillGroup.UltimateSkill;
        }

        return configToUse;
    }

    public IDamageCalculator GetCalculator()
    {
        return _calculator;
    }
    #endregion
}
