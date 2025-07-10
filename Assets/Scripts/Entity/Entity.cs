using UnityEngine;

/// <summary>
/// Central hub component that aggregates common gameplay systems for a unit.
/// It configures attached components using data from a <see cref="CharacterDefinitionSO"/>.
/// </summary>
[DefaultExecutionOrder(-1000)]
[DisallowMultipleComponent]
[RequireComponent(typeof(Faction))]
public class Entity : MonoBehaviour
{
    [Header("Definition")]
    [Tooltip("Character definition containing configuration data for this entity.")]
    public CharacterDefinitionSO characterDefinition;

    /// <summary>
    /// Gets the <see cref="Faction"/> component attached to this entity.
    /// </summary>
    public Faction Faction { get; private set; }

    /// <summary>
    /// Gets the <see cref="Health"/> component attached to this entity if present.
    /// </summary>
    public Health Health { get; private set; }

    /// <summary>
    /// Gets the <see cref="Attacker"/> component attached to this entity if present.
    /// </summary>
    public Attacker Attacker { get; private set; }

    /// <summary>
    /// Gets the <see cref="MovementController"/> component attached to this entity if present.
    /// </summary>
    public MovementController MovementController { get; private set; }

    /// <summary>
    /// Gets the <see cref="TargetScanner"/> component attached to this entity if present.
    /// </summary>
    public TargetScanner TargetScanner { get; private set; }

    private void Awake()
    {
        CacheComponents();
        InitializeFromDefinition();
    }

    /// <summary>
    /// Caches frequently used component references for quick access.
    /// </summary>
    private void CacheComponents()
    {
        Faction = GetComponent<Faction>();
        Health = GetComponent<Health>();
        Attacker = GetComponent<Attacker>();
        MovementController = GetComponent<MovementController>();
        TargetScanner = GetComponent<TargetScanner>();
    }

    /// <summary>
    /// Initializes attached systems using values from <see cref="characterDefinition"/>.
    /// </summary>
    public void InitializeFromDefinition()
    {
        if (characterDefinition == null)
        {
            Debug.LogError($"[Entity] CharacterDefinitionSO is not assigned on {gameObject.name}.", this);
            return;
        }

        if (Faction != null)
        {
            Faction.Initialize(characterDefinition.faction);
        }
        else
        {
            Debug.LogError($"[Entity] Missing Faction component on {gameObject.name}.", this);
        }

        if (Health != null)
        {
            Health.stats = characterDefinition.healthStats;
        }
        else if (characterDefinition.healthStats != null)
        {
            Debug.LogWarning($"[Entity] {gameObject.name} has health stats in its definition but no Health component.", this);
        }

        if (MovementController != null)
        {
            MovementController.Initialize(characterDefinition.movementStats);
        }
        else if (characterDefinition.movementStats != null)
        {
            Debug.LogWarning($"[Entity] {gameObject.name} has movement stats in its definition but no MovementController component.", this);
        }

        if (Attacker != null)
        {
            Attacker.attackerProfile = characterDefinition.attackerProfile;
        }
        else if (characterDefinition.attackerProfile != null)
        {
            Debug.LogWarning($"[Entity] {gameObject.name} has an attacker profile but no Attacker component.", this);
        }

        if (TargetScanner != null)
        {
            TargetScanner.scannerProfile = characterDefinition.targetScanner;
        }
        else if (characterDefinition.targetScanner != null)
        {
            Debug.LogWarning($"[Entity] {gameObject.name} has a target scanner profile but no TargetScanner component.", this);
        }
    }
}
