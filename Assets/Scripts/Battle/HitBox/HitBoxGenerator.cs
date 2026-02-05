using System;
using System.Collections.Generic;
using UnityEngine;

public struct HitSphereData
{
    public Vector3 Offset;
    public float Radius;
    public LayerMask Layers;
}

public struct HitBoxData
{
    public Vector3 Offset;
    public Vector3 HalfExtend;
    public Quaternion Rotation;
    public LayerMask Layers;
}

public struct HitSectorData
{
    public Vector3 Offset;
    public float Radius;
    public float Angle;
    public LayerMask Layers;
}


public class HitBoxGenerator : IHitBoxGenerator
{
    private const int MaxHits = 10;
    private readonly Collider[] _hitResults = new Collider[MaxHits];

    private Transform _transform;

    public void Initialize(Transform transform)
    {
        _transform = transform;
    }

    public List<IAttackTarget> GenerateHitSector(HitSectorData data)
    {
        Vector3 worldAttackPosition = _transform.TransformPoint(data.Offset);
        int hitCount = Physics.OverlapSphereNonAlloc(
            worldAttackPosition,
            data.Radius,
            _hitResults,
            data.Layers
        );

        return ProcessSectorResults(hitCount, worldAttackPosition, data);
    }

    private List<IAttackTarget> ProcessSectorResults(int hitCount, Vector3 attackOriginWorldPos, HitSectorData data)
    {
        List<IAttackTarget> targetList = new List<IAttackTarget>(hitCount);
        if (hitCount > 0)
        {
            float halfAngle = data.Angle / 2f;
            Vector3 attackDirection = _transform.forward;

            for (int i = 0; i < hitCount; i++)
            {
                Collider enemyCollider = _hitResults[i];
                if (enemyCollider.transform.root == _transform.root)
                    continue;

                Vector3 directionToTarget = (enemyCollider.transform.position - attackOriginWorldPos);;
                if (directionToTarget == Vector3.zero)
                    continue;

                directionToTarget.Normalize();

                float angleToTarget = Vector3.Angle(attackDirection, directionToTarget);
                if (angleToTarget <= halfAngle)
                {
                    if (enemyCollider.TryGetComponent<IAttackTarget>(out var target))
                    {
                        targetList.Add(target);
                    }
                }
            }
        }

        Array.Clear(_hitResults, 0, hitCount);

        return targetList;
    }

    public List<IAttackTarget> ExecuteHitCheck(HitShapeData data)
    {
        Vector3 worldAttackPosition = _transform.TransformPoint(data.Offset);
        int hitCount = 0;

        switch (data.Type)
        {
            case HitShapeType.Sphere:
                hitCount = Physics.OverlapSphereNonAlloc(
                    worldAttackPosition,
                    data.Radius,
                    _hitResults,
                    data.Layers);
                break;

            case HitShapeType.Box:
                Quaternion worldRotation = _transform.rotation * data.Rotation;
                hitCount = Physics.OverlapBoxNonAlloc(
                    worldAttackPosition,
                    data.HalfExtend,
                    _hitResults,
                    worldRotation,
                    data.Layers
                );
                break;

            case HitShapeType.Sector:
                hitCount = Physics.OverlapSphereNonAlloc(
                    worldAttackPosition,
                    data.Radius,
                    _hitResults,
                    data.Layers
                );
                break;

            case HitShapeType.None:
            default:
                Array.Clear(_hitResults, 0, _hitResults.Length);
                return new List<IAttackTarget>();
        }

        return ProcessHitResults(hitCount, worldAttackPosition, data);
    }

    private List<IAttackTarget> ProcessHitResults(int hitCount, Vector3 attackOriginWorldPos, HitShapeData data)
    {
        List<IAttackTarget> targetList = new List<IAttackTarget>(hitCount);
        if (hitCount == 0)
        {
            Array.Clear(_hitResults, 0, _hitResults.Length);
            return targetList;
        }

        for (int i = 0; i < hitCount; i++)
        {
            Collider enemyCollider = _hitResults[i];

            if (enemyCollider == null || enemyCollider.transform.root == _transform.root)
                continue;

            bool withinAngle = true;
            if (data.Type == HitShapeType.Sector)
            {
                Vector3 directionToTarget = (enemyCollider.transform.position - attackOriginWorldPos).normalized;
                if (directionToTarget != Vector3.zero)
                {
                    float halfAngle = (data.Type == HitShapeType.Sector) ? data.Angle / 2f : 180f;
                    Vector3 attackDirection = _transform.forward;

                    float angleToTarget = Vector3.Angle(attackDirection, directionToTarget);
                    if (angleToTarget > halfAngle)
                    {
                        withinAngle = false;
                    }
                }
                else
                {
                    withinAngle = false;
                }
            }

            if (withinAngle && enemyCollider.TryGetComponent<IAttackTarget>(out var target))
            {
                targetList.Add(target);
            }
        }

        Array.Clear(_hitResults, 0, hitCount);

        return targetList;
    }
}