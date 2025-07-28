using UnityEngine;

/// <summary>
/// Virtual gravity system that applies gravity to a CharacterController with configurable update rate for optimization.
/// Automatically finds the CharacterController component on this GameObject or its children.
/// </summary>
public class VirtualGravity : MonoBehaviour
{
    [Header("Gravity Settings")]
    [SerializeField] private float gravity = -9.81f;
    
    [Header("Update Rate Settings")]
    [SerializeField] private float updateRate = 60f; // Updates per second
    
    private CharacterController characterController;
    private Vector3 velocity;
    private WaitForSeconds waitForUpdate;
    
    private void Awake()
    {
        // Automatically find CharacterController on this GameObject or its children
        characterController = GetComponentInChildren<CharacterController>();
        
        if (characterController == null)
        {
            Debug.LogError($"VirtualGravity: No CharacterController found on {gameObject.name} or its children!");
            enabled = false;
            return;
        }
        
        // Calculate wait time based on update rate
        float updateInterval = 1f / Mathf.Max(updateRate, 1f);
        waitForUpdate = new WaitForSeconds(updateInterval);
    }
    
    private void Start()
    {
        // Initialize velocity and start gravity coroutine
        velocity = Vector3.zero;
        StartCoroutine(GravityUpdateCoroutine());
    }
    
    private System.Collections.IEnumerator GravityUpdateCoroutine()
    {
        while (enabled)
        {
            ApplyGravity();
            yield return waitForUpdate;
        }
    }
    
    private void ApplyGravity()
    {
        if (characterController == null || !characterController.enabled)
            return;
        
        // Calculate delta time based on update rate
        float deltaTime = 1f / updateRate;
        
        // Apply gravity to velocity if not grounded
        if (!characterController.isGrounded)
        {
            velocity.y += gravity * deltaTime;
        }
        else
        {
            // Reset downward velocity when grounded
            if (velocity.y < 0)
                velocity.y = 0;
        }
        
        // Apply the velocity to move the character
        Vector3 movement = velocity * deltaTime;
        characterController.Move(movement);
    }
    
    /// <summary>
    /// Updates the update interval based on the current update rate
    /// </summary>
    private void UpdateInterval()
    {
        float updateInterval = 1f / Mathf.Max(updateRate, 1f);
        waitForUpdate = new WaitForSeconds(updateInterval);
    }
    
    /// <summary>
    /// Sets the gravity value
    /// </summary>
    /// <param name="newGravity">New gravity value (negative for downward force)</param>
    public void SetGravity(float newGravity)
    {
        gravity = newGravity;
    }
    
    /// <summary>
    /// Sets the update rate for gravity calculations
    /// </summary>
    /// <param name="newUpdateRate">Updates per second</param>
    public void SetUpdateRate(float newUpdateRate)
    {
        updateRate = Mathf.Max(newUpdateRate, 1f);
        UpdateInterval();
        // Note: This will only take effect after the current wait cycle completes
    }
    
    /// <summary>
    /// Gets the current velocity
    /// </summary>
    public Vector3 GetVelocity()
    {
        return velocity;
    }
    
    /// <summary>
    /// Sets the velocity (useful for external forces or jumps)
    /// </summary>
    /// <param name="newVelocity">New velocity vector</param>
    public void SetVelocity(Vector3 newVelocity)
    {
        velocity = newVelocity;
    }
    
    /// <summary>
    /// Adds velocity to the current velocity (useful for jumps or external forces)
    /// </summary>
    /// <param name="additionalVelocity">Velocity to add</param>
    public void AddVelocity(Vector3 additionalVelocity)
    {
        velocity += additionalVelocity;
    }
    
    /// <summary>
    /// Resets the velocity to zero
    /// </summary>
    public void ResetVelocity()
    {
        velocity = Vector3.zero;
    }
    
    /// <summary>
    /// Gets whether the character is currently grounded
    /// </summary>
    public bool IsGrounded()
    {
        return characterController != null && characterController.isGrounded;
    }
    
    private void OnValidate()
    {
        // Update interval when values change in inspector
        if (Application.isPlaying)
        {
            UpdateInterval();
        }
    }
}
