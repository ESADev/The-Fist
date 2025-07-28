using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New World Recipe", menuName = "World Generation/World Recipe")]
public class WorldGenerationRecipeSO : ScriptableObject
{
    [Header("Global Area Settings")]
    [Tooltip("The radius of the generation path.")]
    public float pathRadius = 10.0f;
    
    [Header("Object Profiles")]
    [Tooltip("The list of all prefabs and their individual settings to be placed.")]
    public List<PropProfile> prefabProfiles;
}