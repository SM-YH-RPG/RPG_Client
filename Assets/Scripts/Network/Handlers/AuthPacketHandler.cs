using UnityEngine;

public class AuthPacketHandler
{
    public void HandleLoginResponse(LoginResponsePacket response)
    {
        if (response.Success && response.Code == ENetworkStatusCode.Success)
        {
            Debug.Log("로그인 성공");
            
            PlayerPrefs.SetString("USER_NAME", response.User.Name);
            PlayerPrefs.SetString("IDENTIFY_CODE", response.User.IdentifyCode);
            
            //PlayerManager.Instance.UpdateCurrentCurrencyValue(response.User.Gold);
            //PlayerManager.Instance.InventoryManagerService.Initialize(response.User.InventoryItems);
            //PlayerManager.Instance.PartyService.SetCurrentSelectedPartyPresetIndex(response.User.SelectPresetID);
            //PlayerManager.Instance.PartyService.Initialize(response.User.PartyPresets);
            //PlayerManager.Instance.CharacterService.Initialize(response.User.OwnsCharacterList, response.User.PartyPresets);


            //WorldInfoRequestPacket packet = new WorldInfoRequestPacket();
            //UnityNetworkBridge.Instance.SendPacket(packet);
            //SceneManager.Instance.ChangeScene("InGameScene");
        }
        else
        {
            Debug.Log($"로그인 실패 : {response.Code} Message {response.Message}");
        }
    }

    public void HandleStatusResponse(CharacterStatsResponsePacket response) { }     
    
}