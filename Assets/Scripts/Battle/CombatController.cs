using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public interface IDamageCalculator
{
    CombatData GetCombatData();
}

public interface IAttackableCtrl
{
    AttackConfig GetAttackConfig();

    IAttackTypeSate GetAttackTypeState();
    IDamageCalculator GetCalculator();
}

public class CombatController : MonoBehaviour
{
    private IAttackableCtrl _attacker;
    private IDamageCalculator _damageCalculator;

    private IHitBoxGenerator _hitBoxGenerator;
    private HitBoxGizmoDrawer _gizmoDrawer;

    private CancellationTokenSource _cancellationTokenSource;

    [SerializeField]
    private bool _debugDrawHitbox = false;

    private void Awake()
    {
        TryGetComponent(out _attacker);

        IHitBoxGenerator hitBoxGenerator = new HitBoxGenerator();

        TryGetComponent(out _gizmoDrawer);

        _damageCalculator = _attacker.GetCalculator();

        if (_debugDrawHitbox && _gizmoDrawer != null)
        {
            _hitBoxGenerator = new HitBoxDebugDecorator(hitBoxGenerator, _gizmoDrawer);
        }
        else
        {
            _hitBoxGenerator = hitBoxGenerator;
        }

        _hitBoxGenerator.Initialize(transform);
        _cancellationTokenSource = new CancellationTokenSource();
    }
    
    private void OnDestroy()
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
        _cancellationTokenSource = null;
    }

    private AttackConfig GetCurrentAttackConfig()
    {
        return _attacker.GetAttackConfig();
    }

    private async UniTaskVoid ActivateHitboxesWithDelayAsync(TimedHitData[] hitDataList, CancellationToken token)
    {
        foreach (var timedData in hitDataList)
        {
            try
            {
                await UniTask.WaitForSeconds(timedData.Delay, cancellationToken: token);
            }
            catch (OperationCanceledException ex)
            {
                Debug.Log($"exception : {ex}");
                return;
            }

            if (this != null && this.isActiveAndEnabled)
            {
                ComputeDamage(timedData.Config);
            }
        }
    }

    private void ComputeDamage(HitShapeData hitData)
    {
        List<IAttackTarget> targetList = _hitBoxGenerator.ExecuteHitCheck(hitData);
        foreach (var target in targetList)
        {
            ApplyDamageTo(target);
        }
    }

    public void DisableAllHitboxes()
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();

        _cancellationTokenSource = new CancellationTokenSource();

        if (_debugDrawHitbox && _gizmoDrawer != null)
        {
            //_gizmoDrawer.ClearGizmos();
        }
    }

    public void AnimationEvent_EnableHitbox()
    {
        DisableAllHitboxes();

        AttackConfig configToUse = GetCurrentAttackConfig();
        if(configToUse != null)
        {
            if (this == null || _cancellationTokenSource == null)
                return;

            CancellationToken newToken = _cancellationTokenSource.Token;
            ActivateHitboxesWithDelayAsync(configToUse.TimedHitData, newToken).Forget();
        }
    }

    private void ApplyDamageTo(IAttackTarget target)
    {
        CombatData combatData = _damageCalculator.GetCombatData();
        target.OnDamaged(combatData);
    }
}