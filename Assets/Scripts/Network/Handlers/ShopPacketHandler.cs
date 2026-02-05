using UnityEngine;

public class ShopPacketHandler
{
    public void HandlePurchaseItemResponse(PurchaseItemResponsePacket response)
    {
        if (response.Success && response.Code == ENetworkStatusCode.Success)
        {                        
            //ShopDataManager.Instance.SuccessPurchaseDataShopPopup(response.Item);
            //PlayerManager.Instance.UpdateCurrentCurrencyValue(response.RemainingGold);
        }
        else
        {
            Debug.Log($"구매 아이템 인벤토리 추가 실패 !! : Code : {response.Code} Message {response.Message}");
        }
    }

    public void HandleShopItemListResponse(ShopItemListResponsePacket response)
    {
        if (response.Success && response.Code == ENetworkStatusCode.Success)
        {
            //ShopDataManager.Instance.InitShopItemListByCategory(ShopDataManager.Instance.CurrentCategory, response.Items);
            //ShopDataManager.Instance.LoadDataCreateElement();
        }
        else
        {
            Debug.Log($"상점 아이템 데이터 로드 실패 !! Code {response.Code} Message {response.Message}");
        }
    }
}
