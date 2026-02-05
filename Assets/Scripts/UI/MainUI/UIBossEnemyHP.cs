using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBossEnemyHP : MonoBehaviour
{
    private const float HP_GAUGE_VISIBLE_TIME = 5f;
    [SerializeField]
    private Image _hpBarImage = null;

    [SerializeField]
    private TextMeshProUGUI _enemyNameText = null;

    private float _lastEnemyAttackTime = 0f;    

    private void Awake()
    {
        EnemyHPHandler.Instance.OnBossEnemyHPChanged += HandleEnemyHPChanged;        
        _enemyNameText.text = string.Empty;
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        EnemyHPHandler.Instance.OnBossEnemyHPChanged -= HandleEnemyHPChanged;        
    }

    private void Update()
    {
        if (gameObject.activeSelf && _lastEnemyAttackTime != 0f)
        {
            if (Time.unscaledTime - _lastEnemyAttackTime > HP_GAUGE_VISIBLE_TIME)
            {
                ResetHPGauge();
            }
        }
    }

    private void HandleEnemyHPChanged(string name, int currentHP, int maxHP)
    {
        if (currentHP <= 0)
        {
            ResetHPGauge();
            return;
        }

        gameObject.SetActive(true);
        _enemyNameText.text = name;
        _hpBarImage.fillAmount = (float)currentHP / maxHP;
        _lastEnemyAttackTime = Time.unscaledTime;
    }

    private void ResetHPGauge()
    {
        _hpBarImage.fillAmount = 1f;
        _enemyNameText.text = string.Empty;
        _lastEnemyAttackTime = 0f;
        gameObject.SetActive(false);
    }
}
