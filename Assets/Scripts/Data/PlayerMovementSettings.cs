using UnityEngine;

/// <summary>
/// A ScriptableObject to hold data for player movement.
/// Using a ScriptableObject allows for data-driven design. We can create different
/// "movement profiles" as assets in the project and assign them to the player
/// without changing any code. This makes tweaking gameplay values easy and fast.
/// </summary>
[CreateAssetMenu(fileName = "PlayerMovementSettings", menuName = "TheFist/Player Movement Settings")]
public class PlayerMovementSettings : ScriptableObject
{
    // The speed at which the player moves horizontally.
    public float moveSpeed = 5f;

    // The speed at which the player rotates to face the movement direction.
    public float rotationSpeed = 10f;

    // The smoothness factor for movement interpolation (higher = smoother, 0 = instant).
    [Range(0f, 1f)]
    public float movementSmoothness = 0.1f;
}
