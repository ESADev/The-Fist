using UnityEngine;

/// <summary>
/// Handles player input and directs a <see cref="MovementController"/> accordingly.
/// </summary>
[RequireComponent(typeof(MovementController))]
public class PlayerInputHandler : MonoBehaviour
{
    private MovementController movementController;
    private float currentInputSpeed = 0f;
    private float lastAnnouncedSpeed = -1f;

    private void Awake()
    {
        movementController = GetComponent<MovementController>();
        if (movementController == null)
        {
            Debug.LogError($"[PlayerInputHandler] MovementController missing on {gameObject.name}", this);
            enabled = false;
        }
    }

    private void OnEnable()
    {
        if (InputManager.Instance != null)
        {
            Debug.Log("[PlayerInputHandler] Subscribed to InputManager.OnMove");
            InputManager.Instance.OnMove += HandleMove;
        }
        else
        {
            Debug.LogError("[PlayerInputHandler] InputManager instance not found.", this);
            enabled = false;
        }
    }

    private void OnDisable()
    {
        if (InputManager.Instance != null)
        {
            Debug.Log("[PlayerInputHandler] Unsubscribed from InputManager.OnMove");
            InputManager.Instance.OnMove -= HandleMove;
        }
    }

    /// <summary>
    /// Receives movement input from the <see cref="InputManager"/>.
    /// </summary>
    /// <param name="movement">Direction of movement.</param>
    private void HandleMove(Vector2 movement)
    {
        Vector3 worldDirection = new Vector3(movement.x, 0f, movement.y).normalized;

        // Calculate input speed
        currentInputSpeed = worldDirection.magnitude;

        // Announce speed changes for camera zoom
        if (Mathf.Abs(currentInputSpeed - lastAnnouncedSpeed) > 0.1f)
        {
            GameEvents.TriggerOnPlayerSpeedChanged(currentInputSpeed, 1f);
            lastAnnouncedSpeed = currentInputSpeed;
        }

        if (worldDirection.sqrMagnitude > 0f)
        {
            //Debug.Log($"[PlayerInputHandler] Moving in direction {worldDirection}");
        }
        movementController.MoveInDirection(worldDirection);
    }
}
