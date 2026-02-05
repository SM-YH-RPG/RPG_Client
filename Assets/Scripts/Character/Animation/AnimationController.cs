using UnityEngine;


public class AnimationController : MonoBehaviour
{
    private const float DEFAULT_FADE_TIME = 0.1f;
    private const float ANIMATION_END_THRESHOLD = 0.98f;

    [SerializeField]
    private Animator _animator = null;

    private void Awake()
    {
        TryGetComponent(out _animator);
    }

    public void CrossFade(int animationHash, float fadeTime = DEFAULT_FADE_TIME)
    {
        _animator.CrossFade(animationHash, fadeTime);
    }

    public void PlayAnimation(int animationHash)
    {
        _animator?.SetBool(animationHash, true);
    }

    public void StopAnimation(int animationHash)
    {
        _animator?.SetBool(animationHash, false);
    }

    public bool IsPlaying(int animationHash)
    {
        return _animator == null || _animator.GetBool(animationHash);
    }

    public bool IsAnimationFinished(int animationHash)
    {
        if (_animator == null)
            return true;

        if (_animator.IsInTransition(0))
        {
            return false;
        }

        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.shortNameHash == animationHash)
        {
            return stateInfo.normalizedTime >= ANIMATION_END_THRESHOLD;
        }

        return true;
    }

    public float GetNormalizedAnimationTime(int animationHash)
    {
        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.shortNameHash == animationHash)
        {
            return stateInfo.normalizedTime;
        }

        return 0f;
    }

    public float GetCurrentAnimationTime()
    {
        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.normalizedTime;
    }
}