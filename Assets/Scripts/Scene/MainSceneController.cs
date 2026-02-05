using UnityEngine;
using UnityEngine.UI;

public class MainSceneController : MonoBehaviour
{
    private IUIManagerService _uiManager => UIManager.Instance;

    [SerializeField]
    private Button startButton = null;

    [SerializeField]
    private Button achivementButton = null;

    [SerializeField]
    private Button cashShopButton = null;

    private void Awake()
    {
        startButton.onClick.AddListener(OnClickStartButton);
        achivementButton.onClick.AddListener(OnClickAchivementButton);
    }

    private void OnDestroy()
    {
        startButton.onClick.RemoveListener(OnClickStartButton);
        achivementButton.onClick.RemoveListener(OnClickAchivementButton);
    }

    private void OnClickStartButton()
    {
        _uiManager.Show<CharacterSelectPopup>();
    }

    private void OnClickAchivementButton()
    {
        _uiManager.Show<AchivementPopup>();
    }
}