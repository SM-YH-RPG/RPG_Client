using UnityEngine;

public class RewardItemList : MonoBehaviour
{
    [SerializeField] private RewardItemElement[] _getRewardItemElementArray;
    [SerializeField] private float _dropItemDelayTime = 0.2f;

    private float[] _goodsOffsetYArray;
    private float _delayTime;
    private int _offsetYIndex;

    private void Awake()
    {
        int count = _getRewardItemElementArray.Length;
        _goodsOffsetYArray = new float[count];
        for (int i = 0; i < count; i++)
        {
            _goodsOffsetYArray[i] = _getRewardItemElementArray[i].GetOriginPositionY();
            _getRewardItemElementArray[i].SetOriginPosition(new Vector3(0f, _goodsOffsetYArray[i], 0f));
            _getRewardItemElementArray[i].SetRewardItemGetEffectPlayCallback(RewardItemDequeueAndShow);
        }
        _delayTime = 0f;
        RewardItemManager.Instance.OnAddRewardItemEffectStart += RewardItemDequeueAndShow;
    }

    private void Update()
    {
        if (_delayTime <= 0f)
            return;

        _delayTime -= Time.deltaTime;
        if (_delayTime <= 0f)
        {
            _delayTime = 0f;
            RewardItemDequeueAndShow();
        }
    }

    private void OnDestroy()
    {
        RewardItemManager.Instance.OnAddRewardItemEffectStart -= RewardItemDequeueAndShow;
    }

    public void RewardItemDequeueAndShow()
    {
        if (_delayTime > 0f)
            return;

        foreach (var goods in _getRewardItemElementArray)
        {
            if (goods.CurrentState != RewardItemElement.EGoodsState.Idle)
            {
                _offsetYIndex++;
            }
            else
            {
                if (RewardItemManager.Instance.GetRewardItemCount() != 0)
                {
                    goods.UpdateGetRewardData(RewardItemManager.Instance.GetRewardItemData());
                    goods.SetOriginPosition(new Vector3(0f, _goodsOffsetYArray[_offsetYIndex], 0f));
                    goods.Play();
                    _delayTime = _dropItemDelayTime;
                    break;
                }
            }
        }
        _offsetYIndex = 0;
    }
}
