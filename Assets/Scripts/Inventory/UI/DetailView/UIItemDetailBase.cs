using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class UIItemDetailBase : MonoBehaviour
{
    [SerializeField]
    protected Image _itemImage;
    
    [SerializeField]
    protected TextMeshProUGUI _itemName;

    public abstract void UpdateSelectView(BaseInventoryItem _data);
}
