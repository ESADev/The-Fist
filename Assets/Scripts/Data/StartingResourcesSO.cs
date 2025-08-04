using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ScriptableObject defining the player's initial resource amounts.
/// </summary>
[CreateAssetMenu(fileName = "StartingResources", menuName = "TheFist/Starting Resources")]
public class StartingResourcesSO : ScriptableObject
{
    [Header("Resources")]
    [Tooltip("List of resource amounts granted at game start.")]
    public List<ResourceAmount> resources = new List<ResourceAmount>();
}