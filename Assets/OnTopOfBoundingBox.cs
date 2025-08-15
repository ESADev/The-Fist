using UnityEngine;
using System.Collections;

public class OnTopOfBoundingBox : MonoBehaviour
{
    [Header("Target Object")]
    [SerializeField] private GameObject targetObject;
    
    [Header("Positioning Settings")]
    [SerializeField] private float offsetY = 0.5f; // Additional offset above the bounding box
    [SerializeField] private bool followX = false; // Whether to follow X position
    [SerializeField] private bool followZ = false; // Whether to follow Z position
    
    [Header("Optimization Settings")]
    [Tooltip("Time between position updates in seconds. Lower values mean more frequent updates but can impact performance.")]
    [SerializeField, Range(0.01f, 1f)] private float checkRate = 0.1f; // Time between position updates in seconds
    
    private Bounds targetBounds;
    private Renderer[] targetRenderers;
    private bool hasValidTarget = false;
    
    private void Start()
    {
        ValidateTarget();
        if (hasValidTarget)
        {
            StartCoroutine(UpdatePositionCoroutine());
        }
    }
    
    private void ValidateTarget()
    {
        if (targetObject == null)
        {
            try
            {
                targetObject = transform.parent.gameObject;
                hasValidTarget = true;
            }
            catch
            {
                hasValidTarget = false;
                Debug.LogWarning($"OnTopOfBoundingBox on {gameObject.name}: No target object assigned and no parrent exists!");
                return;
            }
        }
        
        // Get all renderers from target and its children
        targetRenderers = targetObject.GetComponentsInChildren<Renderer>();
        
        if (targetRenderers.Length == 0)
        {
            Debug.LogWarning($"OnTopOfBoundingBox on {gameObject.name}: Target object '{targetObject.name}' and its children have no Renderer or Collider components!");
            hasValidTarget = false;
            return;
        }
        
        hasValidTarget = true;
    }

    private void RefreshTargetRenderers()
    {
        // Refresh the target renderers and collider in case they change at runtime
        targetRenderers = targetObject.GetComponentsInChildren<Renderer>();
        
        if (targetRenderers.Length == 0)
        {
            Debug.LogWarning($"OnTopOfBoundingBox on {gameObject.name}: Target object '{targetObject.name}' and its children have no Renderer or Collider components!");
            hasValidTarget = false;
            return;
        }
    }
    
    private IEnumerator UpdatePositionCoroutine()
    {
        while (hasValidTarget && targetObject != null)
        {
            UpdatePosition();
            yield return new WaitForSeconds(checkRate);
        }
    }
    
    private void UpdatePosition()
    {
        if (!hasValidTarget || targetObject == null) return;

        RefreshTargetRenderers();

        // Calculate combined bounds from all renderers or use collider bounds
        if (targetRenderers.Length > 0)
        {
            // Start with the first renderer's bounds
            targetBounds = targetRenderers[0].bounds;

            // Encapsulate all other renderer bounds
            for (int i = 1; i < targetRenderers.Length; i++)
            {
                targetBounds.Encapsulate(targetRenderers[i].bounds);
            }
        }
        else
        {
            return;
        }
        
        // Calculate new position
        Vector3 newPosition = transform.position;
        
        if (followX)
            newPosition.x = targetBounds.center.x;
            
        if (followZ)
            newPosition.z = targetBounds.center.z;
            
        // Always position on top of the bounding box
        newPosition.y = targetBounds.max.y + offsetY;
        
        transform.position = newPosition;
    }
    
    // Public method to manually update position (useful for immediate updates)
    public void ForceUpdatePosition()
    {
        if (hasValidTarget)
        {
            UpdatePosition();
        }
    }
    
    // Public method to change target at runtime
    public void SetTarget(GameObject newTarget)
    {
        targetObject = newTarget;
        ValidateTarget();
        
        if (hasValidTarget)
        {
            // Stop current coroutine and start new one
            StopAllCoroutines();
            StartCoroutine(UpdatePositionCoroutine());
        }
    }
    
    // Public method to change check rate at runtime
    public void SetCheckRate(float newRate)
    {
        checkRate = Mathf.Clamp(newRate, 0.01f, 1f);
    }
    
    private void OnValidate()
    {
        // Clamp check rate in editor
        checkRate = Mathf.Clamp(checkRate, 0.01f, 1f);
    }
    
    private void OnDrawGizmosSelected()
    {
        if (targetObject != null && hasValidTarget)
        {
            // Draw a line from this object to the target's top
            Gizmos.color = Color.green;
            
            Bounds bounds;
            Renderer[] renderers = targetObject.GetComponentsInChildren<Renderer>();
            Collider col = targetObject.GetComponent<Collider>();
            
            if (renderers.Length > 0)
            {
                // Calculate combined bounds from all child renderers
                bounds = renderers[0].bounds;
                for (int i = 1; i < renderers.Length; i++)
                {
                    bounds.Encapsulate(renderers[i].bounds);
                }
            }
            else if (col != null)
            {
                bounds = col.bounds;
            }
            else
            {
                return;
            }
                
            Vector3 topCenter = new Vector3(bounds.center.x, bounds.max.y, bounds.center.z);
            Gizmos.DrawLine(transform.position, topCenter);
            
            // Draw a small sphere at the target position
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(topCenter, 0.1f);
            
            // Draw the combined bounding box outline
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }
    }
}
