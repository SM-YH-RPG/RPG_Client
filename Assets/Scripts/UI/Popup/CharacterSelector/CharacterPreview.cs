using TMPro;
using UnityEngine;

public class CharacterPreview : MonoBehaviour
{
    #region Inspector
    [SerializeField]
    private TextMeshProUGUI nameText;

    [SerializeField]
    private TextMeshProUGUI descText;

    [SerializeField]
    private TextMeshProUGUI hpText;

    [SerializeField]
    private TextMeshProUGUI damageText;

    [SerializeField]
    private TextMeshProUGUI moveSpeedText;
    #endregion

    public void Clear()
    {
        nameText.text = string.Empty;
        descText.text = string.Empty;

        hpText.text = string.Empty;
        damageText.text = string.Empty;
        moveSpeedText.text = string.Empty;
    }

    public void OnUpdate(CharacterData _data)
    {
        nameText.text = _data.Name;
        descText.text = _data.Description;

        hpText.text = $"{_data.BaseHP}";
        damageText.text = $"{_data.BaseDamage}";
    }
}