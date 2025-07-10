using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Singleton component that manages all player resources and broadcasts changes through <see cref="GameEvents"/>.
/// </summary>
public class ResourceManager : MonoBehaviour
{
    /// <summary>
    /// Global singleton instance.
    /// </summary>
    public static ResourceManager Instance { get; private set; }

    [Header("Configuration")]
    [Tooltip("Starting amounts for each resource type.")]
    public StartingResourcesSO startingResources;

    /// <summary>
    /// Dictionary holding the current amount for each resource type.
    /// </summary>
    private readonly Dictionary<ResourceType, int> resources = new Dictionary<ResourceType, int>();

    private void Awake()
    {
        // Singleton enforcement
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeResources();
    }

    /// <summary>
    /// Initializes the resources dictionary using the provided <see cref="startingResources"/> asset.
    /// </summary>
    public void InitializeResources()
    {
        resources.Clear();

        foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
        {
            resources[type] = 0;
        }

        if (startingResources == null)
        {
            Debug.LogError("[ResourceManager] StartingResourcesSO is not assigned.", this);
        }
        else
        {
            foreach (StartingResourcesSO.ResourceAmount entry in startingResources.resources)
            {
                resources[entry.type] = Mathf.Max(entry.amount, 0);
            }
        }

        foreach (KeyValuePair<ResourceType, int> pair in resources)
        {
            GameEvents.TriggerOnPlayerResourceChanged(pair.Key, pair.Value);
        }
    }

    /// <summary>
    /// Returns the current amount of the specified resource.
    /// </summary>
    /// <param name="type">Type of resource.</param>
    /// <returns>Amount of resource currently stored.</returns>
    public int GetResourceAmount(ResourceType type)
    {
        resources.TryGetValue(type, out int amount);
        return amount;
    }

    /// <summary>
    /// Determines if the player has at least the specified amount of a resource.
    /// </summary>
    /// <param name="type">Type of resource.</param>
    /// <param name="amount">Amount required.</param>
    /// <returns>True if the player can afford the cost.</returns>
    public bool CanAfford(ResourceType type, int amount)
    {
        return GetResourceAmount(type) >= amount;
    }

    /// <summary>
    /// Adds a quantity of the specified resource to the player's bank.
    /// </summary>
    /// <param name="type">Type of resource to add.</param>
    /// <param name="amount">Amount to add.</param>
    public void AddResource(ResourceType type, int amount)
    {
        if (amount == 0)
        {
            return;
        }

        int newAmount = GetResourceAmount(type) + amount;
        resources[type] = newAmount;
        Debug.Log($"[ResourceManager] Added {amount} {type}. New amount: {newAmount}");
        GameEvents.TriggerOnPlayerResourceChanged(type, newAmount);
    }

    /// <summary>
    /// Attempts to deduct a quantity of the specified resource from the player's bank.
    /// </summary>
    /// <param name="type">Type of resource to spend.</param>
    /// <param name="amount">Amount to spend.</param>
    /// <returns>True if the resource was successfully spent.</returns>
    public bool SpendResource(ResourceType type, int amount)
    {
        if (!CanAfford(type, amount))
        {
            Debug.LogWarning($"[ResourceManager] Cannot spend {amount} {type}. Current: {GetResourceAmount(type)}");
            return false;
        }

        int newAmount = GetResourceAmount(type) - amount;
        resources[type] = newAmount;
        Debug.Log($"[ResourceManager] Spent {amount} {type}. New amount: {newAmount}");
        GameEvents.TriggerOnPlayerResourceChanged(type, newAmount);
        return true;
    }
}
