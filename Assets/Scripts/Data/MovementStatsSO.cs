using UnityEngine;

/// <summary>
/// Holds configuration data for movement behaviour.
/// </summary>
[CreateAssetMenu(fileName = "MovementStats", menuName = "TheFist/Movement Stats")]
public class MovementStatsSO : ScriptableObject
{
    [Header("Movement")]

    /// <summary>
    /// Base movement speed in units per second.
    /// </summary>
    [Tooltip("Base movement speed in units per second.")]
    public float moveSpeed = 5f;

    /// <summary>
    /// The speed used when turning toward the movement direction.
    /// </summary>
    [Tooltip("The speed used when turning toward the movement direction.")]
    public float turnSpeed = 360f;

    /// <summary>
    /// Controls how smoothly the character moves. 1 means instant movement.
    /// </summary>
    [Tooltip("Controls how smoothly the character moves. 1 means instant movement.")]
    [Range(0f, 1f)]
    public float movementSmoothness = 0.1f;

    /// <summary>
    /// Controls how smoothly the character rotates. 1 means instant rotation.
    /// </summary>
    [Tooltip("Controls how smoothly the character rotates. 1 means instant rotation.")]
    [Range(0f, 1f)]
    public float rotationSmoothness = 0.1f;
}
