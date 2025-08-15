using System.Collections.Generic;
using NUnit.Framework.Interfaces;
using UnityEngine;

/// <summary>
/// A simple worker component that drops resources of specified types and amounts.
/// Uses smart decimal division to create resources with appropriate values (1s, 10s, 100s, 1000s).
/// </summary>
public class ResourceDropper : MonoBehaviour
{
    [Header("Drop Settings")]
    [SerializeField] private float dropRadius = 2f;
    [SerializeField] private Vector3 dropOffset = Vector3.zero;

    /// <summary>
    /// Initialize with a single resource type and amount
    /// </summary>
    public void Initialize(ResourceType resourceType, int amount, Vector3? dropPosition = null)
    {
        var resources = new Dictionary<ResourceType, int> { { resourceType, amount } };
        Initialize(resources, dropPosition);
    }

    /// <summary>
    /// Initialize with multiple resource types and amounts
    /// </summary>
    public void Initialize(Dictionary<ResourceType, int> resources, Vector3? dropPosition = null)
    {
        if (resources == null || resources.Count == 0)
        {
            Debug.LogWarning("ResourceDropper: No resources to drop!");
            DestroySelf();
            return;
        }

        Vector3 dropPos = dropPosition ?? transform.position;
        DropResources(resources, dropPos);
    }

    /// <summary>
    /// Initialize with BountySO configuration
    /// </summary>
    public void Initialize(BountySO bounty, Vector3? dropPosition = null)
    {
        Debug.Log($"ResourceDropper: Initializing with bounty {bounty.name}");

        if (bounty == null)
        {
            Debug.LogWarning("ResourceDropper: No bounty provided!");
            DestroySelf();
            return;
        }

        // Apply bounty settings
        dropRadius = bounty.dropRadius;

        // Get randomized resources and drop them
        var resources = bounty.GetRandomizedResources();
        Initialize(resources, dropPosition);
    }

    /// <summary>
    /// Simple division that creates a random number of resource drops
    /// </summary>
    private List<int> SimpleDivideAmount(int totalAmount)
    {
        var result = new List<int>();
        
        if (totalAmount <= 0) return result;

        // Randomly decide how many drops to create (1-5)
        int dropCount = Mathf.RoundToInt(RandomnessHelper.RandomGaussian01() * 4 + 1f);
        int remaining = totalAmount;

        for (int i = 0; i < dropCount - 1 && remaining > 1; i++)
        {
            int maxDrop = remaining - (dropCount - i - 1); // Ensure we have at least 1 for each remaining drop
            int dropAmount = Random.Range(1, maxDrop + 1);
            result.Add(dropAmount);
            remaining -= dropAmount;
        }
        
        if (remaining > 0)
        {
            result.Add(remaining);
        }

        return result;
    }

    private void DropResources(Dictionary<ResourceType, int> resources, Vector3 dropPosition)
    {
        foreach (var resource in resources)
        {
            if (resource.Value > 0)
            {
                CreateResourcesWithDecimalDivision(resource.Key, resource.Value, dropPosition);
            }
        }

        // SFX
        SFXManager.Instance.PlaySound("gem splash", dropPosition);

        DestroySelf();
    }

    private void CreateResourcesWithDecimalDivision(ResourceType resourceType, int totalAmount, Vector3 basePosition)
    {
        GameObject resourcePrefab = LoadResourcePrefab(resourceType);
        if (resourcePrefab == null)
        {
            Debug.LogError($"ResourceDropper: Could not find prefab for {resourceType}");
            return;
        }

        // Simple division into multiple drops
        var amounts = SimpleDivideAmount(totalAmount);
        Debug.Log($"ResourceDropper: Dropping {totalAmount} of {resourceType} as {amounts.Count} drops: {string.Join(", ", amounts)}");

        foreach (int amount in amounts)
        {
            CreateSingleResource(resourcePrefab, amount, basePosition);
        }
    }

    private void CreateSingleResource(GameObject prefab, int value, Vector3 basePosition)
    {
        Vector3 dropPosition = GetRandomDropPosition(basePosition + dropOffset);
        GameObject instance = Instantiate(prefab, dropPosition, GetRandomRotation());

        // Initialize the resource
        Resource resourceComponent = instance.GetComponent<Resource>();
        if (resourceComponent != null)
        {
            resourceComponent.Initialize(value);
        }
        else
        {
            Debug.LogError($"ResourceDropper: Resource prefab {prefab.name} missing Resource component!");
        }

        // Add physics
        AddDropPhysics(instance);
    }

    private GameObject LoadResourcePrefab(ResourceType resourceType)
    {
        string[] paths = {
            $"Prefabs/Resources/{resourceType}Resource",
            $"Prefabs/Resources/{resourceType}",
            $"Resources/{resourceType}",
            $"{resourceType}"
        };

        foreach (string path in paths)
        {
            GameObject prefab = Resources.Load<GameObject>(path);
            if (prefab != null) return prefab;
        }

        Debug.LogError($"ResourceDropper: Could not find prefab for {resourceType} in any of the paths: {string.Join(", ", paths)}");
        // If no prefab found, return null
        return null;
    }

    private Vector3 GetRandomDropPosition(Vector3 basePosition)
    {
        Vector2 randomCircle = Random.insideUnitCircle * dropRadius;
        return basePosition + dropOffset + new Vector3(randomCircle.x, 0, randomCircle.y);
    }

    private Quaternion GetRandomRotation()
    {
        return Quaternion.Euler(0, Random.Range(0f, 360f), 0);
    }

    private void AddDropPhysics(GameObject instance)
    {
        Rigidbody rb = instance.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 randomDirection = new Vector3(
                Random.Range(-1f, 1f),
                Random.Range(0.5f, 1f),
                Random.Range(-1f, 1f)
            ).normalized;
            
            rb.AddForce(randomDirection * Mathf.Pow(dropRadius, 2), ForceMode.Impulse);
            rb.AddTorque(Random.insideUnitSphere * Mathf.Pow(dropRadius, 2) / 5 * 0.5f, ForceMode.Impulse);
        }
    }

    private void DestroySelf()
    {
        Destroy(this, 0.1f);
    }
}
