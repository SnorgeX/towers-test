using UnityEngine;
using UnityEngine.UIElements;
public class UIManager : MonoBehaviour
{
    [SerializeField] private VisualTreeAsset BarDocument;
    [SerializeField] private VisualTreeAsset ShieldButtonDocument;

    public VisualElement leftBarContainer;
    public VisualElement rightBarContainer;

    public VisualElement leftShieldContainer;
    public VisualElement rightShieldContainer;

    private TemplateContainer _pauseMenu;
    private Button _pauseButton;
    private Button _restartButton;
    private Button _resumeButton;
    private void OnEnable()
    {
        var rootVisualElement = GetComponent<UIDocument>().rootVisualElement;

        leftBarContainer = rootVisualElement.Q("LeftBars");
        rightBarContainer = rootVisualElement.Q("RightBars");

        leftShieldContainer = rootVisualElement.Q("LeftShieldButtons");
        rightShieldContainer = rootVisualElement.Q("RightShieldButtons");

        _pauseButton = rootVisualElement.Q<Button>("pause-button");
        _pauseButton.clicked += PauseButtonPressed;
        _pauseMenu = rootVisualElement.Q<TemplateContainer>("PauseUI");

        _restartButton = rootVisualElement.Q<Button>("restart-button");
        _restartButton.clicked += RestartButtonPressed;

        _resumeButton = rootVisualElement.Q<Button>("resume-button");
        _resumeButton.clicked += ResumeButtonPressed;


    }
    
    private void PauseButtonPressed()
    {
        _pauseMenu.style.display = DisplayStyle.Flex;
        GameManager.Instance.UpdateState(GameManager.GameState.Pause);
    }
    private void RestartButtonPressed()
    {
        _pauseMenu.style.display = DisplayStyle.None;
        GameManager.Instance.StartNewGame();
        GameManager.Instance.UpdateState(GameManager.GameState.Game);
    }
    private void ResumeButtonPressed()
    {
        _pauseMenu.style.display = DisplayStyle.None;
        GameManager.Instance.UpdateState(GameManager.GameState.Game);
    }
    public VisualElement CreateBar()
    {
        var barContainer = BarDocument.CloneTree();
        barContainer.name = "barContainer";
        barContainer.style.flexGrow = 1;
        return barContainer;
    }

    public void AddBar(VisualElement container)
    {
        var barContainer = CreateBar();
        container.Add(barContainer);
    }

    public Button CreateShieldButton(VisualElement parent)
    {
        var buttonContainer = ShieldButtonDocument.CloneTree();
        buttonContainer.name = "buttonContainer";
        buttonContainer.style.flexGrow = 1;
        parent.Add(buttonContainer);
        return buttonContainer.Q<Button>("shield-button");
    }
    private void OnDestroy()
    {
        _pauseButton.clicked -= PauseButtonPressed;
        _restartButton.clicked -= RestartButtonPressed;
        _resumeButton.clicked -= ResumeButtonPressed;
    }
}
