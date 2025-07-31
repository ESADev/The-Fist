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
    /// Divides a number into decimal components (1s, 10s, 100s, 1000s, etc.)
    /// Example: 2597 becomes [2, 5, 9, 7] representing 2×1000 + 5×100 + 9×10 + 7×1
    /// </summary>
    private List<int> DivideIntoDecimalComponents(int number)
    {
        var components = new List<int>();
        
        if (number <= 0) return components;

        // Extract digits from highest to lowest value
        while (number > 0)
        {
            int digit = number % 10;
            components.Insert(0, digit); // Insert at beginning to maintain order
            number /= 10;
        }

        return components;
    }

    /// <summary>
    /// Gets the value multiplier for a decimal place (1, 10, 100, 1000, etc.)
    /// </summary>
    private int GetDecimalMultiplier(int digitIndex, int totalDigits)
    {
        int powerOf10 = totalDigits - digitIndex - 1;
        int result = Mathf.RoundToInt(Mathf.Pow(10, powerOf10));
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

        // Divide the amount into decimal components
        var components = DivideIntoDecimalComponents(totalAmount);
        Debug.Log($"ResourceDropper: Dropping {totalAmount} of {resourceType} as components: {string.Join(", ", components)}");

        for (int i = 0; i < components.Count; i++)
        {
            int digitValue = components[i];
            if (digitValue == 0) continue; // Skip zeros

            int multiplier = GetDecimalMultiplier(i, components.Count);

            if (i + 1 < components.Count)
            {
                // Create resources for this decimal component
                for (int j = 0; j < digitValue; j++)
                {
                    //CreateSingleResource(resourcePrefab, multiplier, basePosition);
                    List<int> numbers = RandomlyDivideInteger(multiplier, 0, 3);
                    for (int k = 0; k < numbers.Count; k++)
                    {
                        CreateSingleResource(resourcePrefab, numbers[k], basePosition);
                    }
                }
            }
            else // First digit won't be divided
            {
                List<int> numbers = RandomlyDivideInteger(digitValue, 0, 3);
                for (int k = 0; k < numbers.Count; k++)
                {
                    CreateSingleResource(resourcePrefab, numbers[k], basePosition);
                }
            }
        }
    }

    private List<int> RandomlyDivideInteger(int totalAmount, int minDivision, int maxDivision)
    {
        List<int> result = new();

        int divisionPossibilityCount = maxDivision - minDivision - 1;
        int divisionCount = Mathf.FloorToInt(RandomnessHelper.RandomGaussian01() * divisionPossibilityCount);
        int remainingNumber = totalAmount;
        for (int i = divisionCount; i > 0 && remainingNumber > i; i--)
        {
            int selectedNumber = Random.Range(1, remainingNumber);
            result.Add(selectedNumber);
            remainingNumber -= selectedNumber;
        }
        result.Add(remainingNumber);

        Debug.Log($"ResourceDropper: Randomly divided {totalAmount} into {result.Count} parts: {string.Join(", ", result)}");

        return result;
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
