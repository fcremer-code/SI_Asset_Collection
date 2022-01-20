using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterType { player, enemy, npc };


public class Character : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private string characterDisplayedName;
    [SerializeField] internal float moveSpeed = 1f;
    [SerializeField ] public float maxHealth = 100f;
    [SerializeField] private float despawnTime = 3;

    [Header("Monitoring")]
    [SerializeField] public CharacterType characterType;
    [SerializeField] internal float currendHealth = 0;
    [SerializeField] internal bool isDead = false;
    [SerializeField] internal bool isMoving = false;

    [Header("References")]
    [SerializeField] internal SpriteRenderer spriteRenderer;
    [SerializeField] internal Healthbar healthBar = null;

    Coroutine healthbarTransitionCorou;

    protected virtual void Awake()
    {
        //spawn a healthbar when the GameObject awakes
        if (spriteRenderer == null)
            Debug.LogError("<SpriteRenderert> == null", this);
        healthBar = ((UIManager.Instance.UIStates[1])as UIState_InGame).healthbarManager.spawnHealthbar(this);
    }

    protected virtual void Start()
    {
        //set the Character health to full, when the Character gets spawned
        SetFullHealth();
    }

    /// <summary>
    /// move to the target Position
    /// </summary>
    /// <param name="targetPos"></param>
    /// <returns></returns>
    internal IEnumerator Move(Vector3 targetPos)
    {
        isMoving = true;
        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;
        isMoving = false;
    }

    /// <summary>
    /// calls TakeDamage() on the target Character
    /// </summary>
    /// <param name="_damageChar"></param>
    /// <param name="_damage"></param>
    public virtual void DealDamage(Character _damageChar, float _damage)
    {
        _damageChar.TakeDamage(_damage);
    }

    /// <summary>
    /// removes the Amount of Damage from the currentHealth <br />
    /// Calls a Coroutine to smooth the health transition. <br />
    /// Calls Die(), once the currentHealth reaches 0
    /// </summary>
    /// <param name="_DamageTaken"></param>
    public void TakeDamage(float _DamageTaken)
    {
        if (isDead)
            return;

        currendHealth -= _DamageTaken;

        if (currendHealth < 0)
        {
            currendHealth = 0;
            Die();
        }

        if (healthbarTransitionCorou != null)
            StopCoroutine(healthbarTransitionCorou);
        healthbarTransitionCorou = StartCoroutine(healthBar.HealthTransition((int)currendHealth));
    }

    /// <summary>
    /// Resets the currentHealth to maxHealth
    /// </summary>
    public void SetFullHealth()
    {
        currendHealth = maxHealth;
        healthBar.SetMaxHealth((int)maxHealth);
        healthBar.SetCurrendHealth((int)currendHealth);
    }

    /// <summary>
    /// Shoot a bullet in the target direction <br />
    /// The bullet gets spawned as a new Gameobject and the bulletContainer will be set
    /// as the parent of the bullet, so we can keep track of all current bullets in the Scene.
    /// </summary>
    /// <param name="_directionZEuler"></param>
    /// <param name="_speed"></param>
    /// <param name="_range"></param>
    /// <param name="_damage"></param>
    internal void ShootBullet(float _directionZEuler, float _speed, float _range, float _damage)
    {
        GameObject _newBullet = Instantiate(GameManager.Instance.bulletPrefabObj, transform);
        _newBullet.transform.SetParent(GameManager.Instance.bulletContainer, true);
        _newBullet.transform.GetComponent<Bullet>().Init(_directionZEuler, characterType, _speed, _range, _damage);
    }


    /// <summary>
    /// Sets isDead to true
    /// <br />
    /// Destroys the healthbar gameObject
    /// <br />
    /// Starts the Despawn Timer
    /// </summary>
    public virtual void Die()
    {
        isDead = true;
        Destroy(healthBar.gameObject);
        StartCoroutine(DespawnTimer(despawnTime));
    }

    /// <summary>
    /// Waits for the given time "_Time" and deactivate the Gameobject afterwards.
    /// </summary>
    /// <param name="_Time"></param>
    /// <returns></returns>
    private IEnumerator DespawnTimer(float _Time)
    {
        yield return new WaitForSeconds(_Time);

        gameObject.SetActive(false);
    }
}
