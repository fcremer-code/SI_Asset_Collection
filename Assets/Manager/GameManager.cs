using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;


/// <summary>
/// Definition of all available Gamestates
/// </summary>
public enum GameState
{
    startGame,
    mainMenu,
    optionsMenu,
    playMode,
    pauseGame,
    died,
    credits,
    loadGame,
    quitGame
}

/// <summary>
/// Manages Gamestates and holds all other Managers (Singleton)
/// </summary>
public class GameManager : MonoBehaviour
{
    #region SINGLETON PATTERN
    public static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<GameManager>();

                if (_instance == null)
                {
                    GameObject container = new GameObject("GameManager");
                    _instance = container.AddComponent<GameManager>();
                }
            }
            return _instance;
        }
    }
    #endregion

    [Header("References")]
    [SerializeField] private string inGameSceneName;
    [SerializeField] private string mainMenuSceneName;
    [SerializeField] public TopDownCameraController topDownCameraController = null;
    [SerializeField] private GameObject playerPrefab = null;
    [SerializeField] private Transform characters = null;
    [SerializeField] public Tilemap tilemap = null;
    [SerializeField] internal GameObject bulletPrefabObj = null;
    [SerializeField] internal Transform bulletContainer = null;
    [Space]

    [Header("Monitoring")]
    [SerializeField] public GameState currendGameState = GameState.playMode;
    [SerializeField] public GameState previousGameState = GameState.playMode;
    [SerializeField] public bool gameIsPaused = false;
    [SerializeField] public PlayerCharacter player;

    void Awake()
    {
        if (inGameSceneName == null || inGameSceneName == "")
        {
            Debug.LogError("<inGameSceneName> is required to load the Game Scene!", this);
        }
        if (mainMenuSceneName == null || mainMenuSceneName == "")
        {
            Debug.LogError("<mainMenuSceneName> is required to load the Game Scene!", this);
        }

        //spawn the Player, if the Active Scene is the inGameScene
        if (SceneManager.GetActiveScene().name == inGameSceneName)
        {
            player = Instantiate(playerPrefab, characters).GetComponent<PlayerCharacter>();
        }
    }

    private void Start()
    {
        //switch Gamestate to Playmode, if the active Scene is the inGameScene
        if (SceneManager.GetActiveScene().name == inGameSceneName)
        {
            ChangeGameState(GameState.playMode);
        }
        //else set the Gamestate to MainMenu
        else if (SceneManager.GetActiveScene().name == mainMenuSceneName)
            ChangeGameState(GameState.mainMenu);
    }

    void Update()
    {
        if (currendGameState == GameState.loadGame)
        {
            if (SceneManager.GetSceneByName(inGameSceneName).isLoaded)
            {
                ChangeGameState(GameState.playMode);
            }
        }
    }

    /// <summary>
    /// switch the Gamestate and adjust the Settings for the other Managers (depending on Gamestate)
    /// </summary>
    /// <param name="_newGameState"></param>
    public void ChangeGameState(GameState _newGameState)
    {
        previousGameState = currendGameState;
        currendGameState = _newGameState;

        switch (_newGameState)
        {
            case GameState.mainMenu:
                PauseGame(false);

                if (SceneManager.GetActiveScene().name != mainMenuSceneName)
                {
                    LoadSceneByName(mainMenuSceneName);
                    break;
                }
                UIManager.Instance.ChangeUIState(UI_State.mainMenu);
                break;
            case GameState.optionsMenu:
                PauseGame(true);
                UIManager.Instance.ChangeUIState(UI_State.none);
                break;
            case GameState.playMode:
                PauseGame(false);
                UIManager.Instance.ChangeUIState(UI_State.inGame);
                break;
            case GameState.pauseGame:
                PauseGame(true);
                UIManager.Instance.ChangeUIState(UI_State.pauseMenu);
                break;
            case GameState.died:
                PauseGame(false);
                UIManager.Instance.ChangeUIState(UI_State.deathScreen);

                if (topDownCameraController != null)
                    topDownCameraController.playerInputDisabled = true;
                break;
            case GameState.credits:
                PauseGame(false);
                UIManager.Instance.ChangeUIState(UI_State.credits);
                break;
            case GameState.loadGame:
                PauseGame(false);
                UIManager.Instance.ChangeUIState(UI_State.loadGame);

                LoadSceneByName(inGameSceneName);
                break;
            case GameState.quitGame:
                PauseGame(false);
                UIManager.Instance.ChangeUIState(UI_State.none);

                Application.Quit();
                break;
            default:
                Debug.LogError("Tried to switch to invalid or unused Gamestate! Input: " + _newGameState, this);
                break;
        }

        Debug.Log("Changed GameState to: " + currendGameState);
    }


    /// <summary>
    /// Load a Scene by its name
    /// </summary>
    /// <param name="_SceneName"></param>
    private void LoadSceneByName(string _SceneName)
    {
        Debug.Log("Load Scene By Name: " + _SceneName);
        SceneManager.LoadScene(_SceneName, LoadSceneMode.Single);
    }

    /// <summary>
    /// Pause/Start the Game. The Timescale will be set to 0, when pausing a Game
    /// </summary>
    /// <param name="_Pause"></param>
    private void PauseGame(bool _Pause)
    {
        if (_Pause == gameIsPaused)
            return;

        if (_Pause)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }

        gameIsPaused = _Pause;
    }
}
