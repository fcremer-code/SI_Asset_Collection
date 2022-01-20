using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Healthbar Class. 
/// requires the Character base Class
/// </summary>
public class Healthbar : MonoBehaviour
{
    [Header("References")]
    public Slider healthBarSlider;

    [Header("Monitoring")]
    public Character ownerCharacter;

    private void Init(Character _ownerCharacter)
    {
        ownerCharacter = _ownerCharacter;
    }

    private void Update()
    {
        //Only update the position of the Enemy Healthbars, as the Players Healthbar should be in a fixed Location on the Screen
        if (!ownerCharacter.characterType.Equals(CharacterType.player))
            UpdateHPbarPosition();   
    }

    /// <summary>
    /// Sets the value of the Healthbar UI
    /// </summary>
    /// <param name="_hp"></param>
    public void SetCurrendHealth(int _hp)
    {
        healthBarSlider.value = _hp;
    }

    /// <summary>
    /// Sets the Healthbar UI value to max
    /// </summary>
    /// <param name="_hp"></param>
    public void SetMaxHealth(int _hp)
    {
        healthBarSlider.maxValue = _hp;
    }

    /// <summary>
    /// Smooth transition the current Health to the new health 
    /// </summary>
    /// <param name="newHealth"></param>
    /// <returns></returns>
    public IEnumerator HealthTransition(int newHealth)
    {
        float elapsedtime = 0;
        while (newHealth != healthBarSlider.value)
        {
            healthBarSlider.value = Mathf.Lerp(healthBarSlider.value, newHealth, (elapsedtime / 1));
            elapsedtime += Time.deltaTime;
            yield return null;
        }
    }

    /// <summary>
    /// Update the Healthbar position to the Position of the owner
    /// </summary>
    private void UpdateHPbarPosition()
    {
        //We need to calculate to position of the healthbar on the Screen from the owner Character Position, because the Healthbar is in the UI Canvas
        Vector3 pos = Camera.main.WorldToScreenPoint(ownerCharacter.transform.position);

        //Add a little extra height to the calculated position, so the Healthbar is not on the exact position as the Character
        pos.y += 60;

        //Then finally set the position of the Healthbar
        transform.position = pos;
    }
}