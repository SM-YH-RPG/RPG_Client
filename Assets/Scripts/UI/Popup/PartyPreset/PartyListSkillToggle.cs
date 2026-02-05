using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PartyListSkillToggle : MonoBehaviour
{
    [SerializeField] private Toggle _toggle;

    [SerializeField] private Image _skillIcon;

    [SerializeField] private Image _selectLine;

    [SerializeField] private TextMeshProUGUI _skillName;

    [SerializeField] private TextMeshProUGUI _skillDesc;

    private SkillUIConfig _skillData;

    private void Awake()
    {
        _toggle.onValueChanged.AddListener(OnValueChangedSkillToggle);        
    }

    public void InitSkillToggleData(SkillUIConfig skill)
    {
        _skillData = skill;
        _skillIcon.sprite = skill.skillUIInfo.SkillIcon;
        OnValueChangedSkillToggle(_toggle.isOn);
    }

    private void OnValueChangedSkillToggle(bool isOn)
    {
        _selectLine.enabled = isOn;
        _toggle.isOn = isOn;
        if (_skillData != null && isOn)
        {
            _skillName.text = _skillData.skillUIInfo.Name;
            _skillDesc.text = _skillData.skillUIInfo.Description;
        }
    }

    public void SelectSkillToggle(bool isOn)
    {
        OnValueChangedSkillToggle(isOn);
    }
}
