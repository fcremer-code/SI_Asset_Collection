using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Is used to spawn new Healthbars. This Class also holds all current healthbars, so we can keep track of them.
/// This could be used to disable Healthbars which aren't currently visible for the Player
/// </summary>
public class HealthbarManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject healthbarPrefab;
    [SerializeField] private Healthbar playerUIHealthbar;

    [Header("Monitoring")]
    [SerializeField] private List<Healthbar> _Healthbars;

    void Start()
    {
        //spawn the player Healthbar on Start
        spawnHealthbar(GameManager.Instance.player);
    }

    /// <summary>
    /// Spawns a Healthbar for the given Character
    /// </summary>
    /// <param name="_spawnedBy"></param>
    /// <returns></returns>
    public Healthbar spawnHealthbar(Character _spawnedBy)
    {
        Healthbar _Healthbar = null;
        //Debug.Log(_spawnedBy + "tryes spawning HPbar");

        //if this somehow gets called by the Player Character, just set the Healthbar again (We don't need to spawn a Healthbar here, as it is set in the Scene already)
        if (_spawnedBy.characterType.Equals(CharacterType.player))
        {
            _spawnedBy.healthBar = playerUIHealthbar;
            _Healthbar = playerUIHealthbar;
        }
        //else we spawn a Healthbar and Set it up for the given Character
        else
        {
            GameObject _healthbarObject = Instantiate(healthbarPrefab, transform.position, Quaternion.identity);
            _healthbarObject.transform.name = "Healthbar " + _Healthbars.Count;
            _healthbarObject.transform.SetParent(this.transform);
            _healthbarObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            _Healthbar = _healthbarObject.GetComponent<Healthbar>();
            _Healthbars.Add(_Healthbar);
        }

        _Healthbar.ownerCharacter = _spawnedBy;
        return _Healthbar;
    }
}
