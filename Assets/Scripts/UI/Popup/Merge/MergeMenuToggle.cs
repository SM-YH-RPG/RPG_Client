using System;
using UnityEngine;
using UnityEngine.UI;

public class MergeMenuToggle : MonoBehaviour
{
    [SerializeField] private Toggle _toggle;

    private Action<string> OnChangeMenuName;

    private EItemCategory _category;

    private void Awake()
    {
        _toggle.onValueChanged.AddListener(OnSelectToggle);
    }

    public void InitToggle(EItemCategory category)
    {
        _category = category;
    }

    private void OnSelectToggle(bool isOn)
    {
        if (isOn)
        {
            switch (_category)
            {
                case EItemCategory.Weapon:
                    OnChangeMenuName?.Invoke("무기 제작");
                    break;
                case EItemCategory.Equipment:
                    OnChangeMenuName?.Invoke("장비 제작");
                    break;
                case EItemCategory.Consumable:
                    OnChangeMenuName?.Invoke("약제 제작");
                    break;
                case EItemCategory.Material:
                    OnChangeMenuName?.Invoke("정제");
                    break;
                case EItemCategory.End:                    
                default:
                    OnChangeMenuName?.Invoke(string.Empty);
                    break;
            }
        }
    }
}
