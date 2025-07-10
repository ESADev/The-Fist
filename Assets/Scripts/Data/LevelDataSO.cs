using UnityEngine;

/// <summary>
/// ScriptableObject containing configuration for a single level.
/// </summary>
[CreateAssetMenu(fileName = "LevelData", menuName = "TheFist/Level Data")]
public class LevelDataSO : ScriptableObject
{
    [Header("Identification")]
    [Tooltip("Unique index for the level.")]
    public int levelIndex;

    [Tooltip("Display name of the level.")]
    public string levelName = "New Level";

    [Header("Gameplay")]
    [Tooltip("How often enemies spawn (in seconds or rate units).")]
    public float enemySpawnRate = 1f;
}
