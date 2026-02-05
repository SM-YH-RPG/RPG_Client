using UnityEngine;

public class CharacterPacketHandler 
{
    public void HandleUseCharacterExpItemResponse(UseCharacterExpItemResponse response) { }

    public void HandleSwapActivePartyCharacterResponse(SwapActivePartyCharacterResponsePacket response)
    {
        if (response.Success && response.Code == ENetworkStatusCode.Success)
        {
            //PartyManager.Instance.SuccessSwapCharacter();
        }
        else
        {
            Debug.Log($"액티브 파티 멤버 Swap실패!! Code : {response.Code} Message {response.Message}");
        }
    }

    public void HandleReviveAllPartyMemberResponsePacket(ReviveAllPartyMemberResponsePacket response)
    {
        if (response.Success && response.Code == ENetworkStatusCode.Success)
        {
            //UIManager.Instance.Hide(); // 부활 팝업 닫기
            //PlayerManager.Instance.PartyService.RequestCharacterSwap(0); // 살아나고 첫번째 인덱스 캐릭터부터 다시 플레이..
        }
        else
        {
            Debug.Log($"부활 실패 ! Code : {response.Code} Message {response.Message}");
        }
    }

    public void HandleUseConsumeableItemResponse(UseConsumeableItemResponsePacket response)
    {
        if (response.Success && response.Code == ENetworkStatusCode.Success)
        {
            //CharacterManager.Instance.SuccessUseConsumableItem();
        }
        else
        {
            Debug.Log($"소모품 사용 실패 ! Code : {response.Code} Message {response.Message}");
        }
    }
}
