using UnityEngine;

/// <summary>
/// Identifies the faction affiliation of a <see cref="GameObject"/>.
/// Components can query this component to determine friend or foe relationships.
/// </summary>
public class Faction : MonoBehaviour
{
    [Header("Faction")]
    [Tooltip("Initial faction alignment of this object.")]
    [SerializeField]
    private FactionType startingFaction = FactionType.Neutral;

    /// <summary>
    /// Gets the current faction of this object.
    /// </summary>
    public FactionType CurrentFaction { get; private set; } = FactionType.Neutral;

    private void Awake()
    {
        CurrentFaction = startingFaction;
    }

    /// <summary>
    /// Initializes the faction with a specific value at runtime.
    /// </summary>
    /// <param name="faction">Faction to assign.</param>
    public void Initialize(FactionType faction)
    {
        CurrentFaction = faction;
    }
}
