using UnityEngine;

/// <summary>
/// Controls runtime behaviour for an individual level such as spawning
/// gameplay elements and checking win conditions.
/// </summary>
public class LevelManager : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("Data asset describing this level.")]
    public LevelDataSO data;

    /// <summary>
    /// Reference to the enemy's main base in the scene.
    /// </summary>
    [Tooltip("Reference to the enemy's main base in the scene.")]
    public GameObject enemyMainBase;

    private void Update()
    {
        CheckWin();
    }

    /// <summary>
    /// Initializes the level using the provided data asset.
    /// </summary>
    /// <param name="levelData">Data asset for the level.</param>
    public void InitializeLevel(LevelDataSO levelData)
    {
        data = levelData;
        if (data == null)
        {
            Debug.LogError("[LevelManager] LevelDataSO is null.", this);
            return;
        }

        Debug.Log($"[LevelManager] Initializing level: {data.levelName}");
        SpawnLevelElements();
    }

    /// <summary>
    /// Spawns all gameplay elements defined for the level.
    /// </summary>
    private void SpawnLevelElements()
    {
        // Implementation specific to the game would go here.
        // For now we simply log for visibility.
        Debug.Log("[LevelManager] Spawning level elements.");
    }

    /// <summary>
    /// Checks if the player has fulfilled win conditions.
    /// </summary>
    private void CheckWin()
    {
        if (enemyMainBase == null)
        {
            Debug.Log("[LevelManager] Enemy base destroyed. Triggering victory.");
            GameEvents.TriggerOnVictory();
            enabled = false;
        }
    }
}
