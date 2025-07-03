using UnityEngine;

/// <summary>
/// ScriptableObject holding the core stats for any unit.
/// Enables data-driven balancing without code changes.
/// </summary>
[CreateAssetMenu(fileName = "UnitStats", menuName = "TheFist/Unit Stats")]
public class UnitStatsSO : ScriptableObject
{
    [Header("Vitality")]
    public float maxHealth = 100f;

    [Header("Offense")]
    public float damage = 10f;

    [Header("Defense")]
    public float armor = 0f;
}
