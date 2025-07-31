using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ScriptableObject defining resource bounty configuration data.
/// Used to configure what resources are awarded when entities are defeated or specific events occur.
/// </summary>
[CreateAssetMenu(fileName = "Bounty", menuName = "TheFist/Bounty")]
public class BountySO : ScriptableObject
{
    [Header("Bounty Settings")]
    [Tooltip("Resources that will be awarded and their amounts.")]
    public List<ResourceAmount> resourcesToDrop = new List<ResourceAmount>();

    [Tooltip("Radius within which resources will be scattered when awarded.")]
    [Range(0.1f, 10f)]
    public float dropRadius = 2f;

    [Header("Randomization")]
    [Tooltip("Enable random variation in awarded amounts.")]
    public bool useRandomization = true;
    
    [Tooltip("Percentage variation in awarded amounts (e.g., 0.2 = ±20%).")]
    [Range(0f, 1f)]
    public float randomizationFactor = 0.2f;

    /// <summary>
    /// Gets the final amount to award for a specific resource, applying randomization if enabled.
    /// </summary>
    /// <param name="baseAmount">Base amount from the configuration.</param>
    /// <returns>Final amount to award after applying randomization.</returns>
    public int GetRandomizedAmount(int baseAmount)
    {
        if (!useRandomization || randomizationFactor <= 0f)
            return baseAmount;

        float variation = baseAmount * randomizationFactor;
        float randomizedAmount = baseAmount + Random.Range(-variation, variation);
        return Mathf.Max(0, Mathf.RoundToInt(randomizedAmount));
    }

    /// <summary>
    /// Gets all resources with their randomized amounts.
    /// </summary>
    /// <returns>Dictionary of resource types and their randomized amounts.</returns>
    public Dictionary<ResourceType, int> GetRandomizedResources()
    {
        Dictionary<ResourceType, int> result = new Dictionary<ResourceType, int>();
        
        foreach (ResourceAmount resource in resourcesToDrop)
        {
            int finalAmount = GetRandomizedAmount(resource.amount);
            if (finalAmount > 0)
            {
                result[resource.type] = finalAmount;
            }
        }
        
        return result;
    }
}
