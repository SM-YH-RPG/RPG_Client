using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectPopup : BasePopup
{
    #region Inspector
    [Header("SelectView")]
    [SerializeField]
    private Transform root;

    [SerializeField]
    private CharacterElement elementTemplate;

    [Header("Preview")]
    [SerializeField]
    private CharacterPreview preview;

    [Header("Button")]
    [SerializeField]
    private Button closeButton;

    [SerializeField]
    private Button selectButton;
    #endregion

    private CharacterElement[] elements;
    private CharacterElement selectedElement;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        closeButton.onClick.AddListener(OnClickCloseButton);
        selectButton.onClick.AddListener(OnClickSelectButton);


        preview.Clear();

        //var datas = CharacterTable.Instance.GetCharacterDatas();
        //CreateElementList(datas);

        elementTemplate.gameObject.SetActive(false);
    }

    private void CreateElementList(List<CharacterData> datas)
    {
        int dataCount = datas.Count;

        elements = new CharacterElement[dataCount];
        for (int i = 0; i < dataCount; i++)
        {
            elements[i] = CreateElement(datas[i]);
        }
    }

    private CharacterElement CreateElement(CharacterData data)
    {
        var element = Instantiate(elementTemplate, root);
        element.Init(data, (int slotIndex) =>
        {
            if (selectedElement != null)
            {
                selectedElement.EnableOutline(false);
            }

            element.EnableOutline(true);
            selectedElement = element;

            preview.OnUpdate(data);
        }).Forget();

        return element;
    }

    private void OnClickCloseButton()
    {
        UIManager.Instance.Hide();
    }

    private async void OnClickSelectButton()
    {
        if (selectedElement == null)
            return;

        var popup = await UIManager.Instance.Show<ConfirmPopup>();
        popup.SetContent("캐릭터 선택", "", () =>
        {
            SceneManager.Instance.ChangeScene("InGameScene");
        });
    }

    public override void Hide()
    {
        base.Hide();

        preview.Clear();
    }
}