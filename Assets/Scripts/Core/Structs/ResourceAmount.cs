
using System;

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