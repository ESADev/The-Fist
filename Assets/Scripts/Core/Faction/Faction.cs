using UnityEngine;

/// <summary>
/// Identifies the faction affiliation of a <see cref="GameObject"/>.
/// Components can query this component to determine friend or foe relationships.
/// </summary>
public class Faction : MonoBehaviour
{
    /// <summary>
    /// Gets the current faction of this object.
    /// </summary>
    public FactionType CurrentFaction { get; private set; } = FactionType.Neutral;

    /// <summary>
    /// Initializes the faction with a specific value at runtime.
    /// </summary>
    /// <param name="faction">Faction to assign.</param>
    public void Initialize(FactionType faction)
    {
        CurrentFaction = faction;
    }
}
