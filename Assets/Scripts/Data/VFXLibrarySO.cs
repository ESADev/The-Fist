using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Serializable data container mapping keys to visual effect prefabs.
/// </summary>
[System.Serializable]
public class VisualEffect
{
    /// <summary>
    /// Unique identifier used to reference this visual effect.
    /// </summary>
    public string key;

    /// <summary>
    /// Prefab to spawn for this effect.
    /// </summary>
    public GameObject effectPrefab;
}

/// <summary>
/// Library asset holding references to all visual effects used by the game.
/// </summary>
[CreateAssetMenu(menuName = "VFX/VFX Library", fileName = "VFXLibrarySO")]
public class VFXLibrarySO : ScriptableObject
{
    [Header("Visual Effects")]
    [Tooltip("List of visual effects keyed by unique identifiers.")]
    public List<VisualEffect> effects = new List<VisualEffect>();
}
