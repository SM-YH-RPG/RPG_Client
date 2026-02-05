using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class InGameSceneController : BaseScene
{
    private const float AUTO_SAVE_INTERVAL_TIME = 600f;

    #region Inspector
    [SerializeField]
    private JoyPadUI _gamepadUI = null;

    [SerializeField]
    private InteractionUI _interactionUI = null;

    [SerializeField]
    private CameraDirector _lookAtCamera = null;

    [SerializeField]
    private MiniMapCamera _miniMapCamera = null;

    [SerializeField]
    private GameObject _damageTextPrefab = null;

    [SerializeField]
    private GameObject[] _playerPrefab = null;

    [SerializeField]
    private Transform _spawnPoint;

    [SerializeField]
    private Transform _worldInfoCreateRoot;

    [SerializeField]
    private ObjectPoolPreset _poolPreset = null;

    [SerializeField]
    private MergeItemConfig _mergeItemConfig = null;

    [SerializeField]
    private PlacementPrefabDataConfig _placementConfig = null;

    [SerializeField]
    private EnhanceConfigData _weaponEnhanceConfig = null;

    [SerializeField]
    private EnhanceConfigData _equipmentEnhanceConfig = null;

    [SerializeField]
    private ConsumeableItemConfigData _consumeableConfig = null;

    [SerializeField]
    private CharacterLevelConfig _characterLevelConfig = null;
    #endregion

    private PlayerController _playerCtrl = null;

    private InGameManager _gameManager = null;

    private PlayerManager _playerManager => PlayerManager.Instance;
    private IPartyService _partyService = PlayerManager.Instance.PartyService;
    private IInventoryManagerService _inventoryManagerService => PlayerManager.Instance.Inventory;

    private float _autoSaveTime;

    private void Awake()
    {
        _autoSaveTime = 0f;

        InitConfigDatas();
        _gameManager = InGameManager.Instance;
        _gameManager.SetAllCountCharacterCount(_playerPrefab.Length);
        InstantiateAllPlayerObjects();

        _partyService.OnPartyCharacterSwapped += HandleCharacterObjectSwapeed;

        if (SaveManager.Instance.TryLoadOnStart() == false)
        {
            _playerManager.Initialize();
            TempAddWeaponItem();
            TempAddEquipItem();
            _partyService.Initialize(null);
            _playerManager.CharacterService.Initialize(null);
            InitPlayerStat();
            _partyService.ChangeActiveParty(0);
        }
        else
        {
            InitPlayerStat();                        
        }

        int selectedCharacterIndex = _playerManager.CharacterService.GetRunTimeCharacterBy(_partyService.CurrentParty.Characters[_partyService.SelectedIndexInParty]).TemplateId;
        _playerCtrl = _gameManager.GetPlayerController(selectedCharacterIndex);
        InitPlayerController(_playerCtrl);

        if (WorldDataManager.Instance.WorldData != null)
        {
            // 로딩중 배치 오브젝트 스폰.. Local전용 코드(None InstanceID)
            SpawnPlacementPrefab(WorldDataManager.Instance.WorldData.Npcs);
            SpawnPlacementPrefab(WorldDataManager.Instance.WorldData.Spawners);
            SpawnPlacementPrefab(WorldDataManager.Instance.WorldData.GatherableInteractions);
            SpawnPlacementPrefab(WorldDataManager.Instance.WorldData.DestoryableInteractions);
        }

        _poolPreset.Initialize();
        UIPoolingObjectSpawnManager.Instance.Initialize(_damageTextPrefab);
    }

    private void Start()
    {
#if UNITY_STANDALONE
        Cursor.lockState = CursorLockMode.Locked;
        _gamepadUI.gameObject.SetActive(false);
        //        UIProvider uIProvider = new PcUIProvider();
#else
        _gamepadUI.gameObject.SetActive(true);
        //        UIProvider uiProvider = new MobileUIProvider();
#endif
        //        uIProvider.GenerateUI("");

        _partyService.ChangeActiveParty(_partyService.CurrentSelectedPartyPresetIndex);
        _partyService.RequestCharacterSwap(_partyService.SelectedIndexInParty);
    }

    private void Update()
    {
        _autoSaveTime += Time.unscaledDeltaTime;
        if (_autoSaveTime >= AUTO_SAVE_INTERVAL_TIME)
        {
            _autoSaveTime = 0f;
            SaveManager.Instance.RequestSave(ESaveCategory.AutoSave);
        }
    }

    private void OnDestroy()
    {
        _partyService.OnPartyCharacterSwapped -= HandleCharacterObjectSwapeed;        
    }

    private void InitConfigDatas()
    {
        if (_weaponEnhanceConfig != null)
            WeaponDataManager.Instance.SetWeaponEnhanceConfigData(_weaponEnhanceConfig);
        if (_equipmentEnhanceConfig != null)
            EquipmentDataManager.Instance.SetEquipmentEnhanceConfigData(_equipmentEnhanceConfig);
        if (_consumeableConfig != null)
            ItemDataManager.Instance.SetConsumeableItemConfig(_consumeableConfig);
        if (_mergeItemConfig != null)
            MergeDataManager.Instance.Initialize(_mergeItemConfig);
        if (_characterLevelConfig != null)
            CharacterLevelManager.Instance.InitLevelData(_characterLevelConfig);
    }

    private void InstantiateAllPlayerObjects()
    {        
        for (int i = 0; i < _playerPrefab.Length; i++)
        {
            GameObject playerObject = Instantiate(_playerPrefab[i]);
            if (playerObject.TryGetComponent(out PlayerController playerCtrl))
            {                
                playerObject.SetActive(false);

                _gameManager.AddPlayer(playerCtrl);                
            }
        }
    }

    private void InitPlayerStat()
    {
        foreach (var (index, playerCtrl) in _gameManager.PlayerControllerDict)
        {
            long characterUniqueIndex = PlayerManager.Instance.CharacterService.GetCharacterUniqueIndex(playerCtrl.CharacterData.Index);
            PlayerManager.Instance.CharacterService.UpdateCharacterStatData(characterUniqueIndex, playerCtrl.CharacterData.StatData);
        }
    }

    private void InitPlayerController(PlayerController playerCtrl)
    {
        playerCtrl.Init(_lookAtCamera, _miniMapCamera, _spawnPoint.position);
        _interactionUI.Init();
        _gameManager.NotifyActivePlayerChanged(playerCtrl);

#if UNITY_ANDROID || UNITY_IOS
        _gamepadUI.UpdateMoveTarget(playerCtrl);
#endif
    }

    #region 처음 플레이 생성 계정 아이템 추가 코드
    private void TempAddWeaponItem()
    {
        int weaponTotalCount = 20;
        int defaultWeaponIndex = 1001;

        for (int i = 0; i < weaponTotalCount; i++)
        {            
            ItemConfigData config = ItemDataManager.Instance.GetItemConfigData(EItemCategory.Weapon, i + defaultWeaponIndex);
            _inventoryManagerService.AddItem(i + defaultWeaponIndex, EItemCategory.Weapon, (EItemGrade)(i % (int)EItemGrade.End), config.Name);
        }        
    }

    private void TempAddEquipItem()
    {
        int equipmentTotalCount = 12;
        int defaultEquipmentIndex = 2001;

        for (int i = 0; i < equipmentTotalCount; i++)
        {            
            ItemConfigData config = ItemDataManager.Instance.GetItemConfigData(EItemCategory.Equipment, i + defaultEquipmentIndex);
            _inventoryManagerService.AddItem(i + defaultEquipmentIndex, EItemCategory.Equipment, config.template.Grade, config.Name);
        }
    }
    #endregion

    private void SpawnPlacementPrefab<T>(List<T> list) where T : WorldPlacementBase
    {
        if (list == null || list.Count == 0) return;

        foreach (var item in list)
        {
            if (item == null) continue;

            var pos = TransformConvertUtil.ToVector3(item.pos);
            var rot = TransformConvertUtil.ToQuaternion(item.rot);
            var config = _placementConfig.GetPlacementPrefaConfigData(item.templateId);
            var go = Instantiate(config.Prefab, pos, rot, _worldInfoCreateRoot);
            if (go.TryGetComponent(out ClientPlacementObjectBase placement))
            {
                if (placement is SpawnAreaBase spawnArea)
                {
                    spawnArea.InitSpawnData(item);
                }
                placement.SetInstanceId(item.instanceId);
                placement.SetActiveGameObject(true);
                _gameManager.AddWorldPlacementObject(placement);
            }
        }
    }

    #region Handle

    private void HandleCharacterObjectSwapeed(RuntimeCharacter characterStats)
    {
        Vector3 position = Vector3.zero;
        Quaternion rotation = Quaternion.identity;
        if (_playerCtrl != null)
        {
            _playerCtrl.SetActivate(false);
            position = _playerCtrl.transform.position;
            rotation = _playerCtrl.transform.rotation;
        }

        _playerCtrl = _gameManager.PlayerControllerDict[characterStats.TemplateId];
        _playerCtrl.transform.position = position;
        _playerCtrl.transform.rotation = rotation;

        _playerCtrl.Rebind(_lookAtCamera,  _miniMapCamera);
        
        _playerCtrl.SetActivate(characterStats.CurrentHP > 0); // 살아있는 캐릭터만 켜주기

        _gameManager.NotifyActivePlayerChanged(_playerCtrl);

#if UNITY_ANDROID || UNITY_IOS
        _gamepadUI.UpdateMoveTarget(_playerCtrl);
#endif
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            SaveManager.Instance.SaveNow();
        }
    }

    private void OnApplicationQuit()
    {
        SaveManager.Instance.SaveNow();
    }
    #endregion
}
