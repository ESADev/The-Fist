using UnityEngine;

/// <summary>
/// Represents the faction alignment of an entity within the game world.
/// </summary>
public enum FactionType
{
    /// <summary>
    /// The faction controlled by the player.
    /// </summary>
    Player,

    /// <summary>
    /// The faction controlled by enemy AI.
    /// </summary>
    Enemy,

    /// <summary>
    /// A neutral faction that is neither allied with the player nor the enemy.
    /// </summary>
    Neutral
}
