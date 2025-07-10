using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Central coordinator for high level game flow such as loading levels and
/// tracking the current <see cref="GameState"/>.
/// </summary>
public class GameManager : MonoBehaviour
{
    /// <summary>
    /// Global singleton instance.
    /// </summary>
    public static GameManager Instance { get; private set; }

    /// <summary>
    /// Current game state.
    /// </summary>
    public GameState CurrentState { get; private set; } = GameState.MainMenu;

    [Header("Levels")]
    [Tooltip("Collection of levels available in the game.")]
    public List<LevelDataSO> levels = new List<LevelDataSO>();

    /// <summary>
    /// Currently loaded level index.
    /// </summary>
    private int currentLevelIndex = -1;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SubscribeToGameEvents();
    }

    private void OnDisable()
    {
        UnsubscribeFromGameEvents();
    }

    /// <summary>
    /// Subscribes to global events used by the game manager.
    /// </summary>
    private void SubscribeToGameEvents()
    {
        GameEvents.OnVictory += HandleVictory;
        GameEvents.OnDefeat += HandleDefeat;
    }

    /// <summary>
    /// Unsubscribes from global events when the game manager is disabled.
    /// </summary>
    private void UnsubscribeFromGameEvents()
    {
        GameEvents.OnVictory -= HandleVictory;
        GameEvents.OnDefeat -= HandleDefeat;
    }

    /// <summary>
    /// Loads a level by index and initializes gameplay.
    /// </summary>
    /// <param name="levelIndex">Index of the level to load.</param>
    public void LoadLevel(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex >= levels.Count)
        {
            Debug.LogError($"[GameManager] Invalid level index {levelIndex}.", this);
            return;
        }

        LevelDataSO levelData = levels[levelIndex];
        if (levelData == null)
        {
            Debug.LogError($"[GameManager] Level data at index {levelIndex} is null.", this);
            return;
        }

        currentLevelIndex = levelIndex;
        CurrentState = GameState.Gameplay;
        Debug.Log($"[GameManager] Loading level {levelData.levelName} ({levelIndex}).");

        SceneManager.LoadScene(levelData.levelName);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    /// <summary>
    /// Called after a scene has loaded to initialize the level.
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        LevelDataSO levelData = null;
        if (currentLevelIndex >= 0 && currentLevelIndex < levels.Count)
        {
            levelData = levels[currentLevelIndex];
        }

        LevelManager levelManager = FindObjectOfType<LevelManager>();
        if (levelManager != null)
        {
            levelManager.InitializeLevel(levelData);
        }
        else
        {
            Debug.LogError("[GameManager] No LevelManager found in loaded scene.", this);
        }
    }

    /// <summary>
    /// Handles logic when the game ends in victory.
    /// </summary>
    private void HandleVictory()
    {
        CurrentState = GameState.Victory;
        Debug.Log("[GameManager] Victory state reached.");
    }

    /// <summary>
    /// Handles logic when the game ends in defeat.
    /// </summary>
    private void HandleDefeat()
    {
        CurrentState = GameState.Defeat;
        Debug.Log("[GameManager] Defeat state reached.");
    }
}
