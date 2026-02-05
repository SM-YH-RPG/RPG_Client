using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmPopup : BasePopup
{
    [SerializeField]
    private TextMeshProUGUI titleText;

    [SerializeField]
    private TextMeshProUGUI contentText;

    [SerializeField]
    private Button noButton;

    [SerializeField]
    private Button yesButton;

    private Action onYesButtonCallback;

    private Action onNoButtonCallback;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        noButton.onClick.AddListener(OnClickNoButton);
        yesButton.onClick.AddListener(OnClickYesButton);
    }

    public void SetContent(string title, string content, Action yesCallback, Action noCallback = null)
    {
        titleText.text = title;
        contentText.text = content;

        onYesButtonCallback = yesCallback;
        onNoButtonCallback = noCallback;
    }

    private void OnClickYesButton()
    {
        onYesButtonCallback?.Invoke();

        UIManager.Instance.Hide();
    }

    private void OnClickNoButton()
    {
        onNoButtonCallback?.Invoke();

        UIManager.Instance.Hide();
    }
}