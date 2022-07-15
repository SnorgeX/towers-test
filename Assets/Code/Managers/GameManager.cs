using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
public class GameManager : MonoBehaviour
{
    public enum GameState { Pause, Game, Lose, MainMenu }

    [SerializeField] private UIManager _uiManager;

    [SerializeField] private TowerView _towerMalePrefab;
    [SerializeField] private TowerView _towerFemalePrefab;
    [SerializeField] private CannonConfiguration _startCannon;

    [SerializeField] private float _distanceInUnits;
    private float modelSize;

    private List<TowerView> _towerFirstTeam;
    private List<TowerView> _towerSecondTeam;

    private bool _isPaused;
    public static event Action<GameState> OnGameStateChanged;
    public GameState State;

    public static GameManager Instance;

    private void Awake()
    {
        Instance = this;
        _towerFirstTeam = new List<TowerView>();
        _towerSecondTeam = new List<TowerView>();
        modelSize = _towerMalePrefab.gameObject.GetComponent<SpriteRenderer>().bounds.size.x;
    }
    public void UpdateState(GameState newGameState)
    {
        if (newGameState == State)
        {
            return;
        }
        if (State == GameState.Pause)
        {
            _isPaused = false;
            Time.timeScale = 1f;
        }

        State = newGameState;
        switch (newGameState)
        {
            case GameState.Pause:
                HandlePauseScreen();
                break;
            case GameState.Game:
                HandleStartGame();
                break;
            case GameState.Lose:
                HandleLoseScreen();
                break;
            case GameState.MainMenu:
                HandleMainMenu();
                break;
        }
        OnGameStateChanged?.Invoke(newGameState);
    }

    private void HandlePauseScreen()
    {
        _isPaused = true;
        Time.timeScale = 0f;
    }

    private void HandleStartGame()
    {

    }

    private void HandleLoseScreen()
    {

    }

    private void HandleMainMenu()
    {
        BuildStartTowers();
    }

    private void Start()
    {
        UpdateState(GameState.Game);
        StartNewGame();
        Camera.main.orthographicSize = (_distanceInUnits + modelSize) * Screen.height / Screen.width * 0.5f;
        Camera.main.transform.position = new Vector3(0, Camera.main.orthographicSize, -3);
    }

    private void CreateTowerLeft()
    {
        var bar = (_uiManager.CreateBar());
        _uiManager.leftBarContainer.Add(bar);

        var tower = CreateTower(new Vector3(-_distanceInUnits / 2, 0, 0), _towerFemalePrefab, bar);
        _towerFirstTeam.Add(tower);


        var shieldButton = (_uiManager.CreateShieldButton(_uiManager.leftShieldContainer));

        tower.CreateShield(new Vector3(-_distanceInUnits / 2 + modelSize, 0, 0), shieldButton);
    }

    private void CreateTowerRight()
    {
        var bar = (_uiManager.CreateBar());
        var tower = CreateTower(new Vector3(_distanceInUnits / 2, 0, 0), _towerMalePrefab, bar);
        _towerSecondTeam.Add(tower);
        _uiManager.rightBarContainer.Add(bar);

        var shieldButton = (_uiManager.CreateShieldButton(_uiManager.rightShieldContainer));
        shieldButton.parent.style.display = DisplayStyle.None;
        tower.CreateShield(new Vector3(_distanceInUnits / 2 - modelSize, 0, 0), shieldButton);
    }

    private void CreatePlayerCannon(TowerView tower)
    {
        tower.CreateCannon(new CannonModel(_startCannon), new PlayerInput());
    }
    private void CreateAICannon(TowerView tower, List<TowerView> enemyTowers)
    {
        tower.CreateCannon(new CannonModel(_startCannon), new AIInput(enemyTowers[0].transform.position, 50f));
    }
    private TowerView CreateTower(Vector3 position, TowerView prefab, VisualElement bar)
    {
        var newtower = Instantiate(prefab);
        newtower.Init(30, position, bar);
        return newtower;
    }

    private void BuildStartTowers()
    {
        if (_towerFirstTeam.Count == 0)
        {
            CreateTowerLeft();
        }
        if (_towerSecondTeam.Count == 0)
        {
            CreateTowerRight();
        }
    }
    public void DestroyTowers()
    {
        if (_towerFirstTeam.Count > 0)
        {
            foreach (var tower in _towerFirstTeam)
            {
                Destroy(tower.gameObject);
            }
            _towerFirstTeam.Clear();
        }

        if (_towerSecondTeam.Count > 0)
        {
            foreach (var tower in _towerSecondTeam)
            {
                Destroy(tower.gameObject);
            }
            _towerSecondTeam.Clear();
        }
    }
    public void StartNewGame()
    {
        DestroyTowers();
        BuildStartTowers();

        CreatePlayerCannon(_towerFirstTeam[0]);
        CreateAICannon(_towerSecondTeam[0], _towerFirstTeam);
    }
}
