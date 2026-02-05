using System;
using UnityEngine;

public enum ECategory
{
    Characters,
    Weapons,
    Equipments,
    End
}

public class PopupWithPreview : BasePopup
{
    protected event Action<int> OnElementSelected;
    protected int currentElementIndex;

    public void OnSelectElement(int index)
    {
        currentElementIndex = index;
        OnElementSelected?.Invoke(index);
    }

    public virtual void AddListener(Action<int> callback) { OnElementSelected += callback; }
    public virtual void RemoveListener(Action<int> callback) { OnElementSelected -= callback; }
}
