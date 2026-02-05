using UnityEngine;

public class EquipPacketHandler
{
    public void HandleWeaponItemEquipResponse(WeaponItemEquipResponsePacket response)
    {
        if (response.Success && response.Code == ENetworkStatusCode.Success)
        {
            //WeaponManager.Instance.SuccessEquipWeaponUpdateData();
        }
        else
        {
            Debug.Log($"무기 교체 실패 !! : Code {response.Code} Message {response.Message}");
        }
        // TODO :: 실패에 대한 예외처리
    }

    public void HandleEquipmentItemEquipResponse(EquipmentItemEquipResponsePacket response)
    {
        if (response.Success && response.Code == ENetworkStatusCode.Success)
        {
            //EquipmentManager.Instance.SuccessEquipmentItemEquipUpdateData();
        }
        else
        {
            Debug.Log($"장비 장착 실패 !! : Code {response.Code} Message {response.Message}");
        }
        // TODO :: 실패에 대한 예외처리
    }

    public void HandleEquipmentItemUnequipResponse(EquipmentItemUnequipResponsePacket response)
    {
        if (response.Success && response.Code == ENetworkStatusCode.Success)
        {
            //EquipmentManager.Instance.SuccessEquipmentItemUnequipUpdateData();
        }
        else
        {
            Debug.Log($"장비 해제 실패 !! : Code {response.Code} Message {response.Message}");
        }
        // TODO :: 실패에 대한 예외처리
    }

    public void HandleEnhanceWeaponItemResponse(EnhanceWeaponResponsePacket response) 
    {
        if (response.Success && response.Code  == ENetworkStatusCode.Success)
        {
            if (response.EnhanceSuccess) // 강화 성공
            {
                //WeaponManager.Instance.SuccessWeaponEnhance();
            }
            else // 강화 실패
            {                
                //WeaponManager.Instance.FaildWeaponEnhance();
            }
        }
        else
        {
            Debug.Log($"무기 강화 실패 !! : Code {response.Code} Message {response.Message}");
        }
    }

    public void HandleEnhanceEquipmentItemResponse(EnhanceEquipmentResponsePacket response) 
    {
        if (response.Success && response.Code == ENetworkStatusCode.Success)
        {
            if (response.EnhanceSuccess) // 강화 성공
            {
                //EquipmentManager.Instance.SuccessEquipmentEnhance();
            }
            else // 강화 실패
            {                
                //EquipmentManager.Instance.FaildEquipmentEnhance();
            }
        }
        else
        {
            Debug.Log($"장비 강화 실패 !! : Code {response.Code} Message {response.Message}");
        }
    }
}
