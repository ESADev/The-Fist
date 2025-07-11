using UnityEngine;
using System;

/// <summary>
/// Manages all player input. It acts as a central hub for input events.
/// This is a singleton to ensure there's only one instance managing input.
/// It uses the Observer Pattern (with a C# Action) to broadcast input events.
/// Components that need to react to input (like PlayerMovement) can subscribe to these events
/// without needing a direct reference to this InputManager, promoting decoupling.
/// </summary>
public class InputManager : MonoBehaviour
{
    // Singleton instance
    public static InputManager Instance { get; private set; }

    // Event fired when there is movement input.
    // The Vector2 parameter represents the movement on the X and Z axes.
    public event Action<Vector2> OnMove;

    private Vector2 _movementInput;

    private void Awake()
    {
        // Singleton pattern implementation
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Make it persistent across scenes
        }
    }

    private void Update()
    {
        Vector2 finalMovementInput;

        // Read keyboard input first.
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector2 keyboardInput = new Vector2(horizontal, vertical);

        // Prioritize joystick input if it's being used, otherwise use keyboard input.
        if (_movementInput != Vector2.zero)
        {
            finalMovementInput = _movementInput;
        }
        else
        {
            finalMovementInput = keyboardInput;
        }

        if (finalMovementInput.magnitude > 0.01f)
        {
            //Debug.Log($"[InputManager] Final input for this frame: {finalMovementInput}");
        }

        // Only invoke the event if there is a listener
        if (OnMove != null)
        {
            OnMove.Invoke(finalMovementInput);
        }
        else
        {
            // This is a critical warning. If nothing is listening, the player won't move.
            Debug.LogWarning("[InputManager] OnMove event has no subscribers!");
        }

        // Reset joystick input at the end of the frame. It must be set every frame by the joystick.
        _movementInput = Vector2.zero;
    }

    /// <summary>
    /// Public method that can be called by UI elements (like the virtual joystick)
    /// to set the movement input value.
    /// </summary>
    /// <param name="movement">The movement input vector.</param>
    public void SetMovementInput(Vector2 movement)
    {
        _movementInput = movement;
    }
}