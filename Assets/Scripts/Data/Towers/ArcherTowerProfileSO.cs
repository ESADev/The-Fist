using UnityEngine;

/// <summary>
/// Data driving an archer tower's modifications to the archer unit placed on it.
/// </summary>
[CreateAssetMenu(fileName = "ArcherTowerProfile", menuName = "TheFist/Towers/Archer Tower Profile")]
public class ArcherTowerProfileSO : ScriptableObject
{
    [Header("Archer Modifiers")]
    [Tooltip("Damage multiplier applied to every attack of the archer occupying this tower (1 = no change).")]
    public float archerDamageMultiplier = 1f;

    [Tooltip("Cooldown multiplier applied to every attack of the archer ( < 1 = faster, 1 = no change, > 1 = slower ).")]
    public float archerCooldownMultiplier = 1f;
}
