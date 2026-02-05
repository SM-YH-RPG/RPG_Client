using System;
using UnityEngine;

public class CharacterLevelManager : LazySingleton<CharacterLevelManager>
{    
    public event Action<int, int> OnUpdateExp;
    public event Action<int, int> OnUpdateLevelUpCharacterCurrentHp;

    private CharacterLevelConfig _config;
    private LevelConfigData _levelData;
    private RuntimeCharacter _currentPlayCharacter;
    private PlayerController _currentPlayerCtrl;

    public void InitLevelData(CharacterLevelConfig config)
    {
        _config = config;
    }

    public void ChangedPlayCharacter(RuntimeCharacter currentCharacter)
    {
        _currentPlayCharacter = currentCharacter;
        _levelData = _config.GetLevelConfigData(currentCharacter.Level);
        _currentPlayerCtrl = InGameManager.Instance.GetPlayerController(currentCharacter.TemplateId);
        OnUpdateExp?.Invoke(_currentPlayCharacter.Exp, _levelData.RequireExp);
    }

    public void GetExp(int _exp)
    {
        if (_currentPlayCharacter.Level >= _config.MaxLevel)
        {
            return;
        }

        _currentPlayCharacter.Exp += _exp;

        bool levelUp = false;

        while (_currentPlayCharacter.Level < _config.MaxLevel && _currentPlayCharacter.Exp >= _levelData.RequireExp)
        {
            levelUp = true;

            _currentPlayCharacter.Exp -= _levelData.RequireExp;
            _currentPlayCharacter.Level += 1;
            _levelData = _config.GetLevelConfigData(_currentPlayCharacter.Level);
        }

        OnUpdateExp?.Invoke(_currentPlayCharacter.Exp, _levelData.RequireExp);

        if (levelUp)
        {
            EffectManager.Instance.SpawnEffect(_config.LevelUpEffect, _currentPlayerCtrl.transform.position, _currentPlayerCtrl.transform.rotation);
         
            PlayerManager.Instance.CharacterService.UpdateCharacterStatData(_currentPlayCharacter.InstanceId, _currentPlayerCtrl.CharacterData.StatData);
            _currentPlayCharacter.CurrentHP = _currentPlayCharacter.MaxHp;

            PlayerManager.Instance.CharacterService.UpdateCurrentCharacterHp(_currentPlayCharacter.CurrentHP, _currentPlayCharacter.InstanceId);
            OnUpdateLevelUpCharacterCurrentHp?.Invoke(_currentPlayCharacter.TemplateId, _currentPlayCharacter.CurrentHP);
        }        
    }

    public LevelConfigData GetLevelData(int level)
    {
        return _config.GetLevelConfigData(level);
    }

    public int GetMaxLevel()
    {
        return _config.MaxLevel;
    }
}
