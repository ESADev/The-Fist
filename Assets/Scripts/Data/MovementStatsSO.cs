using UnityEngine;

/// <summary>
/// Holds configuration data for movement behaviour.
/// </summary>
[CreateAssetMenu(fileName = "MovementStats", menuName = "TheFist/Movement Stats")]
public class MovementStatsSO : ScriptableObject
{
    [Header("Speed Settings")]

    /// <summary>
    /// Base movement speed in units per second.
    /// </summary>
    [Tooltip("Base movement speed in units per second.")]
    public float moveSpeed = 5f;

    /// <summary>
    /// Maximum speed that the object can reach.
    /// </summary>
    [Tooltip("Maximum speed that the object can reach.")]
    public float maxSpeed = 10f;

    /// <summary>
    /// Rate at which the object accelerates.
    /// </summary>
    [Tooltip("Rate at which the object accelerates.")]
    public float acceleration = 5f;
}
