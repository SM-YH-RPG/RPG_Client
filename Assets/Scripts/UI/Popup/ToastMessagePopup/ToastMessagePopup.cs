using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class ToastMessagePopup : BasePopup
{
    [SerializeField] private RectTransform _toastRectTransform;
    [SerializeField] private Image _toastBg;
    [SerializeField] private TextMeshProUGUI _messageText;
    [SerializeField] private float _startYOffset;
    [SerializeField] private float _endYOffset;

    private Action _onToastEndAction;

    private float _timer;
    private float _fadeInTime = 0.2f;
    private float _holdTime = 0.6f;
    private float _fadeOutTime = 0.2f;

    private Vector3 _startPos;
    private Vector3 _endPos;

    private bool _isPlay = false;

    protected override void Awake()
    {
        base.Awake();
    }

    public void PlayToast(string message, Action toastEndAction = null)
    {
        _messageText.text = message;
        _onToastEndAction = toastEndAction;

        _timer = 0f;
        _startPos = new Vector3(0f, _startYOffset, 0f);
        _endPos = new Vector3(0f, _endYOffset, 0f);

        _toastRectTransform.anchoredPosition = _startPos;
        SetFadeAlpha(0f);
        gameObject.SetActive(true);
        _isPlay = true;
    }

    private void Update()
    {
        if (_isPlay)
        {
            _timer += Time.deltaTime;

            if (_timer <= _fadeInTime)
            {
                float fadeInProgress = _timer / _fadeInTime;
                SetFadeAlpha(fadeInProgress);
                _toastRectTransform.anchoredPosition = Vector3.Lerp(_startPos, Vector3.zero, fadeInProgress);
            }
            else if (_timer <= _fadeInTime + _holdTime)
            {
                SetFadeAlpha(1f);
                _toastRectTransform.anchoredPosition = Vector3.zero;
            }
            else if (_timer <= _fadeInTime + _holdTime + _fadeOutTime)
            {
                float fadeOutProgress = (_timer - _fadeInTime - _holdTime) / _fadeOutTime;
                SetFadeAlpha(1f - fadeOutProgress);
                _toastRectTransform.anchoredPosition = Vector3.Lerp(Vector3.zero, _endPos, fadeOutProgress);
            }

            if (_timer > _fadeInTime + _holdTime + _fadeOutTime)
            {
                _timer = 0f;
                _isPlay = false;
                ResetToast();
                _onToastEndAction?.Invoke();
                UIManager.Instance.Hide();
            }
        }
    }

    private void SetFadeAlpha(float alpha)
    {
        _toastBg.color = new Color(1f, 1f, 1f, alpha);
        _messageText.color = new Color(1f, 1f, 1f, alpha);
    }

    private void ResetToast()
    {
        _toastBg.color = Color.white;
        _messageText.color = Color.white;
        _messageText.text = string.Empty;
    }
}
