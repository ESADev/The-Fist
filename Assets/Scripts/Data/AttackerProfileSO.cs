using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Aggregates a list of possible attacks for an attacker.
/// </summary>
[CreateAssetMenu(fileName = "AttackerProfile", menuName = "TheFist/Attacker Profile")]
public class AttackerProfileSO : ScriptableObject
{
    [Header("Attacks")]

    /// <summary>
    /// Available attacks for this profile.
    /// </summary>
    [Tooltip("Available attacks for this profile.")]
    public List<AttackDefinitionSO> attacks = new List<AttackDefinitionSO>();
}
