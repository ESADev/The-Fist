using UnityEngine;

/// <summary>
/// Determines long range movement goals for an AI entity and commands its
/// <see cref="MovementController"/> accordingly. This component no longer
/// handles targeting or attacking.
/// </summary>
[RequireComponent(typeof(MovementController))]
public class AIStrategyController : MonoBehaviour
{
    [Header("Strategy")]
    [Tooltip("Ultimate destination for this AI unit when play begins.")]
    public Transform strategicTarget;

    private MovementController movementController;

    private void Awake()
    {
        movementController = GetComponent<MovementController>();
        if (movementController == null)
        {
            Debug.LogError($"[AIStrategyController] Missing MovementController on {gameObject.name}", this);
            enabled = false;
        }
    }

    private void Start()
    {
        if (strategicTarget == null)
        {
            Debug.LogError($"[AIStrategyController] Strategic target not assigned on {gameObject.name}", this);
            return;
        }

        if (movementController != null)
        {
            Debug.Log($"[AIStrategyController] Moving towards {strategicTarget.name}");
            movementController.MoveTo(strategicTarget.position);
        }
    }
}
