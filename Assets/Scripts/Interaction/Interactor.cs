using UnityEngine;

/// <summary>
/// Represents an entity capable of interacting with objects implementing <see cref="IInteractable"/>.
/// </summary>
public class Interactor : MonoBehaviour
{
    [Header("Configuration")]

    /// <summary>
    /// The faction this interactor belongs to.
    /// </summary>
    [Tooltip("The faction this interactor belongs to.")]
    public FactionType faction = FactionType.Player;
}
