using System.Collections;
using UnityEngine;

public class ResourceCollector : MonoBehaviour
{
    [Header("Collection Settings")]
    [Tooltip("Radius within which resources can be collected.")]
    public float collectionRadius = 5f;
    
    [Tooltip("How often to scan for resources in seconds.")]
    public float scanFrequency = 0.5f;

    [SerializeField] Vector3 collectionPointOffset = Vector3.zero;

    private Coroutine collectionCoroutine;

    private void OnEnable()
    {
        collectionCoroutine = StartCoroutine(CollectionCoroutine());
    }

    private void OnDisable()
    {
        if (collectionCoroutine != null)
        {
            StopCoroutine(collectionCoroutine);
            collectionCoroutine = null;
        }
    }

    private IEnumerator CollectionCoroutine()
    {
        while (true)
        {
            CollectResourcesInRange();
            yield return new WaitForSeconds(1f/scanFrequency);
        }
    }

    private void CollectResourcesInRange()
    {
        // Find all colliders within collection radius on the "Resource" layer
        Collider[] colliders = Physics.OverlapSphere(transform.position, collectionRadius, LayerMask.GetMask("Resource"));

        foreach (Collider collider in colliders)
        {
            // Try to get Resource component from the collider
            Resource resource = collider.GetComponentInParent<Resource>();
            if (resource != null)
            {
                // Call the collect method on the resource
                resource.Collect(transform.position + collectionPointOffset);
            }
            else
            {
                Debug.LogWarning($"ResourceCollector: No Resource component found on {collider.name}");
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Draw the collection radius in the scene view
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, collectionRadius);
    }
}
