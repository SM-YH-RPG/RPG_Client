using UnityEngine;
using UnityEngine.UI;

public class InvenCategoryButton : MonoBehaviour
{
    [SerializeField] private Button _categoryButton;

    private int _categoryIndex;

    private IInventoryManagerService _inventoryManagerService => PlayerManager.Instance.Inventory;

    private void Awake()
    {
        _categoryButton.onClick.AddListener(OnClickCategoryButton);
    }

    public void InitCategoryButton(int index)
    {
        _categoryIndex = index;
    }

    private void OnClickCategoryButton()
    {
        _inventoryManagerService.SelectCategory((EItemCategory)_categoryIndex);
    }
}
