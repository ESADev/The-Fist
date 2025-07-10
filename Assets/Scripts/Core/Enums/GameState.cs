using UnityEngine;

/// <summary>
/// Represents the high level state of the game.
/// </summary>
public enum GameState
{
    /// <summary>
    /// The game is at the main menu.
    /// </summary>
    MainMenu,

    /// <summary>
    /// Gameplay is active.
    /// </summary>
    Gameplay,

    /// <summary>
    /// Gameplay is paused.
    /// </summary>
    Paused,

    /// <summary>
    /// The player achieved victory.
    /// </summary>
    Victory,

    /// <summary>
    /// The player was defeated.
    /// </summary>
    Defeat
}
