using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class CharacterElement : MonoBehaviour
{
    [SerializeField]
    private Button elementButton;

    [SerializeField]
    private Image thumbnail;

    [SerializeField]
    private Image selectOutline;

    private int index;

    public async UniTaskVoid Init(CharacterData _data, Action<int> _onElementClick)
    {
        index = _data.Index;

        var requester = Resources.LoadAsync<Sprite>(_data.ImagePath);
        await UniTask.WaitUntil(() => requester.isDone);

        Sprite thumbnailSprite = requester.asset as Sprite;
        thumbnail.sprite = thumbnailSprite;

        elementButton.onClick.AddListener(() =>
        {
            _onElementClick?.Invoke(_data.Index);
        });
    }

    public int GetIndex()
    {
        return index;
    }

    public void EnableOutline(bool _isEnable)
    {
        selectOutline.enabled = _isEnable;
    }
}