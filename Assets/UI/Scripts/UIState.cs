using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Definition of a UI State
/// </summary>
public class UIState: MonoBehaviour
{
    [SerializeField] private bool showMouseCurser = true;
    [SerializeField] private CursorLockMode lockMouseCurser = CursorLockMode.None;
    [SerializeField] private Texture2D specialMouseCurserTexture = null;
    [SerializeField] private Vector2 offsetForMouseCurserTexture = Vector2.zero;
    [Space]
    [SerializeField] public string mainMenuSceneName;
    [SerializeField] public string inGameSceneName;

    void Start()
    {
        Cursor.visible = showMouseCurser;
        Cursor.lockState = lockMouseCurser;
        Cursor.SetCursor(specialMouseCurserTexture, offsetForMouseCurserTexture, CursorMode.Auto);
    }

    void Update()
    {
        
    }


    public void LoadScene(string _sceneName)
    {
        if (string.IsNullOrEmpty(_sceneName))
        {
            Debug.LogError("LoadScene(string) failed, because the required parameter is empty or null!", this);
            return;
        }
        SceneManager.LoadScene(_sceneName, LoadSceneMode.Single);
    }

    #region BUTTONS
    public void RestartButton()
    {
        GameManager.Instance.ChangeGameState(GameState.loadGame);
    }

    public void MainMenuButton()
    {
        GameManager.Instance.ChangeGameState(GameState.mainMenu);
    }

    public void OptionsMenu()
    {
        //GameManager.Instance.ChangeGameState(GameState.mainMenu);
    }

    public void QuitGameButton()
    {
        GameManager.Instance.ChangeGameState(GameState.quitGame);
    }
    #endregion
}
