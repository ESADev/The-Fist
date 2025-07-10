using UnityEngine;

/// <summary>
/// Holds configuration data for a <see cref="TargetScanner"/> component. This
/// ScriptableObject allows designers to tweak scanning behaviour without modifying code.
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


}

