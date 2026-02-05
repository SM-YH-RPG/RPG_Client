using UnityEngine;

public class Preview3D : IPreview
{
    private PreviewController _previewCtrl;

    public Preview3D(Camera camera)
    {
        _previewCtrl = camera.GetComponent<PreviewController>();
    }

    public void UpdatePreviewObject(int index)
    {        
        _previewCtrl?.ChangePreviewCharacter(index);
    }
}
