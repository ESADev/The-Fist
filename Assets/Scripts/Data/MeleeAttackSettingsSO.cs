using UnityEngine;

/// <summary>
/// Settings used by MeleeAttacker to control attack timing and range.
/// </summary>
[CreateAssetMenu(fileName = "MeleeAttackSettings", menuName = "TheFist/Melee Attack Settings")]
public class MeleeAttackSettingsSO : ScriptableObject
{
    [Header("Timing")]
    [Tooltip("Seconds between consecutive attacks")]
    public float attackRate = 1.5f;

    [Header("Range")]
    public float attackRange = 1.5f;
}
