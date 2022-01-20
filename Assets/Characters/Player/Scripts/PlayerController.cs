using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Standalone variant of the Grid-based movement
/// (works without Character class)
/// </summary>
public class PlayerController : MonoBehaviour
{
    public float moveSpeed;
    private bool isMoving;
    private Vector2 input;
    public LayerMask solidObjectsLayer;


    void Start()
    {
    }

    void Update()
    {
        if(!isMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");
            if (input != Vector2.zero)
            {
                var targetPos = transform.position;
                targetPos.x += input.x;
                targetPos.y += input.y;
                if (IsWalkable(targetPos))
                StartCoroutine(Move(targetPos));
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Damage Player");
            //TakeDamage(20);
        }
    }

    /// <summary>
    /// Checks if the targetPos is a valid position (nothing is blocking the way)
    /// </summary>
    /// <param name="targetPos"></param>
    /// <returns>Returns True, if targetPos is valid.</returns>
    private bool IsWalkable(Vector3 targetPos)
    {
       if (Physics2D.OverlapCircle(targetPos, 0.3f, solidObjectsLayer) != null)
       {
           return false;
       } 
       return true;
    }

    /// <summary>
    /// Moves the Player to the target position
    /// </summary>
    /// <param name="targetPos"></param>
    /// <returns></returns>
    IEnumerator Move(Vector3 targetPos)
    {
        isMoving = true;
        while((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;
        isMoving = false;
    }
}
