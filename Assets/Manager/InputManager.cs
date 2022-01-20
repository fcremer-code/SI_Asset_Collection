using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Input Manager Class (Singleton). Manages all kinds of Input for the Gamemanager
/// </summary>
public class InputManager : MonoBehaviour
{
    #region SINGLETON PATTERN
    public static InputManager _instance;
    public static InputManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<InputManager>();

                if (_instance == null)
                {
                    GameObject container = new GameObject("InputManager");
                    _instance = container.AddComponent<InputManager>();
                }
            }
            return _instance;
        }
    }
    #endregion

    void Start()
    {
       
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            EscButton();
    }

    private void EscButton()
    {
        switch (GameManager.Instance.currendGameState)
        {
            case GameState.playMode:
                GameManager.Instance.ChangeGameState(GameState.pauseGame);
                break;
            case GameState.pauseGame:
                GameManager.Instance.ChangeGameState(GameManager.Instance.previousGameState);
                break;
            case GameState.mainMenu:
            case GameState.died:
            case GameState.credits:
                break;
            default:
                break;
        }
    }
}
