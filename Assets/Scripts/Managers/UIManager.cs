using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public interface IUIManagerService
{
    UniTask<T> Show<T>() where T : BasePopup;
    void Hide();

    bool IsShow();

    event Action OnShowPopup;
    event Action OnHidePopup;
}

public class UIManager : LazySingleton<UIManager>, IUIManagerService
{
    #region CONST
    private const int SORT_ORDER_INDEX = 100;
    #endregion

    private Dictionary<Type, BasePopup> _cachedPopupDict = new Dictionary<Type, BasePopup>();

    private Stack<BasePopup> _popupStack = new Stack<BasePopup>();

    #region Action
    public event Action OnShowPopup;
    public event Action OnHidePopup;
    #endregion

    public async UniTask<T> Show<T>() where T : BasePopup
    {
        var type = typeof(T);
        _cachedPopupDict.TryGetValue(type, out var popup);
        if (popup == null)
        {
            popup = await CreatePopup<T>(type);
        }

        popup.SetSortingOrderLayer(SORT_ORDER_INDEX + _popupStack.Count);
        popup.Show();
        _popupStack.Push(popup);

        OnShowPopup?.Invoke();

        return popup as T;
    }

    public void Hide()
    {
        if (_popupStack.Count == 0)
            return;

        var popup = _popupStack.Pop();
        popup.Hide();

        if(_popupStack.Count == 0)
        {
            OnHidePopup?.Invoke();
        }

    }

    private async UniTask<T> CreatePopup<T>(Type type) where T : BasePopup
    {
        string path = $"Prefabs/UI/Popup/{type.Name}";

        var requester = Resources.LoadAsync<GameObject>(path);
        await UniTask.WaitUntil(() => requester.isDone);

        if(requester.asset == null)
        {
            throw new Exception($"Asset Load Failed : {path}.prefab");
        }

        GameObject prefab = requester.asset as GameObject;
        GameObject popupObject = GameObject.Instantiate(prefab);

        popupObject.TryGetComponent<T>(out var popup);
        _cachedPopupDict.Add(type, popup);

        return popup;
    }

    public bool IsShow()
    {
        return _popupStack.Count > 0;
    }

    public void CachedClear()
    {
        _cachedPopupDict.Clear();
    }
}
