using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Can be used to spawn Bullets
/// </summary>
public class Bullet : MonoBehaviour
{
    [SerializeField] private CharacterType owner = CharacterType.player;
    [SerializeField] private float speed = 1;
    [SerializeField] private float range = 6;
    [SerializeField] private float damage = 10;
    private Vector2 startPosition = Vector2.zero;

    /// <summary>
    /// Spawns a new bullet
    /// </summary>
    /// <param name="_directionEulerZ"></param>
    /// <param name="_owner"></param>
    /// <param name="_speed"></param>
    /// <param name="_range"></param>
    /// <param name="_damage"></param>
    internal void Init(float _directionEulerZ, CharacterType _owner, float _speed, float _range, float _damage)
    {
        owner = _owner;
        speed = _speed;
        range = _range;
        damage = _damage;
        startPosition = transform.position;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, _directionEulerZ));
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.right * Time.deltaTime * speed;

        if (Vector2.Distance(startPosition, new Vector2(transform.position.x, transform.position.y)) > range)
        {
            DestroyBullet();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("CollisionEnter", this);
        if (collision.transform.TryGetComponent(out Character _character))
        {
            if (!_character.characterType.Equals(owner))
            {
                _character.TakeDamage(damage);
                DestroyBullet();
            }
        }
        else
        {
            DestroyBullet();
        }
    }

    /// <summary>
    /// Destroy the bullet gameObject after a specific time
    /// </summary>
    /// <param name="_afterTime"></param>
    internal void DestroyBullet(float _afterTime = 0)
    {
        Destroy(gameObject, _afterTime);
    }
}
