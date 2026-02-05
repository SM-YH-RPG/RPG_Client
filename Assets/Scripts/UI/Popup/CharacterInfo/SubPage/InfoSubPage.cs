using System;
using UnityEngine;
using UnityEngine.UI;

public abstract class InfoSubPage : MonoBehaviour
{
    protected int _selectedElementIndex;
    protected IPreview _preview;
    protected event Action _UpdatecharacterInfo;

    [SerializeField]
    protected RawImage _3DImage;

    [SerializeField]
    protected Image _2DImage;

    [SerializeField]
    private GameObject _currentEquipmentListObject;

    public virtual void Initialize(PopupWithPreview popupWithPreview)
    {
        popupWithPreview.AddListener(HandleElementSelectIndexChanged);
        popupWithPreview.AddListener(HandleElementSelectionChanged);
    }

    public void SetPreviewImage()
    {
        bool is3D = _preview is Preview3D;
        _3DImage.enabled = is3D;
        _2DImage.enabled = !is3D;
        _currentEquipmentListObject.SetActive(!is3D);
    }

    protected void HandleElementSelectionChanged(int index)
    {        
        _preview?.UpdatePreviewObject(index);
        _UpdatecharacterInfo?.Invoke();
    }

    protected virtual void HandleElementSelectIndexChanged(int index)
    {
        _selectedElementIndex = index;
    }

    protected abstract void SetupPreview();
}
