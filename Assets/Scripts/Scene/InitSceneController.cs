using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
using UnityEngine;

public class InitSceneController : BaseScene
{
    [SerializeField] private ItemGroupConfig _itemGroupConfig = null;

    [SerializeField] private RewardItemConfig _rewardItemConfig = null;

    [SerializeField] private WeaponItemListConfig _weaponItemListConfig = null;

    [SerializeField] private EquipItemListConfig _equipItemListconfig = null;

    private void Awake()
    {
        if (_itemGroupConfig != null)
            ItemDataManager.Instance.InitializeData(_itemGroupConfig);

        if (_rewardItemConfig != null)
            RewardItemManager.Instance.InitializeData(_rewardItemConfig);

        if (_weaponItemListConfig != null)
            WeaponDataManager.Instance.Initialize(_weaponItemListConfig);

        if (_equipItemListconfig != null)
            EquipmentDataManager.Instance.Initialize(_equipItemListconfig);

        LoadTable();
    }

    private void WorldInfoJsonLoadForLocal()
    {
        TextAsset ta = Resources.Load<TextAsset>("WorldInfo");

        // 2) 역직렬화
        WorldPlacementData data;
        try
        {
            data = JsonConvert.DeserializeObject<WorldPlacementData>(ta.text);
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to deserialize JSON: {e}");
            return;
        }

        if (data == null)
        {
            Debug.LogError("Deserialized data is null.");
            return;
        }

        WorldDataManager.Instance.Initialize(data);
    }

    private async void LoadTable()
    {
        await CharacterTable.Instance.LoadTable();
        await ShopTable.Instance.LoadTable();

        long size =  await AddressableManager.Instance.Downloader.GetDownloadSizeAsync("preload");        
        await AddressableManager.Instance.Downloader.DownloadRemoteDependenciesAsync("preload");

        WorldInfoJsonLoadForLocal();
        SceneManager.Instance.ChangeScene("InGameScene");
        return;
        await UniTask.WaitUntil(() => UnityNetworkBridge.Instance.IsConnected);

        string name = PlayerPrefs.GetString("USER_NAME");
        string identify_code = PlayerPrefs.GetString("IDENTIFY_CODE");
        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(identify_code))
        {
            LoginRequestPacket packet = new LoginRequestPacket()
            {
                Username = string.Empty,
                IdentifyCode = string.Empty
            };

            UnityNetworkBridge.Instance.SendPacket(packet);
        }
        else
        {
            LoginRequestPacket packet = new LoginRequestPacket()
            {
                Username = name,
                IdentifyCode = identify_code
            };

            UnityNetworkBridge.Instance.SendPacket(packet);
        }
    }
}
