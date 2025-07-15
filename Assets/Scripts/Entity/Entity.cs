using UnityEngine;

/// <summary>
/// Central hub component that aggregates common gameplay systems for a unit.
/// It configures attached components using data from a <see cref="CharacterDefinitionSO"/>.
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(Faction))]
[RequireComponent(typeof(Health))]
public class Entity : MonoBehaviour
{
    [Header("Definition")]
    [Tooltip("Character definition containing configuration data for this entity.")]
    [SerializeField] private CharacterDefinitionSO _characterDefinition;

    /// <summary>
    /// Gets or sets the character definition. Setting this value automatically initializes the entity.
    /// </summary>
    public CharacterDefinitionSO characterDefinition
    {
        get => _characterDefinition;
        set
        {
            _characterDefinition = value;
            if (value != null && Application.isPlaying)
            {
                InitializeFromDefinition();
            }
        }
    }

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

    /// <summary>
    /// Gets the <see cref="AutoInteractor"/> component attached to this entity if present.
    /// </summary>
    public AutoInteractor Interactor { get; private set; }

    private void Awake()
    {
        CacheComponents();

        if (_characterDefinition == null)
        {
            Debug.LogError($"[Entity] CharacterDefinitionSO is not assigned on {gameObject.name}. Entity initialization will be skipped.", this);
            return;
        }
    }

    private void Start()
    {
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
        Interactor = GetComponent<AutoInteractor>();
    }

    /// <summary>
    /// Initializes attached systems using values from <see cref="characterDefinition"/>.
    /// </summary>
    public void InitializeFromDefinition()
    {
        if (characterDefinition == null)
        {
            Debug.LogWarning($"[Entity] Cannot initialize {gameObject.name} - CharacterDefinitionSO is not assigned.", this);
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
            Health.Initialize(characterDefinition.healthStats);
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
            Attacker.Initialize(characterDefinition.attackerProfile);
        }
        else if (characterDefinition.attackerProfile != null)
        {
            Debug.LogWarning($"[Entity] {gameObject.name} has an attacker profile but no Attacker component.", this);
        }

        if (TargetScanner != null)
        {
            TargetScanner.Initialize(characterDefinition.targetScanner);
        }
        else if (characterDefinition.targetScanner != null)
        {
            Debug.LogWarning($"[Entity] {gameObject.name} has a target scanner profile but no TargetScanner component.", this);
        }

        if (Interactor != null)
        {
            Interactor.Initialize(characterDefinition.interactorProfile);
        }
        else if (characterDefinition.interactorProfile != null)
        {
            Debug.LogWarning($"[Entity] {gameObject.name} has an interactor profile but no AutoInteractor component.", this);
        }
    }

    /// <summary>
    /// Manually triggers reinitialization of the entity. Useful when the characterDefinition 
    /// is assigned at runtime or when you need to refresh the configuration.
    /// </summary>
    public void Reinitialize()
    {
        if (_characterDefinition == null)
        {
            Debug.LogWarning($"[Entity] Cannot reinitialize {gameObject.name} - CharacterDefinitionSO is not assigned.", this);
            return;
        }

        InitializeFromDefinition();
    }
}
