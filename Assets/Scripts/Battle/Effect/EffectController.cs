using UnityEngine;

public class EffectController : BaseEffect
{
    [SerializeField] private ParticleSystem _particle;

    private GameObject _presetPrefab;

    public void InitEffect(GameObject preset)
    {
        _presetPrefab = preset;
        _particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        _particle.Clear(true);
        _particle.Play(true);
    }

    private void Update()
    {
        if (!_particle.IsAlive(true))
        {
            _particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            ObjectPoolManager.Instance.ReturnToPool(_presetPrefab, this);
        }
    }
}
