using UnityEngine;

/// <summary>
/// ScriptableObject containing configuration for a single gameplay level.
/// </summary>
[CreateAssetMenu(fileName = "GameLevelData", menuName = "TheFist/Game Level Data")]
public class GameLevelDataSO : ScriptableObject
{
    [Header("Identification")]
    [Tooltip("Unique index for the level.")]
    public int levelIndex;

    [Tooltip("Display name of the level.")]
    public string levelName = "New Level";

    [Header("Gameplay")]
    [Tooltip("How often enemies spawn.")]
    public float enemySpawnRateScaler = 1f;
}
