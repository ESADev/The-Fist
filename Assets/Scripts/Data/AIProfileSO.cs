using UnityEngine;

/// <summary>
/// Configuration profile used by AI systems to evaluate and
/// prioritize potential targets.
/// </summary>
[CreateAssetMenu(fileName = "AIProfile", menuName = "TheFist/AI Profile")]
public class AIProfileSO : ScriptableObject
{
    [Header("Target Scoring")]
    [Tooltip("Score bonus applied when the target can be destroyed.")]
    public float destructibleBonus = 10f;

    [Tooltip("Multiplier applied to the distance when evaluating targets. Lower values make distant targets less desirable.")]
    public float distanceWeight = 1f;
}
