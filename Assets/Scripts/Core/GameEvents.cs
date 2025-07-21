using System;
using UnityEngine;

/// <summary>
/// Central hub for global game events used to decouple communication
/// between independent systems.
/// </summary>
public static class GameEvents
{

    /// <summary>
    /// Fired when a unit receives damage.
    /// </summary>
    public static event Action<DamageInfo> OnUnitDamaged;

    /// <summary>
    /// Fired when a unit dies.
    /// </summary>
    public static event Action<GameObject> OnUnitDied;

    /// <summary>
    /// Fired when the player gains or loses resources.
    /// Parameters: the type of resource and the new amount.
    /// </summary>
    public static event Action<ResourceType, int> OnPlayerResourceChanged;

    /// <summary>
    /// Fired when the player achieves victory.
    /// </summary>
    public static event Action OnVictory;

    /// <summary>
    /// Fired when the player is defeated.
    /// </summary>
    public static event Action OnDefeat;

    /// <summary>
    /// Fired when the player's movement speed changes.
    /// Parameters: current speed, max speed.
    /// </summary>
    public static event Action<float, float> OnPlayerSpeedChanged;

    /// <summary>
    /// Fired when an object is upgraded.
    /// Parameters: the upgraded GameObject, the new level.
    /// </summary>
    public static event Action<GameObject, int> OnObjectUpgraded;

    /// <summary>
    /// Fired when an object is unlocked.
    /// Parameters: the unlocked GameObject.
    /// </summary>
    public static event Action<GameObject> OnObjectUnlocked;


    /// <summary>
    /// Invokes the <see cref="OnUnitDamaged"/> event.
    /// </summary>
    /// <param name="damageInfo">Information about the damage dealt.</param>
    public static void TriggerOnUnitDamaged(DamageInfo damageInfo)
    {
        Debug.Log("[GameEvents] Unit damaged");
        OnUnitDamaged?.Invoke(damageInfo);
    }

    /// <summary>
    /// Invokes the <see cref="OnUnitDied"/> event.
    /// </summary>
    /// <param name="unit">The unit GameObject that died.</param>
    public static void TriggerOnUnitDied(GameObject unit)
    {
        if (unit == null)
        {
            Debug.LogError("[GameEvents] TriggerOnUnitDied called with null unit.");
            return;
        }

        Debug.Log($"[GameEvents] Unit died: {unit.name}");
        OnUnitDied?.Invoke(unit);
    }

    /// <summary>
    /// Invokes the <see cref="OnPlayerResourceChanged"/> event.
    /// </summary>
    /// <param name="type">Type of resource affected.</param>
    /// <param name="amount">The player's new resource amount.</param>
    public static void TriggerOnPlayerResourceChanged(ResourceType type, int amount)
    {
        Debug.Log($"[GameEvents] Player resource changed: {type} -> {amount}");
        OnPlayerResourceChanged?.Invoke(type, amount);
    }

    /// <summary>
    /// Invokes the <see cref="OnVictory"/> event.
    /// </summary>
    public static void TriggerOnVictory()
    {
        Debug.Log("[GameEvents] Victory achieved");
        OnVictory?.Invoke();
    }

    /// <summary>
    /// Invokes the <see cref="OnDefeat"/> event.
    /// </summary>
    public static void TriggerOnDefeat()
    {
        Debug.Log("[GameEvents] Defeat occurred");
        OnDefeat?.Invoke();
    }

    /// <summary>
    /// Invokes the <see cref="OnPlayerSpeedChanged"/> event.
    /// </summary>
    /// <param name="currentSpeed">Current player movement speed.</param>
    /// <param name="maxSpeed">Maximum player movement speed.</param>
    public static void TriggerOnPlayerSpeedChanged(float currentSpeed, float maxSpeed)
    {
        Debug.Log($"[GameEvents] Player speed changed: {currentSpeed:F2}/{maxSpeed:F2}");
        OnPlayerSpeedChanged?.Invoke(currentSpeed, maxSpeed);
    }

    /// <summary>
    /// Invokes the <see cref="OnObjectUpgraded"/> event.
    /// </summary>
    /// <param name="upgradedObject">The GameObject that was upgraded.</param>
    /// <param name="newLevel">The new upgrade level.</param>
    public static void TriggerOnObjectUpgraded(GameObject upgradedObject, int newLevel)
    {
        if (upgradedObject == null)
        {
            Debug.LogError("[GameEvents] TriggerOnObjectUpgraded called with null object.");
            return;
        }

        Debug.Log($"[GameEvents] Object upgraded: {upgradedObject.name} -> Level {newLevel}");
        OnObjectUpgraded?.Invoke(upgradedObject, newLevel);
    }

    /// <summary>
    /// Invokes the <see cref="OnObjectUnlocked"/> event.
    /// </summary>
    /// <param name="unlockedObject">The GameObject that was unlocked.</param>
    public static void TriggerOnObjectUnlocked(GameObject unlockedObject)
    {
        if (unlockedObject == null)
        {
            Debug.LogError("[GameEvents] TriggerOnObjectUnlocked called with null object.");
            return;
        }

        Debug.Log($"[GameEvents] Object unlocked: {unlockedObject.name}");
        OnObjectUnlocked?.Invoke(unlockedObject);
    }
}
