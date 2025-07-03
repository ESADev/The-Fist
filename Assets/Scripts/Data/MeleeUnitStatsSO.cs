using UnityEngine;

/// <summary>
/// Extended stats for melee units. Inherits base UnitStats and adds
/// configuration for melee combat and AI behaviour so a single asset
/// can drive all components of a melee unit.
/// </summary>
[CreateAssetMenu(fileName = "MeleeUnitStats", menuName = "TheFist/Melee Unit Stats")]
public class MeleeUnitStatsSO : UnitStatsSO
{
    [Header("Melee Combat")]
    [Tooltip("Seconds between consecutive melee attacks")]
    public float attackRate = 1.5f;

    [Tooltip("Distance within which melee attacks can hit")] 
    public float attackRange = 1.5f;

    [Header("AI Targeting")]
    [Tooltip("Radius within which enemies will be detected")]
    public float aggroRadius = 5f;

    [Tooltip("Seconds between target scan cycles")]
    public float scanInterval = 0.5f;

    [Header("Movement")]
    [Tooltip("Movement speed for AI controlled units")]
    public float moveSpeed = 3f;
}
