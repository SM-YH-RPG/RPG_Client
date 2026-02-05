using UnityEngine;

public class WorldMapPacketHandler
{
    public void HandleWorldInfoResponsePacket(WorldInfoResponsePacket resposne)
    {
        if (resposne.Success && resposne.Code == ENetworkStatusCode.Success)
        {
            //WorldDataManager.Instance.Initialize(resposne.worldData);
        }
        else
        {
            Debug.Log($"월드 정보 불러오기 실패! Code : {resposne.Code} Message {resposne.Message}");
        }
    }
}
