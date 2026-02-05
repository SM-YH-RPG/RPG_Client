using UnityEngine;

public class ConsumptionStoreNPC : InteractableNPCCharacter
{
    private IUIManagerService _UIManagerService => UIManager.Instance;

    public async override void Interact()
    {
        base.Interact();

        var popup = await _UIManagerService.Show<ShopPopup>();
        popup.SetShopCategoryType(ShopCategory);
    }
}
