using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusController : MonoBehaviour
{
    #region Const
    private const float FillPerSecond = 1.5f;
    private const float Epsilon = 0.0005f;
    private const float STAMINA_REGEN_PER_SECOND = 50f; // 초당 Stamina 회복량
    #endregion

    #region Inspector
    [SerializeField]
    private Image _hpBar;
    
    [SerializeField]
    private Image _staminaBar;
    
    [SerializeField]
    private Image _staminaBarBg;

    [SerializeField]
    private Image _expBar;

    [SerializeField]
    private TextMeshProUGUI _hpText;

    [SerializeField]
    private float _staminaFadeOutDuration = 1f;
    #endregion

    private float _hpTargetFill = 1f;
    private float _staminaTargetFill = 1f;    
    private float _staminaInvisibleDelayTime = 0f;
    private float _staminaFadeTime;
    private float _recoveryTimer = 0f;

    private bool _isHpAnimating = false;
    private bool _isStaminaAnimating = false;    

    private InGameManager _gameManager;
    private PlayerManager _playerManager;

    private PlayerController _activePlayerController;    

    private IPartyService _partyService => PlayerManager.Instance.PartyService;

    private void Awake()
    {
        _gameManager = InGameManager.Instance;
        _playerManager = PlayerManager.Instance;
        
        _playerManager.CharacterService.OnCharacterStatUpdated += HandleCharacterStatUpdated;
        _playerManager.OnPlayerStaminaUpdated += HandleStaminaChanged;
        _gameManager.OnActivePlayerControllerChanged += HandlePlayerControllerChanged;
        CharacterLevelManager.Instance.OnUpdateExp += HandleEXPGaugeUpdate;

        SetStaminaGaugeVisible(false);
    }

    private void Start()
    {
        int selectedIndexInParty = _partyService.SelectedIndexInParty;

        var initialCharacter = _partyService.CurrentParty.Characters[selectedIndexInParty];
        HandleCharacterStatUpdated(selectedIndexInParty, _playerManager.CharacterService.GetRunTimeCharacterBy(initialCharacter));        
        _playerManager.UpdatePlayerStamina(_playerManager.MaxStamina);
    }

    private void OnDestroy()
    {
        _playerManager.CharacterService.OnCharacterStatUpdated -= HandleCharacterStatUpdated;
        _playerManager.OnPlayerStaminaUpdated -= HandleStaminaChanged;
        _gameManager.OnActivePlayerControllerChanged -= HandlePlayerControllerChanged;
        CharacterLevelManager.Instance.OnUpdateExp -= HandleEXPGaugeUpdate;
    }

    private void Update()
    {
        UpdateHpGauge();
        UpdateStaminaGauge();

        HandleStaminaRecoveryCheck();
    }

    private void SetStaminaGaugeFadeOutAlpha(float alpha)
    {
        _staminaBar.color = new Color(1f, 0.92f, 0.016f, alpha);
        _staminaBarBg.color = new Color(1f, 1f, 1f, alpha);
    }

    private void SetStaminaGaugeVisible(bool visible)
    {
        _staminaBar.enabled = visible;
        _staminaBarBg.enabled = visible;
    }

    private void ResetStaminaGaugeColor()
    {
        _staminaBar.color = Color.yellow;
        _staminaBarBg.color = Color.white;
    }

    private void HandleCharacterStatUpdated(long characterIndexInParty, RuntimeCharacter character)
    {
        // Check if the updated character is the currently selected one        
        if (characterIndexInParty == _partyService.SelectedIndexInParty)
        {            
            HpChanged(character.CurrentHP, character.MaxHp);
        }
    }

    private void HandleStaminaChanged(int currentStamina, int maxStamina)
    {
        if (currentStamina < maxStamina)
        {
            SetStaminaGaugeVisible(true);
            ResetStaminaGaugeColor();
            _staminaInvisibleDelayTime = 0f;
            _staminaFadeTime = 0f;
        }
        StaminaChanged(currentStamina, maxStamina);
    }
    
    private void HandlePlayerControllerChanged(PlayerController player)
    {
        _activePlayerController = player;
    }

    private void HpChanged(int currentHp, int maxHp)
    {
        if (currentHp <= 0)
            currentHp = 0;

        _hpTargetFill = (maxHp <= 0) ? 0f : (float)currentHp / maxHp;
        _hpText.text = $"{currentHp} / {maxHp}";
        _isHpAnimating = true;
    }

    private void UpdateHpGauge()
    {
        if (!_isHpAnimating) return;

        _hpBar.fillAmount = Mathf.MoveTowards(
            _hpBar.fillAmount,
            _hpTargetFill,
            FillPerSecond * Time.deltaTime
        );

        if (Mathf.Abs(_hpBar.fillAmount - _hpTargetFill) <= Epsilon)
        {
            _hpBar.fillAmount = _hpTargetFill;
            _isHpAnimating = false;
        }
    }

    private void StaminaChanged(int currentStamina, int maxStamina)
    {
        _staminaTargetFill = (maxStamina <= 0) ? 0f : (float)currentStamina / maxStamina;
        _isStaminaAnimating = true;
    }

    private void UpdateStaminaGauge()
    {
        if (!_isStaminaAnimating) return;

        _staminaBar.fillAmount = Mathf.MoveTowards(
            _staminaBar.fillAmount,
            _staminaTargetFill,
            FillPerSecond * Time.deltaTime
        );

        if (Mathf.Abs(_staminaBar.fillAmount - _staminaTargetFill) <= Epsilon)
        {
            _staminaBar.fillAmount = _staminaTargetFill;
            _isStaminaAnimating = false;
        }
    }    

    private void StartStaminaGaugeFadeOutSequence()
    {
        if (_staminaInvisibleDelayTime < 1f) // 1초 기다린 후..
        {
            _staminaInvisibleDelayTime += Time.deltaTime;
            return;
        }

        // FadeOut시작..
        _staminaFadeTime += Time.unscaledDeltaTime;
        float alpha = Mathf.Clamp01(_staminaFadeTime / Mathf.Max(0.0001f, _staminaFadeOutDuration));
        SetStaminaGaugeFadeOutAlpha(1f - alpha);

        if (alpha >= 1f)
        {
            ResetStaminaGaugeColor();
            SetStaminaGaugeVisible(false);
            _staminaInvisibleDelayTime = 0f;
            _staminaFadeTime = 0f;
        }
    }

    private void HandleStaminaRecoveryCheck()
    {
        if (_activePlayerController == null)
            return;

        if (_activePlayerController.FSM.GetCurrentState() is IdleState || _activePlayerController.FSM.GetCurrentState() is WalkState)
        {
            if (_playerManager.CurrentStamina >= _playerManager.MaxStamina)
            {
                if (_staminaBar.enabled && _staminaBarBg.enabled)
                {
                    StartStaminaGaugeFadeOutSequence();
                }
                else
                {
                    return;
                }
            }

            _recoveryTimer += STAMINA_REGEN_PER_SECOND * Time.deltaTime;

            int staminaToAdd = Mathf.FloorToInt(_recoveryTimer);
            if (staminaToAdd <= 0)
                return;

            _recoveryTimer -= staminaToAdd;
            _activePlayerController.RecoveryStamina(staminaToAdd);
        }
    }

    private void HandleEXPGaugeUpdate(int currentExp, int requireExp)
    {
        if (PlayerManager.Instance.PartyService.GetCurrentCharacterInActiveParty().Level >= CharacterLevelManager.Instance.GetMaxLevel())
        {
            _expBar.fillAmount = 1f;
        }
        else
        {
            _expBar.fillAmount = (float)currentExp / requireExp;
        }
    }
}
