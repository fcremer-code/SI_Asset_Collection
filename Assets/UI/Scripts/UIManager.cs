using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Definition of all available UI states
/// </summary>
public enum UI_State {
    none = 0, 
    inGame = 1, 
    mainMenu = 2, 
    pauseMenu = 3, 
    credits = 4,
    deathScreen = 5,
    loadGame = 6
};

/// <summary>
/// Manages the UI states (Singleton)
/// </summary>
public class UIManager : MonoBehaviour
{
    //Singelton
    #region SINGLETON PATTERN
    public static UIManager _instance;
    public static UIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<UIManager>();

                if (_instance == null)
                {
                    GameObject container = new GameObject("UIManager");
                    _instance = container.AddComponent<UIManager>();
                }
            }
            return _instance;
        }
    }
    #endregion

    [Header("Monitoring")]
    [SerializeField] public UI_State currendUIState = 0;
    [SerializeField] public UI_State previousUIState = 0;
    [Space]
    [Header("References")]
    [Tooltip("Link the UIStates in the order of the <UI_State>-array. When null, the UIState gets ignored.")]
    public UIState[] UIStates = new UIState[7];

    void Awake()
    {

    }

    void Update()
    {
        
    }

    /// <summary>
    /// Change the UI state to the given State. 
    /// There are multiple Gameobjects for the different UI States set up in the Demo Scene. 
    /// The Gameobjects will be set active and inactive, depending on the current state.
    /// </summary>
    /// <param name="newUIState"></param>
    public void ChangeUIState(UI_State newUIState)
    {
        if ((int)newUIState >= UIStates.Length)
        {
            return;
        }
        else if (UIStates[(int)newUIState] == null)
        {
            return;
        }

        previousUIState = currendUIState;
        UIStates[(int)currendUIState].gameObject.SetActive(false);
        currendUIState = newUIState;
    

        UIStates[(int)currendUIState].gameObject.SetActive(true);
    }
}
