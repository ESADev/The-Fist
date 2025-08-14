using UnityEngine;

/// <summary>
/// Moves a character using Unity's <see cref="CharacterController"/> component.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class CharacterControllerMover : MonoBehaviour, IMoveable
{
    private CharacterController characterController;
    private MovementStatsSO stats;
    private Vector3 currentVelocity;
    private Camera targetCamera;
    private Transform cameraTransform;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        if (characterController == null)
        {
            Debug.LogError($"[CharacterControllerMover] Missing CharacterController on {gameObject.name}", this);
            enabled = false;
        }

        // Find the main camera for camera-relative movement
        FindMainCamera();
    }

    /// <summary>
    /// Finds and sets the main camera for camera-relative movement.
    /// Uses the same logic as FaceCamera and FaceOrtographicCamera components.
    /// </summary>
    private void FindMainCamera()
    {
        // Find the target camera if not assigned
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
            
            // Fallback to any camera with the "MainCamera" tag
            if (targetCamera == null)
            {
                GameObject cameraObject = GameObject.FindGameObjectWithTag("MainCamera");
                if (cameraObject != null)
                {
                    targetCamera = cameraObject.GetComponent<Camera>();
                }
            }
            
            // Final fallback to any active camera
            if (targetCamera == null)
            {
                targetCamera = FindFirstObjectByType<Camera>();
            }
        }

        if (targetCamera != null)
        {
            cameraTransform = targetCamera.transform;
        }
        else
        {
            // Not an error - just means we'll use world-space movement
            Debug.Log($"[CharacterControllerMover] No camera found for {gameObject.name}. Using world-space movement.");
        }
    }

    /// <summary>
    /// Initializes the mover with movement statistics.
    /// </summary>
    /// <param name="stats">Movement stats asset.</param>
    public void Initialize(MovementStatsSO stats)
    {
        if (stats == null)
        {
            Debug.LogError($"[CharacterControllerMover] MovementStatsSO is null on {gameObject.name}", this);
            return;
        }
        this.stats = stats;
    }

    float currentSpeed = 0;

    /// <summary>
    /// Moves the character in a direction.
    /// </summary>
    /// <param name="direction">Normalized direction.</param>
    public void MoveInDirection(Vector3 direction)
    {
        if (stats == null) return;

        // Transform the direction based on camera rotation if camera is available
        Vector3 adjustedDirection = GetCameraRelativeDirection(direction);
        Vector3 targetVelocity = adjustedDirection * stats.moveSpeed;

        //currentVelocity = Vector3.Lerp(Vector3.zero, targetVelocity, 1f - Mathf.Pow(1f - stats.movementSmoothness, Time.deltaTime * 60f));

        currentSpeed = adjustedDirection.magnitude * Time.deltaTime * stats.movementSmoothness;

        //currentVelocity = Vector3.Lerp(Vector3.zero, targetVelocity, currentSpeed);

        currentVelocity = targetVelocity;

        if (adjustedDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(adjustedDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 1f - Mathf.Pow(1f - stats.rotationSmoothness, Time.deltaTime * 60f));
        }
        characterController.Move(currentVelocity * Time.deltaTime);
    }

    /// <summary>
    /// Converts input direction to camera-relative direction.
    /// If no camera is found, returns the original direction (world-space).
    /// </summary>
    /// <param name="inputDirection">The input direction (typically from joystick/keyboard)</param>
    /// <returns>Camera-relative direction or original direction if no camera</returns>
    private Vector3 GetCameraRelativeDirection(Vector3 inputDirection)
    {
        // If no camera is available, use the original direction (world-space)
        if (cameraTransform == null)
        {
            return inputDirection;
        }

        // Only use the Y rotation of the camera (ignore pitch and roll)
        float cameraYRotation = cameraTransform.eulerAngles.y;
        Quaternion cameraYOnlyRotation = Quaternion.Euler(0f, cameraYRotation, 0f);

        // Transform the input direction by the camera's Y rotation
        Vector3 cameraRelativeDirection = cameraYOnlyRotation * inputDirection;

        return cameraRelativeDirection;
    }

    /// <summary>
    /// Moves the character towards a destination.
    /// </summary>
    /// <param name="destination">World space destination.</param>
    public void MoveTo(Vector3 destination)
    {
        Vector3 direction = (destination - transform.position).normalized;
        MoveInDirection(direction);
    }

    /// <summary>
    /// Sets a new movement speed.
    /// </summary>
    /// <param name="newSpeed">Speed value.</param>
    public void SetSpeed(float newSpeed)
    {
        if (stats == null) return;
        stats.moveSpeed = newSpeed;
    }

    /// <summary>
    /// Stops the character's movement.
    /// </summary>
    public void Stop()
    {
        currentVelocity = Vector3.zero;
    }

    /// <summary>
    /// Moves the character to a specific transform.
    /// This method is not implemented in this mover.
    /// </summary>
    /// <param name="destinationTransform">The transform to move towards.</param>
    public void MoveTo(Transform destinationTransform)
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Continues the character's movement after a stop or pause.
    /// This method is not implemented in this mover.
    /// </summary>
    /// <exception cref="System.NotImplementedException"></exception>
    public void Continue()
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Sets the stopping distance for the mover.
    /// This method is not implemented in this mover.
    /// </summary>
    public void SetStoppingDistance(float stoppingDistance)
    {
        throw new System.NotImplementedException();
    }
}
