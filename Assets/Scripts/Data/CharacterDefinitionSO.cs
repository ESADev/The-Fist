using UnityEngine;

/// <summary>
/// Defines all data required to spawn and configure a character.
/// Acts as an ID card that can be shared between gameplay systems.
/// </summary>
[CreateAssetMenu(fileName = "CharacterDefinition", menuName = "TheFist/Character Definition")]
public class CharacterDefinitionSO : ScriptableObject
{
    [Header("General")]

    /// <summary>
    /// Display name of the character shown in UI and logs.
    /// </summary>
    [Tooltip("Display name of the character shown in UI and logs.")]
    public string characterName = "New Character";

    /// <summary>
    /// Prefab representing the character in the scene.
    /// </summary>
    [Tooltip("Prefab representing the character in the scene.")]
    public GameObject characterPrefab;

    /// <summary>
    /// Faction alignment determining friend or foe relationships.
    /// </summary>
    [Tooltip("Faction alignment determining friend or foe relationships.")]
    public FactionType faction = FactionType.Neutral;

    [Header("Stats")]

    /// <summary>
    /// Health statistics defining hit points and armor.
    /// </summary>
    [Tooltip("Health statistics defining hit points and armor.")]
    public HealthStatsSO healthStats;

    /// <summary>
    /// Movement statistics controlling speed and turning behaviour.
    /// </summary>
    [Tooltip("Movement statistics controlling speed and turning behaviour.")]
    public MovementStatsSO movementStats;

    /// <summary>
    /// Collection of attacks available to the character.
    /// </summary>
    [Tooltip("Collection of attacks available to the character.")]
    public AttackerProfileSO attackerProfile;

    [Header("Targeting")]

    /// <summary>
    /// Configuration asset describing how this character scans for targets.
    /// </summary>
    [Tooltip("Configuration asset describing how this character scans for targets.")]
    public TargetScannerSO targetScanner;

    [Header("Presentation")]

    /// <summary>
    /// Optional portrait used in UI elements such as dialogue or HUD.
    /// </summary>
    [Tooltip("Optional portrait used in UI elements such as dialogue or HUD.")]
    public Sprite characterPortrait;
}
