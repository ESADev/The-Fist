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
    [Space]
    [Tooltip("How much in game spawn rate scaler will increase per second. Higher value means faster difficulty increasement.")]
    public float inGameSpawnRateSlope = 0.0015f;
}
