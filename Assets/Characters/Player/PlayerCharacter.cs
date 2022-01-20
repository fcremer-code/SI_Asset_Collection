using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


/// <summary>
/// Player Character Class for Grid-based Movement
/// </summary>
public class PlayerCharacter : Character
{
    [Header("Player: Monitoring")]
    [SerializeField] private Vector2 input;

    public Tilemap obstacles;


    protected override void Awake()
    {
        //get the obstacle Map from the Gamemanager (we need this for the grid-based Player Movement)
        obstacles = GameManager.Instance.tilemap;

        //Set Charactertype to Player, so we can check the Type of a Character
        characterType = CharacterType.player;
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
    }

    void Update()
    {
        //-----------------------------MOVEMENT----------------------------------------------
        //Only check the movement Input, if the Player is currently not moving and not dead.
        //Because of the grid-based movement, we don't have to check the Input while the Player moves from one Cell to another        
        if (!isMoving && !isDead)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            //if the Input is not zero, move the Player to the target position
            if(input != Vector2.zero)
            {
                //Calculate the target Position with the x and y Value of the Input.
                var targetPos = transform.position;
                targetPos.x += input.x;
                targetPos.y += input.y;

                //Check if there are any obstacles on our target Position. If there are no obstacles, Start the movement Coroutine
                Vector3Int obstacleMap = obstacles.WorldToCell(targetPos);
                if (obstacles.GetTile(obstacleMap) == null)
                {
                    StartCoroutine(Move(targetPos));
                }
            }
        }
        //--------------------------ATTACK--------------------------------------------------
        //Check if Spacebar or Leftclick is pressed down
        //If true, calculate target Position with the position of the Cursor and shoot a bullet. 
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            Vector2 _mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            float _shootDir = Mathf.Atan2(_mousePos.y, _mousePos.x) * Mathf.Rad2Deg;
            ShootBullet(_shootDir, 5, 7, 5);
            Debug.Log("shootDir: " + _shootDir);
        }
    }

    /// <summary>
    /// calls the Die() method of the Character base class.
    /// <br />
    /// sets the game state to "died"
    /// </summary>
    public override void Die()
    {
        base.Die();
        GameManager.Instance.ChangeGameState(GameState.died);
    }
}
