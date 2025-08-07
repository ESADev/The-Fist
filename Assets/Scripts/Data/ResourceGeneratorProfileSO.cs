using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Configuration data for a <see cref="ResourceGenerator"/> component.
/// </summary>
[CreateAssetMenu(fileName = "ResourceGeneratorProfile", menuName = "TheFist/Resource Generator Profile")]
public class ResourceGeneratorProfileSO : ScriptableObject
{
    public List<ResourceGenerationField> resources;
}

[Serializable]
public class ResourceGenerationField
{
    [Header("Generator Settings")]
    [Tooltip("Type of resource to generate.")]
    public ResourceType resourceType = ResourceType.Prestige;

    [Tooltip("Amount generated per second.")]
    public int amountPerSeconds = 1;

    [Tooltip("Time in seconds between each generation tick. A new batch (amount depends on the amount per second field) will be generated after this amount of time.")]
    public float tickRateInSeconds = 5f;
}