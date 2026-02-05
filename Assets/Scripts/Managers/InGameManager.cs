using System;
using System.Collections.Generic;
using UnityEngine;

public class InGameManager : LazySingleton<InGameManager>
{
    private readonly Dictionary<long, PlayerController> _playerControllerDict = new Dictionary<long, PlayerController>();
    public IReadOnlyDictionary<long, PlayerController> PlayerControllerDict => _playerControllerDict;

    private readonly Dictionary<int, SkillGroupConfig> _playerSkillGroupConfigDict = new Dictionary<int, SkillGroupConfig>();
    public IReadOnlyDictionary<int, SkillGroupConfig> PlayerSkillGroupConfigDict => _playerSkillGroupConfigDict;

    private readonly Dictionary<long, ClientPlacementObjectBase> _worldPlacementObjectDict = new Dictionary<long, ClientPlacementObjectBase>();
    public IReadOnlyDictionary<long, ClientPlacementObjectBase> WorldPlacementObjectDict => _worldPlacementObjectDict;

    private ConsumableController _consumableController;
    public ConsumableController ConsumableController => _consumableController;

    #region Action
    public event Action<PlayerController> OnActivePlayerControllerChanged;
    public event Action<bool> OnActiveControlInShop;
    #endregion

    private PlayerLevel _playerLevel;

    private int _allCharacterCount; // 서버 붙지 않았을때 캐릭터 생성용..
    public int AllCharaceterCount => _allCharacterCount;

    public void InitConsumableController(ConsumableController controller)
    {
        _consumableController = controller;
    }

    public void Init()
    {
        _playerLevel = new PlayerLevel();
    }

    public void GetExp(int exp)
    {
        _playerLevel?.GetExp(exp);
    }    

    public void AddPlayer(PlayerController player)
    {
        var data = player.CharacterData;

        if (_playerControllerDict.ContainsKey(data.Index) == false)
        {
            _playerControllerDict.Add(data.Index, player);
        }
        else
        {
            _playerControllerDict[data.Index] = player;
        }

        if (_playerSkillGroupConfigDict.ContainsKey(data.Index) == false)
        {
            _playerSkillGroupConfigDict.Add(data.Index, data.SkillGroup);
        }
    }

    public void AddWorldPlacementObject(ClientPlacementObjectBase placement)
    {
        if (_worldPlacementObjectDict.ContainsKey(placement.GetInstanceId()) == false)
        {
            _worldPlacementObjectDict.Add(placement.GetInstanceId(), placement);
        }
    }

    public PlayerController GetPlayerController(int characterIndex)
    {
        if (_playerControllerDict.TryGetValue(characterIndex, out PlayerController controller))
        {
            return controller;
        }

        return null;
    }
    
    public void WorldPlacementObjectRespawn(long instanceId)
    {
        if (_worldPlacementObjectDict.TryGetValue(instanceId, out var placement))
        {
            if (placement is IRespawnable interactionObj)
            {
                interactionObj.OnRespawned();
            }
        }
    }

    public void NotifyActivePlayerChanged(PlayerController newController)
    {
        OnActivePlayerControllerChanged?.Invoke(newController);
    }

    public void SetAllCountCharacterCount(int count)
    {
        _allCharacterCount = count;
    }

    public void NotifyObjectSetActiveInShop(bool isActive)
    {
        OnActiveControlInShop?.Invoke(isActive);
    }
}