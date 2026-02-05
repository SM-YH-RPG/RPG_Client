using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardItemElement : MonoBehaviour
{
    public enum EGoodsState
    {
        Idle,
        In,
        Hold,
        Out
    }

    [SerializeField] private Image _rewardBg;
    [SerializeField] private Image _rewardImage;
    [SerializeField] private TextMeshProUGUI _amountText;
    [SerializeField] private RectTransform _transform;
    [SerializeField] private Sprite _goldSprite;

    [Header("Timings")]
    [SerializeField] private float inDuration = 0.3f;
    [SerializeField] private float holdDuration = 2.0f;
    [SerializeField] private float outDuration = 0.2f;

    private Vector3 _inStartPos;
    private float _inOffsetX = 300f;
    private Vector3 _outEndPos;
    private float _outOffsetY = 50f;
    private Vector3 _originPosition;

    private EGoodsState _currentState;
    public EGoodsState CurrentState => _currentState;

    private float _animTime = 0f;

    private Action _onRewardItemGetEffectPlayCallback;

    private static float EaseOutCubic(float t) => 1f - Mathf.Pow(1f - t, 3f);
    private static float EaseInCubic(float t) => t * t * t;

    private ItemConfigData _itemConfig;

    public void UpdateGetRewardData(RewardItemTemplate data)
    {
        _itemConfig = ItemDataManager.Instance.GetItemConfigData(data.CategoryType, data.TemplateId);
        if (data.TemplateId != 0)
        {
            _rewardImage.sprite = _itemConfig.Sprite;
            _amountText.text = $"{_itemConfig.Name}x{data.Amount}";
        }
        else
        {
            _rewardImage.sprite = _goldSprite;
            _amountText.text = $"Goldx{data.Amount}";
            PlayerManager.Instance.UpdateCurrentCurrencyValue(PlayerManager.Instance.CurrentCurrencyValue + data.Amount);
        }

        RewardItemDataAddInventory(data);
    }

    private void RewardItemDataAddInventory(RewardItemTemplate data)
    {
        switch (data.CategoryType)
        {           
            case EItemCategory.Weapon:
                break;
            case EItemCategory.Equipment:
                break;                        
            case EItemCategory.Consumable:
                ItemConfigData consumConfig = ItemDataManager.Instance.GetItemConfigData(EItemCategory.Consumable, data.TemplateId);                
                PlayerManager.Instance.Inventory.AddItem(data.TemplateId, EItemCategory.Consumable, consumConfig.template.Grade, _itemConfig.Name, data.Amount);
                break;
            case EItemCategory.Material:
                ItemConfigData materialConfig = ItemDataManager.Instance.GetItemConfigData(EItemCategory.Material, data.TemplateId);
                PlayerManager.Instance.Inventory.AddItem(data.TemplateId, EItemCategory.Material, materialConfig.template.Grade, _itemConfig.Name, data.Amount);
                break;
            default:
                break;
        }
    }

    public void SetRewardItemGetEffectPlayCallback(Action callback)
    {
        _onRewardItemGetEffectPlayCallback = callback;
    }

    public float GetOriginPositionY()
    {
        return _transform.anchoredPosition.y;
    }    

    public void SetOriginPosition(Vector3 position)
    {
        _originPosition = position;
        _inStartPos = new Vector3(_originPosition.x + _inOffsetX, _originPosition.y, _originPosition.z);
        _outEndPos = new Vector3(_originPosition.x, _originPosition.y + _outOffsetY, _originPosition.z);
        _transform.anchoredPosition = _inStartPos;
        ResetGoods();
    }

    private void Update()
    {
        if (_currentState == EGoodsState.Idle) 
            return;

        _animTime += Time.deltaTime;
        switch (_currentState)
        {            
            case EGoodsState.In:
                float inTime = (inDuration <= 0f) ? 1f : Mathf.Clamp01(_animTime / inDuration);
                float inEased = EaseOutCubic(inTime);
                _transform.anchoredPosition = Vector2.LerpUnclamped(_inStartPos, _originPosition, inEased);
                _rewardBg.color = new Color(1f, 1f, 1f, Mathf.LerpUnclamped(0f, 1f, inEased));
                _rewardImage.color = new Color(1f, 1f, 1f, Mathf.LerpUnclamped(0f, 1f, inEased));

                if (inTime >= 1f) EnterHold();
                break;                
            case EGoodsState.Hold:
                if (_animTime >= holdDuration) 
                    EnterOut();
                break;
            case EGoodsState.Out:
                float outTime = (outDuration <= 0f) ? 1f : Mathf.Clamp01(_animTime / outDuration);
                float outEased = EaseInCubic(outTime);

                _transform.anchoredPosition = Vector2.LerpUnclamped(_originPosition, _outEndPos, outEased);
                _rewardBg.color = new Color(1f, 1f, 1f, Mathf.LerpUnclamped(1f, 0f, outEased));
                _rewardImage.color = new Color(1f, 1f, 1f, Mathf.LerpUnclamped(1f, 0f, outEased));

                if (outTime >= 1f) Finish();
                break;
            default:
                break;
        }
    }

    public void Play()
    {
        gameObject.SetActive(true);        
        _currentState = EGoodsState.In;
    }

    private void EnterHold()
    {
        _currentState = EGoodsState.Hold;
        _animTime = 0f;

        // Hold에서는 end 위치/alpha 고정
        _transform.anchoredPosition = _originPosition;
        _rewardBg.color = Color.white;
        _rewardImage.color = Color.white;
    }

    private void EnterOut()
    {
        _currentState = EGoodsState.Out;
        _animTime = 0f;
        
    }

    private void Finish()
    {
        // 끄기 전에 리셋
        ResetGoods();
        _onRewardItemGetEffectPlayCallback?.Invoke();
    }

    private void ResetGoods()
    {
        _rewardBg.color = new Color(1f, 1f, 1f, 0f);
        _rewardImage.color = new Color(1f, 1f, 1f, 0f);
        _currentState = EGoodsState.Idle;
        _animTime = 0f;
        gameObject.SetActive(false);
    }


}
