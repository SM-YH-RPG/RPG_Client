using UnityEngine;

public class GroundedAttackState : AttackState
{
    protected const float MinInputThreshold = 0.01f;

    protected CameraDirector _camera;

    private Vector3 _inputDirectionDuringAttack;

    public override void Initialize(PlayerStateMachine stateMachine)
    {
        base.Initialize(stateMachine);

        _camera = _stateMachine.PlayerCtrl.Camera;
    }

    public override void OnEnter()
    {
        base.OnEnter();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (_currentAnimationHash == -1)
            return;

        if (Time.time - _enteredTime < MIN_ANIMATION_READ_TIME)
            return;


        if (_nextAttackQueued && IsInComboWindow())
        {
            TransitionToNextComboAttack();
            return;
        }

        bool isFinished = _animationCtrl.IsAnimationFinished(_currentAnimationHash);
        if (isFinished)
        {
            _currentComboIndex = 0;
            DetermineMovementState();
        }
    }

    public override void OnInput()
    {
        if (_inputActions.Move.IsPressed())
        {
            var direction = _inputActions.Move.ReadValue<Vector2>();
            SetDirection(direction);
        }
    }

    protected override void AddInputListeners()
    {
        base.AddInputListeners();

        _inputActions.Move.canceled += OnMoveCanceled;
    }

    protected override void RemoveInputListeners()
    {
        base.RemoveInputListeners();

        _inputActions.Move.canceled -= OnMoveCanceled;
    }

    private void OnMoveCanceled(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        SetDirection(Vector3.zero);
    }
    
    protected void UseSkill(ESkillType type)
    {
        var skillCtrl = _stateMachine.PlayerCtrl.SkillCtrl;
        if (skillCtrl.IsPossibleUseSkill(type) == false)
            return;

        float skillCooldownDuration = skillCtrl.GetConfigCooldown(type);
        SkillManager.Instance.NotifySkillUsed(type, skillCooldownDuration);
    }

    private void SetRotationToMoveDirection()
    {
        float inputMagnitude = _inputDirectionDuringAttack.sqrMagnitude;
        const float InputThresholdSq = MinInputThreshold * MinInputThreshold;

        if (inputMagnitude >= InputThresholdSq)
        {
            Vector3 forward = _camera.GetForward();
            Vector3 right = _camera.GetRight();

            Vector3 desiredDirection = (forward * _inputDirectionDuringAttack.y + right * _inputDirectionDuringAttack.x).normalized;

            desiredDirection.y = 0;

            Quaternion targetRotation = Quaternion.LookRotation(desiredDirection, Vector3.up);
            _transform.rotation = targetRotation;
        }
    }

    private void SetDirection(Vector3 direction)
    {
        _inputDirectionDuringAttack = direction;
    }

    protected void TransitionToNextComboAttack()
    {
        SetRotationToMoveDirection();

        _enteredTime = Time.time;

        _currentComboIndex = (_currentComboIndex + 1) % _comboMax;

        _nextAttackQueued = false;

        _currentAnimationHash = GetCurrentAnimationHashInternal();

        _animationCtrl.CrossFade(_currentAnimationHash);
    }

    protected bool IsInComboWindow()
    {
        float normalizedTime = _animationCtrl.GetNormalizedAnimationTime(_currentAnimationHash);
        int index = _currentComboIndex;

        if (index >= 0 && index < _comboMax)
        {
            var currentAttackData = _comboConfigs[index];

            return normalizedTime >= currentAttackData.StartTimeNormalized &&
                   normalizedTime <= currentAttackData.EndTimeNormalized;
        }

        return false;
    }

    protected bool IsAbleToQueueCombo()
    {
        if (_currentComboIndex >= 0 &&
            _currentComboIndex < _comboConfigs.Length)
        {
            float normalizedTime = _animationCtrl.GetNormalizedAnimationTime(_currentAnimationHash);
            var _comboConfig = _comboConfigs[_currentComboIndex];

            return normalizedTime >= _comboConfig.StartTimeNormalized &&
                   normalizedTime <= _comboConfig.EndTimeNormalized;
        }

        return false;
    }

    protected int GetCurrentAnimationHashInternal()
    {
        int index = _currentComboIndex;
        if (index >= 0 && index < _comboMax)
        {
            var currentAttackData = _comboConfigs[index];
            return currentAttackData.AnimationStateHash;
        }

        return -1;
    }
}