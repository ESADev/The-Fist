using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PropProfile
{
    public string name = "Prop";
    [Header("Prefab & Placement")]
    public List<GameObject> prefabVariants;
    [Tooltip("Determines how many placement attempts are made for this prefab relative to the area.")]
    public float density;
    
    [Header("Distribution & Filtering")]
    [Tooltip("Controls probability based on distance from the center line.")]
    public AnimationCurve distanceDistribution;
    [Tooltip("If checked, this prefab will not be placed if it intersects with any other generated object.")]
    public bool checkForIntersections;
    
    [Header("Perlin Noise Filter")]
    public bool usePerlinFilter;
    [Range(0.00001f, 2f)]
    public float perlinScale;
    [Range(0f, 1f)]
    public float perlinThreshold;

    // This constructor sets the default values when you click the '+' button in the Inspector.
    public PropProfile()
    {
        density = 0.005f;
        checkForIntersections = true;
        usePerlinFilter = true;
        perlinScale = 0.025f;
        perlinThreshold = 0.5f;
        distanceDistribution = new AnimationCurve(new Keyframe(0.2f, 0), new Keyframe(1, 10));
    }
}