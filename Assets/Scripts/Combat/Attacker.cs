using UnityEngine;

/// <summary>
/// Basic component representing an entity that can deal damage.
/// </summary>
[RequireComponent(typeof(Collider))]
public class Attacker : MonoBehaviour
{
    [Header("Attack Settings")]

    /// <summary>
    /// The amount of damage inflicted when attacking.
    /// </summary>
    [Tooltip("The amount of damage inflicted when attacking.")]
    public float attackPower = 10f;

    /// <summary>
    /// The faction this attacker belongs to.
    /// </summary>
    [Tooltip("The faction this attacker belongs to.")]
    public FactionType faction = FactionType.Player;
}
