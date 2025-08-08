using UnityEngine;

/// <summary>
/// Controls runtime behaviour for an individual level such as spawning
/// gameplay elements and checking win conditions.
/// </summary>
public class LevelManager : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("Data asset describing this level.")]
    public GameLevelDataSO data;

    /// <summary>
    /// Reference to the enemy's main base in the scene.
    /// </summary>
    [Tooltip("Reference to the player's main base in the scene.")]
    public Entity playerMainBase;

    /// <summary>
    /// Reference to the enemy's main base in the scene.
    /// </summary>
    [Tooltip("Reference to the enemy's main base in the scene.")]
    public Entity enemyMainBase;

    private void Update()
    {
        CheckWin();
    }

    /// <summary>
    /// Initializes the level using the provided data asset.
    /// </summary>
    /// <param name="levelData">Data asset for the level.</param>
    public void InitializeLevel(GameLevelDataSO levelData)
    {
        data = levelData;
        if (data == null)
        {
            Debug.LogError("[LevelManager] GameLevelDataSO is null.", this);
            return;
        }
    }

    /// <summary>
    /// Checks if the player has fulfilled win conditions.
    /// </summary>
    private void CheckWin()
    {
        if (enemyMainBase.Health.IsDead)
        {
            Debug.Log("[LevelManager] Enemy base destroyed. Triggering victory.");
            GameEvents.TriggerOnVictory();
            enabled = false;
        }
        else if (playerMainBase.Health.IsDead)
        {
            Debug.Log("[LevelManager] Player base destroyed. Triggering defeat.");
            GameEvents.TriggerOnDefeat();
            enabled = false;
        }
    }
}