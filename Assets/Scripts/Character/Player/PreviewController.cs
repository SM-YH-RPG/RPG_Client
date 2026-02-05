using System.Collections.Generic;
using UnityEngine;

public class PreviewController : MonoBehaviour
{
    [SerializeField]
    private Camera _previewCamera;
    private Transform _cameraTransform;

    [SerializeField]
    private Vector3 _characterPositionOffest;    

    private Dictionary<int, GameObject> _cachedCharacterDict = new Dictionary<int, GameObject>();
    private GameObject _currentCharacter;    

    private void Awake()
    {
        _cachedCharacterDict.Clear();
        TryGetComponent(out _previewCamera);        
    }

    public void SetEnable(bool isEnable)
    {
        _previewCamera.enabled = isEnable;
        _currentCharacter.SetActive(isEnable);
    }


    public async void ChangePreviewCharacter(int index)
    {       
        _cameraTransform = _previewCamera.transform;
        if (_currentCharacter != null)
        {
            _currentCharacter.SetActive(false);
        }
        if (_cachedCharacterDict.ContainsKey(index))
        {
            _currentCharacter = _cachedCharacterDict[index];
            _currentCharacter.SetActive(true);
        }
        else
        {
            CharacterConfig config = InGameManager.Instance.GetPlayerController(index).CharacterData;            
            GameObject characterPrefab = await ResourcesManager.Instance.LoadPreviewCharacter(config.PrefabName);
            if (characterPrefab == null)
            {
                Debug.Log($"{config.PrefabName} Preview Prefab is Null!!");
                return;
            }

            _currentCharacter = Instantiate(characterPrefab);
            Transform previewTransform = _currentCharacter.transform;

            var position = previewTransform.position;
            position = _cameraTransform.position;
            position += _characterPositionOffest;
            previewTransform.position = position;

            var previewRotation = previewTransform.rotation;
            previewRotation.y = 180f;
            previewTransform.rotation = previewRotation;

            if (!_cachedCharacterDict.ContainsKey(index))
                _cachedCharacterDict.Add(index, _currentCharacter);
        }
    }

    private void OnDestroy()
    {
        _cachedCharacterDict.Clear();
    }
}
