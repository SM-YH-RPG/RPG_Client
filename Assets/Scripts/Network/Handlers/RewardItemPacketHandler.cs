using UnityEngine;

public class RewardItemPacketHandler
{
    public void HandleGatheringResponsePacket(GatheringResponsePacket response)
    {
        if (response.Success && response.Code == ENetworkStatusCode.Success)
        {
            // UI 출력용 패킷
            //RewardItemManager.Instance.SuccessGetInteractionRewardItem(response.Items);
        }
        else
        {
            Debug.Log($"리워드 아이템 획득 실패!! Code : {response.Code} Message {response.Message}");
        }
    }
}
