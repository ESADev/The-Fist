using UnityEngine;

/// <summary>
/// Configuration data for a <see cref="ResourceGenerator"/> component.
/// </summary>
[CreateAssetMenu(fileName = "ResourceGeneratorProfile", menuName = "TheFist/Resource Generator Profile")]
public class ResourceGeneratorProfileSO : ScriptableObject
{
    [Header("Generator Settings")]
    [Tooltip("Type of resource to generate.")]
    public ResourceType resourceType = ResourceType.Gold;

    [Tooltip("Amount generated per tick.")]
    public int amountPerTick = 1;

    [Tooltip("Time in seconds between each generation tick.")]
    public float tickRateInSeconds = 5f;
}
