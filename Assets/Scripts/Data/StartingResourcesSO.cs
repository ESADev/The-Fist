using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ScriptableObject defining the player's initial resource amounts.
/// </summary>
[CreateAssetMenu(fileName = "StartingResources", menuName = "TheFist/Starting Resources")]
public class StartingResourcesSO : ScriptableObject
{
    /// <summary>
    /// Represents a single resource amount entry.
    /// </summary>
    [Serializable]
    public struct ResourceAmount
    {
        /// <summary>
        /// Type of resource.
        /// </summary>
        public ResourceType type;

        /// <summary>
        /// Amount of the resource.
        /// </summary>
        public int amount;
    }

    [Header("Resources")]
    [Tooltip("List of resource amounts granted at game start.")]
    public List<ResourceAmount> resources = new List<ResourceAmount>();
}
