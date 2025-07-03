using UnityEngine;

/// <summary>
/// Handles the player's horizontal movement.
/// This component follows the Single Responsibility Principle (SRP) by only handling movement logic.
/// It's decoupled from the input source; it doesn't care if the input comes from a keyboard,
/// a joystick, or AI. It simply listens for an OnMove event from the InputManager.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    // A reference to the ScriptableObject holding our movement settings.
    // This makes the component data-driven.
    [SerializeField] private PlayerMovementSettings movementSettings;

    private Rigidbody _rb;
    private Vector2 _moveInput;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        // This is a critical check. If the settings asset is not assigned in the inspector,
        // the player will not be able to move.
        if (movementSettings == null)
        {
            Debug.LogError("[PlayerMovement] Movement Settings is not assigned! Please assign it in the Inspector.");
            // Disable the component to prevent further errors.
            this.enabled = false;
        }
    }

    // Subscribe to the input event when the component is enabled.
    private void OnEnable()
    {
        if (InputManager.Instance != null)
        {
            Debug.Log("[PlayerMovement] Subscribing to OnMove event.");
            InputManager.Instance.OnMove += HandleMove;
        }
        else
        {
            // This could happen if the InputManager is not in the scene or is initialized after this component.
            Debug.LogError("[PlayerMovement] InputManager.Instance is null on Enable! Check script execution order.");
        }
    }

    // Unsubscribe from the event when the component is disabled to prevent memory leaks.
    private void OnDisable()
    {
        if (InputManager.Instance != null)
        {
            Debug.Log("[PlayerMovement] Unsubscribing from OnMove event.");
            InputManager.Instance.OnMove -= HandleMove;
        }
    }

    // The handler for the OnMove event. It receives the movement input.
    private void HandleMove(Vector2 movementInput)
    {
        // The check for movementInput != Vector2.zero was removed.
        // We must accept the zero vector from the InputManager to know when to stop.
        if (this.enabled)
        {
            _moveInput = movementInput;

            if (_moveInput.magnitude > 0.01f)
            {
                Debug.Log($"[PlayerMovement] Move event received with input: {_moveInput}");
            }
        }
    }

    // Apply the movement and rotation in Update.
    private void Update()
    {
        // --- MOVEMENT ---
        // We create a 3D movement vector from the 2D input (x,y) -> (x,0,y).
        Vector3 movementDirection = new Vector3(_moveInput.x, 0f, _moveInput.y);

        // We move the transform based on the direction, speed, and delta time.
        transform.position += movementDirection.normalized * (movementSettings.moveSpeed * Time.deltaTime);

        // --- ROTATION ---
        if (movementDirection != Vector3.zero)
        {
            // Create a rotation that looks in the direction of movement.
            Quaternion targetRotation = Quaternion.LookRotation(movementDirection);

            // Smoothly interpolate from the current rotation to the target rotation.
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, movementSettings.rotationSpeed * Time.deltaTime);

            Debug.Log($"[PlayerMovement] Moving with direction: {movementDirection}");
        }
    }
}
