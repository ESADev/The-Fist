using UnityEngine;

/// <summary>
/// Configuration data for UnitAIController. Keeps AI behaviour data-driven.
/// </summary>
[CreateAssetMenu(fileName = "UnitAISettings", menuName = "TheFist/Unit AI Settings")]
public class UnitAISettingsSO : ScriptableObject
{
    [Header("Targeting")]
    public float aggroRadius = 5f;

    [Tooltip("Seconds between scan cycles")]
    public float scanInterval = 0.5f;

    [Header("Movement")]
    public float moveSpeed = 3f;
}
