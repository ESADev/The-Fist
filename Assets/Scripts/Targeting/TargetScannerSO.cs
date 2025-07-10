using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds configuration data for a TargetScanner component. This ScriptableObject
/// allows designers to tweak scanning behaviour and interaction priorities without modifying code.
/// </summary>
[CreateAssetMenu(fileName = "TargetScanner", menuName = "TheFist/Target Scanner")]
public class TargetScannerSO : ScriptableObject
{
    [Header("Scanning")]

    /// <summary>
    /// Maximum distance at which this scanner can detect targets.
    /// </summary>
    [Tooltip("Maximum distance at which this scanner can detect targets.")]
    public float detectionRadius = 5f;

    /// <summary>
    /// How often, in seconds, the scanner checks for targets.
    /// </summary>
    [Tooltip("How often, in seconds, the scanner checks for targets.")]
    public float scanFrequency = 0.5f;

    /// <summary>
    /// Which interaction types this scanner is allowed to consider when looking for targets.
    /// Multiple types can be combined because <see cref="InteractionType"/> is a flag enum.
    /// </summary>
    [Tooltip("Which interaction types this scanner is allowed to consider when looking for targets.")]
    public InteractionType allowedInteractions = InteractionType.None;

    [Header("Interaction Prioritization")]

    /// <summary>
    /// Defines how different interaction types are prioritised when multiple targets are detected.
    /// Entries at the beginning of the list have higher priority.
    /// </summary>
    [Tooltip("Priority order of interaction types from highest to lowest priority.")]
    public List<InteractionType> interactionPriority = new List<InteractionType>();
}

