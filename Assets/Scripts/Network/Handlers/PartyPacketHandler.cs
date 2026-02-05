using UnityEngine;

public class PartyPacketHandler
{
    public void HandleUpdateDeployPartyPresetResponse(UpdatePartyPresetResponsePacket response) 
    {
        if (response.Success && response.Code == ENetworkStatusCode.Success)
        {
            if (PartyManager.Instance.isActivePartyChange) // 액티브 파티 교체 출전 (프리셋 자체가 바뀌었을때)
            {
                //PartyManager.Instance.SuccessDeployPartyPresetUpdate();
            }
            else // 비활성 파티 멤버가 바뀌었을때
            {
                //PartyManager.Instance.SuccessChangePartyMemberInactivePartyPreset();
            }
        }
        else
        {
            if (PartyManager.Instance.isActivePartyChange)
            {
                Debug.Log($"액티브 파티 교체 실패 !! Code : {response.Code} Message : {response.Message}");
            }
            else
            {
                Debug.Log($"파티 멤버 교체 실패 !! Code : {response.Code} Message : {response.Message}");
            }
        }
    }    
}
