using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enemy Character class
/// requires Character Class to work
/// </summary>
public class EnemyCharacter : Character
{
    [Header("Settings")]
    [SerializeField] private Vector2 damageRange = new Vector2(5, 7);
    [Header("Monitoring")]
    [SerializeField] internal Character target = null;
    [SerializeField] private float damageCooldownTime = 1;
    [SerializeField] private bool cooldown = false;


    private Coroutine damageCooldownCorou;

    protected override void Awake()
    {
        //call the Awake function of the base Character Class
        base.Awake();
        //set the Character Type to "enemy"
        characterType = CharacterType.enemy;
    }

    protected override void Start()
    {
        //call the Start function of the base Character Class
        base.Start();
    }

    void Update()
    {
        //if the enemy Character has a target and is not dead, move towards the target
        if (target != null && !isDead)
        {
            isMoving = true;
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, moveSpeed * Time.deltaTime);
        }
        else
        {
            isMoving = false;
        }
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        //if the enemys range collider collides with the Player, set the target of the Enemy
        if (collision.transform.CompareTag("Player"))
        {
            Debug.Log(collision.transform.gameObject.name);
            target = collision.GetComponent<PlayerCharacter>();
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        //if the enemys collider collides with the Player, deal damage to the Player.         
        if (collision.transform.TryGetComponent(out PlayerCharacter _playerCharacter))
        {
            //if the enemy still has cooldown, don't deal any damage to the Player
            if (cooldown == false)
            {                
                cooldown = true;
                DealDamage(_playerCharacter, Random.Range(damageRange.x, damageRange.y));

                //start cooldown Coroutine
                damageCooldownCorou = StartCoroutine(DealDamageCooldown());
            }
        }
    }

    /// <summary>
    /// Starts the Cooldown. Sets Cooldown to false after a set time
    /// </summary>
    /// <returns></returns>
    private IEnumerator DealDamageCooldown()
    {
        yield return new WaitForSeconds(damageCooldownTime);
        cooldown = false;
    }


    /// <summary>
    /// deals a specific amount of Damage to another Character
    /// </summary>
    /// <param name="_damageChar"></param>
    /// <param name="_damage"></param>
    public override void DealDamage(Character _damageChar, float _damage)
    {
        _damage = Random.Range(damageRange.x, damageRange.y);
        base.DealDamage(_damageChar, _damage);
    }
}
